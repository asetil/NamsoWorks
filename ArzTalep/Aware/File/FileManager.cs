using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aware.BL.Model;
using Aware.Data;
using Aware.File.Model;
using Aware.Manager;
using Aware.Util;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Log;
using Aware.Util.Lookup;
using Microsoft.Extensions.Configuration;

namespace Aware.File
{
    public class FileManager : BaseManager<FileRelation>, IFileManager
    {
        private readonly ILookupManager _lookupManager;
        private readonly IConfiguration _configuration;

        public FileManager(IRepository<FileRelation> repository, IAwareLogger logger, ILookupManager lookupManager, IConfiguration configuration) : base(repository, logger)
        {
            _lookupManager = lookupManager;
            _configuration = configuration;
        }

        public FileRelation GetFile(int fileID, int relationID, int relationType)
        {
            FileRelation result = null;
            if (fileID > 0)
            {
                result = First(i => i.ID == fileID && i.RelationId == relationID && i.RelationType == relationType);
            }
            else if (fileID < 0)
            {
                result = First(i => i.RelationId == relationID && i.RelationType == relationType);
            }

            return result ?? new FileRelation()
            {
                RelationId = relationID,
                RelationType = relationType
            };
        }

        public FileGalleryModel GetGalleryForModel(int relationID, int relationType, string imageInfo, ViewTypes viewMode, int size = 3)
        {
            if (relationID > 0 && relationType > 0)
            {
                var model = new FileGalleryModel()
                {
                    RelationID = relationID,
                    RelationType = relationType,
                    Size = size,
                    ViewMode = viewMode
                };
                model.SetFileInfo(imageInfo);
                return model;
            }
            return null;
        }

