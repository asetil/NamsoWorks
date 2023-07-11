using System.IO;

namespace Aware.File
{
    public interface IImageResizer
    {
        void ResizeImage(Stream stream, string path, int relationType);
    }
}