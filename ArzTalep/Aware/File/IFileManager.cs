using System.Collections.Generic;
using Aware.BL.Model;
using Aware.File.Model;
using Aware.Manager;
using Aware.Util.Enum;

namespace Aware.File
{
    public interface IFileManager : IBaseManager<FileRelation>
    {
        FileRelation GetFile(int fileID, int relationID, int relationType);

        FileGalleryModel GetGalleryForModel(int relationID, int relationType, string imageInfo, ViewTypes viewMode, int size = 3);

        FileGalleryModel GetGallery(int relationID, int relationType, int size);

        OperationResult<FileRelation> SaveGallery(FileRelation fileInfo, List<PostedFileModel> postedFiles);

        OperationResult<string> SaveFileToDisc(int relationID, int relationType, PostedFileModel postedFile);

        OperationResult<FileRelation> SaveFilesToDirectory(string path, List<PostedFileModel> postedFiles);

        OperationResult<bool> Delete(int fileID, int relationID, int relationType, bool deletePhysicalFile);

        OperationResult<bool> DeletePhysicalFile(string path);
    }
}