        public FileGalleryModel GetGallery(int relationID, int relationType, int size)
        {
            try
            {
                var relations = SearchBy(i => i.RelationType == relationType && i.RelationId == relationID).ToList();
                if (relations != null)
                {
                    var gallery = new FileGalleryModel
                    {
                        RelationID = relationID,
                        RelationType = relationType,
                        Size = size,
                        Files = relations
                    };
                    return gallery;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|GetGallery", "RelationID:{1}, RelationType:{2}", ex, relationID, relationType);
            }
            return null;
        }

        public OperationResult<FileRelation> SaveGallery(FileRelation fileInfo, List<PostedFileModel> postedFiles)
        {
            try
            {
                if (fileInfo != null)
                {
                    var fileCount = postedFiles != null ? postedFiles.Count : 0;
                    if (fileCount > 1)
                    {
                        var errorCount = 0;
                        foreach (var postedFile in postedFiles)
                        {
                            var result = SaveFile(fileInfo.Clone(), postedFile, false);
                            errorCount += result.Ok ? 0 : 1;
                        }

                        RefreshFileRelationInfo(fileInfo.RelationId, fileInfo.RelationType);
                        if (errorCount < fileCount)
                        {
                            return Success();
                        }
                    }
                    else
                    {
                        var file = fileCount == 1 ? postedFiles.FirstOrDefault() : null;
                        return SaveFile(fileInfo, file, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveGallery", "--", ex);
            }
            return Failed(ResultCodes.Error.OperationFailed);
        }

        private OperationResult<FileRelation> SaveFile(FileRelation fileInfo, PostedFileModel file, bool refreshRelations)
        {
            try
            {
                if (fileInfo != null)
                {
                    var hasFile = file != null && file.HasStream;
                    if (hasFile)
                    {
                        if (!file.IsExtensionValid)
                        {
                            return Failed(ResultCodes.Error.File.InvalidExtension);
                        }

                        if (!fileInfo.FileName.Valid()) { fileInfo.FileName = file.Name; }
                        var idInfo = fileInfo.ID > 0 ? fileInfo.ID.ToString() : "#";
                        var relationName = _lookupManager.GetLookupName(LookupType.RelationTypes, fileInfo.RelationType.ToString());
                        fileInfo.Path = string.Format("{0}/{1}{2}", relationName, idInfo, Path.GetExtension(file.Name));
                    }

                    var result = SaveFileRelation(fileInfo);
                    if (result == null || result.ID == 0)
                    {
                        throw new Exception("Yeni dosya kaydedilirken hata!!!");
                    }

                    if (refreshRelations)
                    {
                        RefreshFileRelationInfo(result.RelationId, result.RelationType);
                    }

                    if (hasFile && !string.IsNullOrEmpty(result.Path))
                    {
                        var path = string.Format("{0}{1}", _configuration.GetValue("ImageRepository"), result.Path);
                        SaveStream(file.Stream, path, result.RelationType);
                    }
                    return Success(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveFile", "FileID:{0}, RelationType: {1}, RelationID : {2}", ex, fileInfo.ID, fileInfo.RelationType, fileInfo.RelationId);
            }
            return Failed(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<string> SaveFileToDisc(int relationID, int relationType, PostedFileModel postedFile)
        {
            try
            {
                var hasFile = postedFile != null && postedFile.HasStream;
                if (hasFile && relationType > 0 && relationID > 0)
                {
                    if (!postedFile.IsExtensionValid)
                    {
                        return Failed<string>(ResultCodes.Error.File.InvalidExtension);
                    }

                    var relationName = _lookupManager.GetLookupName(LookupType.RelationTypes, relationType.ToString());
                    var filePath = string.Format("{0}/{1}{2}", relationName, relationID, Path.GetExtension(postedFile.Name));
                    var savePath = _configuration.GetValue("ImageRepository") + filePath;

                    if (SaveStream(postedFile.Stream, savePath, default(int)))
                    {
                        RefreshFileRelationInfo(relationID, relationType, filePath);
                        return Success(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveFileToDisc", "relationID:{0}, relationType: {1}", ex, relationID, relationType);
            }
            return Failed<string>(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<FileRelation> SaveFilesToDirectory(string path, List<PostedFileModel> postedFiles)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && postedFiles != null && postedFiles.Any())
                {
                    foreach (var postedFile in postedFiles)
                    {
                        if (!postedFile.HasStream)
                        {
                            return Failed(ResultCodes.Error.File.InvalidFile);
                        }

                        if (!postedFile.IsExtensionValid)
                        {
                            return Failed(ResultCodes.Error.File.InvalidExtension);
                        }

                        var filePath = string.Format("{0}/{1}", path, postedFile.Name);
                        if (!SaveStream(postedFile.Stream, filePath, default(int)))
                        {
                            throw new Exception("Dosya kaydedilemedi : " + filePath);
                        }
                    }
                    return Success();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveFilesToDirectory", "path: {0}", ex, path);
            }
            return Failed(ResultCodes.Error.OperationFailed);
        }

        private FileRelation SaveFileRelation(FileRelation model)
        {
            try
            {
                if (model == null) { return null; }
                if (model.ID <= 0)
                {
                    Save(model);

                    model.Path = model.Path.Replace("#", model.ID.ToString());  //Update path info
                    Save(model);
                    return model;
                }

                var file = Get(model.ID);
                if (file == null) { throw new Exception("File not found!"); }

                Save(file);
                return file;
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveFileInfo", "ID:{0}!", ex, model.ID);
            }
            return null;
        }

        public OperationResult<bool> Delete(int fileID, int relationID, int relationType, bool deletePhysicalFile)
        {
            try
            {
                if (fileID > 0 && relationID > 0 && relationType > 0)
                {
                    var file = First(i => i.ID == fileID && i.RelationType == relationID && i.RelationType == relationType);
                    if (file != null && file.ID > 0)
                    {
                        var filePath = string.Format("{0}{1}", _configuration.GetValue("ImageRepository"), file.Path);
                        Delete(file.ID);

                        if (deletePhysicalFile)
                        {
                            DeletePhysicalFile(filePath);
                        }
                    }

                    RefreshFileRelationInfo(relationID, relationType);
                    return Success<bool>();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|Delete", "ID : {0}, RelationID:{1}, RelationType:{2}", ex, fileID, relationID, relationType);
            }
            return Failed<bool>(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<bool> DeletePhysicalFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        return Success<bool>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|DeletePhysicalFile", "path:{0}", ex, path);
            }
            return Failed<bool>(ResultCodes.Error.OperationFailed);
        }

        private bool RefreshFileRelationInfo(int relationID, int relationType, string fileInfo = "")
        {
            if (relationID > 0 && relationType > 0)
            {
                //TODO osokuoglu!
                //var updateSQL = ECommerce.Util.SqlHelper.RefreshFileRelationInfo(relationID, relationType, fileInfo);
                //_fileRelationRepository.ExecuteSp(updateSQL);
                return true;
            }
            return false;
        }


        private bool SaveStream(Stream stream, string path, int relationType)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception("path is empty or null!");
                }

                if (stream == null || stream.Length <= 0)
                {
                    throw new Exception("stream is not valid!");
                }

                ////Resize image on before save
                //_resizer.ResizeImage(stream, path, relationType);

                using (var fileStream = System.IO.File.Create(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }

                //using (var fileStream = System.IO.File.Create(path, (int)stream.Length))
                //{
                //    byte[] bytesInStream = new byte[stream.Length];
                //    stream.Read(bytesInStream, 0, bytesInStream.Length);

                //    // Use write method to write to the file specified above
                //    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                //    fileStream.Dispose();
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("FileService|SaveToDisc", "path:{0}", ex, path);
                throw ex;
            }
            return true;
        }

        protected override OperationResult<FileRelation> OnBeforeUpdate(ref FileRelation existing, FileRelation model)
        {
            if (existing != null && model != null)
            {
                existing.FileName = model.FileName;
                existing.Path = model.Path;
                existing.SortOrder = model.SortOrder;
                existing.Status = model.Status;
                existing.Type = model.Type;
                return Success();
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }
    }
}