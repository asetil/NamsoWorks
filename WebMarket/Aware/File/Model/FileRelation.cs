using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.File.Model
{
    public class FileRelation
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Path { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual FileSize Size { get; set; }
        public virtual int RelationID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual Statuses Status { get; set; }

        public virtual FileRelation Clone()
        {
            var result = MemberwiseClone() as FileRelation;
            result.ID = 0;
            return result;
        }

        [NotMapped]
        public virtual string Extension {
            get
            {
                return !string.IsNullOrEmpty(Path) ? System.IO.Path.GetExtension(Path) : string.Empty;
            }
        }
    }
}