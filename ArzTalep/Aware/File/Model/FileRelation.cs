using System.ComponentModel.DataAnnotations.Schema;
using Aware.Model;
using Aware.Util;
using Aware.Util.Enum;

namespace Aware.File.Model
{
    public class FileRelation : BaseEntity
    {
        public string FileName { get; set; }

        public FileType Type { get; set; }

        public string Path { get; set; }

        public virtual string SortOrder { get; set; }

        public int RelationType { get; set; }

        public int RelationId { get; set; }

        [NotMapped]
        public virtual string Extension
        {
            get
            {
                return !string.IsNullOrEmpty(Path) ? System.IO.Path.GetExtension(Path) : string.Empty;
            }
        }

        public override bool IsValid()
        {
            return FileName.Valid() && Extension.Valid();
        }

        public virtual FileRelation Clone()
        {
            var result = MemberwiseClone() as FileRelation;
            result.ID = 0;
            return result;
        }
    }
}