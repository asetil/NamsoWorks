using System.Collections.Generic;
using Aware.File.Model;
using Aware.Util.Enums;
using Aware.Util.Model;

namespace Aware.File
{
    public interface IFileService
    {
        FileRelation GetFile(int fileID, int relationID, int relationType);
        FileGalleryModel GetGalleryForModel(int relationID, int relationType, string imageInfo, ViewTypes viewMode, int size = 3);
        FileGalleryModel GetGallery(int relationID, int relationType, int size);
        Result SaveGallery(FileRelation fileInfo,List<PostedFileModel> postedFiles);
        Result SaveFileToDisc(int relationID, int relationType, PostedFileModel postedFile);
        Result SaveFilesToDirectory(string path, List<PostedFileModel> postedFiles);
        Result Delete(int fileID, int relationID, int relationType, bool deletePhysicalFile);
        Result DeletePhysicalFile(string path);
    }
}