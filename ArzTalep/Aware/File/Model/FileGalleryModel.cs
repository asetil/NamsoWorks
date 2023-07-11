using Aware.Util.Enum;
using System.Collections.Generic;

namespace Aware.File.Model
{
    public class FileGalleryModel
    {
        public int RelationID { get; set; }

        public int RelationType { get; set; }

        public int Size { get; set; }

        public bool HasModal { get; set; }

        public List<FileRelation> Files { get; set; }

        public ViewTypes ViewMode { get; set; }

        public string AllowedExtensions { get; set; }

        public void SetFileInfo(string fileInfo, string allowedExtensions = ".jpg,.jpeg,.gif,.png")
        {
            if (!string.IsNullOrEmpty(fileInfo))
            {
                if (!string.IsNullOrEmpty(fileInfo))
                {
                    //TODO osokuoglu!
                    //Files = fileInfo.GetFiles();
                }
            }
            AllowedExtensions = allowedExtensions;
        }

        public static FileGalleryModel ModalInstance(string allowedExtensions = ".jpg,.jpeg,.gif,.png")
        {
            return new FileGalleryModel
            {
                HasModal = true,
                AllowedExtensions = allowedExtensions
            };
        }
    }
}
