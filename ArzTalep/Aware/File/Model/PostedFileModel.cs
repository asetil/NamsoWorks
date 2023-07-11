using System.Collections.Generic;
using System.IO;

namespace Aware.File.Model
{
    public class PostedFileModel
    {
        public string Name { get; set; }

        public Stream Stream { get; set; }

        public string ContentType { get; set; }

        public int ContentLength { get; set; }

        public bool HasStream
        {
            get { return Stream != null && Stream.Length > 0; }
        }

        public bool IsExtensionValid
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    var extension = Path.GetExtension(Name).ToLowerInvariant();
                    return ImageExtensions.Contains(extension);
                }
                return false;
            }
        }

        public static List<string> ImageExtensions
        {
            get { return new List<string>() { ".jpg", ".png", ".gif" }; }
        }

        public static bool IsImage(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var extension = Path.GetExtension(path).ToLowerInvariant();
                return ImageExtensions.Contains(extension);

            }
            return false;
        }
    }
}
