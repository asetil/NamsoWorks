using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Aware.Data;
using Aware.ECommerce.Enums;
using Aware.File.Model;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Log;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace Aware.File
{
    public class FileService : IFileService
    {
        private readonly IRepository<FileRelation> _fileRelationRepository;
        private readonly ILogger _logger;
        private readonly IImageResizer _resizer;
        private readonly ILookupManager _lookupManager;

        public FileService(IRepository<FileRelation> fileRelationRepository, ILogger logger, IImageResizer resizer, ILookupManager lookupManager)
        {
            _fileRelationRepository = fileRelationRepository;
            _logger = logger;
            _resizer = resizer;
            _lookupManager = lookupManager;
        }

        public FileRelation GetFile(int fileID, int relationID, int relationType)
        {
            FileRelation result = null;
            if (fileID > 0)
            {
                result = _fileRelationRepository.Where(i => i.ID == fileID && i.RelationID == relationID && i.RelationType == relationType).First();
            }
            else if (fileID < 0)
            {
                result = _fileRelationRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType).First();
            }

            return result ?? new FileRelation()
            {
                RelationID = relationID,
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
                var relations = _fileRelationRepository.Where(i => i.RelationType == relationType && i.RelationID == relationID).ToList();
                if (relations != null)
                {
                    var gallery = new FileGalleryModel
                    {
                        RelationID = relationID,
                        RelationType = relationType,
                        Size = size,
                        Files = relations.ToList()
                    };
                    return gallery;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > GetGallery - Fail RelationID:{1}, RelationType:{2}", ex, relationID, relationType);
            }
            return null;
        }

        public Result SaveGallery(FileRelation fileInfo, List<PostedFileModel> postedFiles)
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
                            errorCount += result.OK ? 0 : 1;
                        }

                        RefreshFileRelationInfo(fileInfo.RelationID, fileInfo.RelationType);
                        if (errorCount < fileCount)
                        {
                            return Result.Success(null, Resource.General_Success);
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
                _logger.Error("FileService > SaveGallery - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        private Result SaveFile(FileRelation fileInfo, PostedFileModel file, bool refreshRelations)
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
                            return Result.Error("Dosya uzantýsý  geçerli deðil!");
                        }

                        if (string.IsNullOrEmpty(fileInfo.Name)) { fileInfo.Name = file.Name; }
                        var idInfo = fileInfo.ID > 0 ? fileInfo.ID.ToString() : "#";
                        var relationName = _lookupManager.GetLookupName(LookupType.RelationTypes, fileInfo.RelationType);
                        fileInfo.Path = string.Format("{0}/{1}{2}", relationName, idInfo, Path.GetExtension(file.Name));
                    }

                    var result = SaveFileRelation(fileInfo);
                    if (result == null || result.ID == 0)
                    {
                        throw new Exception("Yeni dosya kaydedilirken hata!!!");
                    }

                    if (refreshRelations)
                    {
                        RefreshFileRelationInfo(result.RelationID, result.RelationType);
                    }

                    if (hasFile && !string.IsNullOrEmpty(result.Path))
                    {
                        var path = string.Format("{0}{1}", Config.ImageRepository, result.Path);
                        var savePath = HttpContext.Current.Server.MapPath(path);
                        SaveStream(file.Stream, savePath, result.RelationType);
                    }
                    return Result.Success(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("FileService > SaveFile - Fail with FileID:{0}, RelationType: {1}, RelationID : {2}", fileInfo.ID, fileInfo.RelationType, fileInfo.RelationID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveFileToDisc(int relationID, int relationType, PostedFileModel postedFile)
        {
            try
            {
                var hasFile = postedFile != null && postedFile.HasStream;
                if (hasFile && relationType > 0 && relationID > 0)
                {
                    if (!postedFile.IsExtensionValid)
                    {
                        return Result.Error("Dosya uzantýsý  geçerli deðil!");
                    }

                    var relationName = _lookupManager.GetLookupName(LookupType.RelationTypes, relationType);
                    var filePath = string.Format("{0}/{1}{2}",relationName, relationID, Path.GetExtension(postedFile.Name));
                    var savePath = HttpContext.Current.Server.MapPath(Config.ImageRepository + filePath);

                    if (SaveStream(postedFile.Stream, savePath, default(int)))
                    {
                        RefreshFileRelationInfo(relationID, relationType, filePath);
                        return Result.Success(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > SaveFileToDisc - Fail with relationID:{0}, relationType: {1}", ex, relationID, relationType);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveFilesToDirectory(string path, List<PostedFileModel> postedFiles)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && postedFiles != null && postedFiles.Any())
                {
                    foreach (var postedFile in postedFiles)
                    {
                        if (!postedFile.HasStream)
                        {
                            return Result.Error("Dosya geçerli deðil");
                        }

                        if (!postedFile.IsExtensionValid)
                        {
                            return Result.Error("Dosya uzantýsý  geçerli deðil!");
                        }

                        var filePath = string.Format("{0}/{1}",path, postedFile.Name);
                        var savePath = HttpContext.Current.Server.MapPath(filePath);

                        if (!SaveStream(postedFile.Stream, savePath, default(int)))
                        {
                            return Result.Error("Dosya kaydedilemedi : "+filePath);
                        }
                    }
                    return Result.Success();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > SaveFilesToDirectory - Fail with path: {0}", ex, path);
            }
            return Result.Error(Resource.General_Error);
        }

        private FileRelation SaveFileRelation(FileRelation model)
        {
            try
            {
                if (model == null) { return null; }
                if (model.ID <= 0)
                {
                    model.Status = Statuses.Active;
                    _fileRelationRepository.Add(model);

                    model.Path = model.Path.Replace("#", model.ID.ToString());  //Update path info
                    _fileRelationRepository.Update(model);
                    return model;
                }

                var file = _fileRelationRepository.Get(model.ID);
                if (file == null) { throw new Exception("File not found!"); }

                Mapper.Map(ref file, model);
                _fileRelationRepository.Update(file);
                return file;
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > SaveFileInfo -Fail with ID:{0}!", ex, model.ID);
            }
            return null;
        }

        public Result Delete(int fileID, int relationID, int relationType, bool deletePhysicalFile)
        {
            try
            {
                if (fileID > 0 && relationID > 0 && relationType > 0)
                {
                    var file = _fileRelationRepository.Where(i => i.ID == fileID && i.RelationID == relationID && i.RelationType == relationType).First();
                    if (file != null && file.ID > 0)
                    {
                        var filePath = HttpContext.Current.Server.MapPath(string.Format("{0}{1}", Config.ImageRepository, file.Path));
                        _fileRelationRepository.Delete(file.ID);

                        if (deletePhysicalFile && System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    RefreshFileRelationInfo(relationID, relationType);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > Delete - Fail with ID : {0}, RelationID:{1}, RelationType:{2}", ex, fileID, relationID, relationType);
            }
            return Result.Error(Resource.File_NotAvailable);
        }

        public Result DeletePhysicalFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var filePath = HttpContext.Current.Server.MapPath(path);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FileService > DeletePhysicalFile - Fail with path:{0}", ex, path);
            }
            return Result.Error(Resource.File_NotAvailable);
        }

        private bool RefreshFileRelationInfo(int relationID, int relationType,string fileInfo="")
        {
            if (relationID > 0 && relationType > 0)
            {
                var updateSQL = ECommerce.Util.SqlHelper.RefreshFileRelationInfo(relationID, relationType, fileInfo);
                _fileRelationRepository.ExecuteSp(updateSQL);
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
                    throw new Exception("FileService > SaveToDisc - path is empty or null!");
                }

                if (stream == null || stream.Length <= 0)
                {
                    throw new Exception("FileService > SaveToDisc - stream is not valid!");
                }

                //Resize image on before save
                _resizer.ResizeImage(stream, path, relationType);

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
                _logger.Error("FileService > SaveToDisc - failed", ex);
                throw ex;
            }
            return true;
        }
    }
}