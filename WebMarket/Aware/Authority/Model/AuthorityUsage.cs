using System;
using System.ComponentModel.DataAnnotations;
using Aware.Util.Enums;

namespace Aware.Authority.Model
{
    public class AuthorityUsage
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual int DefinitionID { get; set; } //Yetki tanımı
        public virtual int RelationID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual int AssignBy { get; set; } //Kim atadı
        public virtual int Quota { get; set; }
        public virtual int Usage { get; set; }
        public virtual DateTime ExpireDate { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual Statuses Status { get; set; }
    }
}