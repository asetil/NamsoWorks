using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.Authority.Model
{
    public class AuthorityDefinition
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Title { get; set; }
        public virtual AuthorityType Type { get; set; }
        public virtual AuthorityMode Mode { get; set; }

        [NotMapped]
        public virtual AuthorityUsage Usage { get; set; }

        [NotMapped]
        public virtual bool HasUsage { get { return Usage != null && Usage.DefinitionID == (int)Type; } }
    }
}