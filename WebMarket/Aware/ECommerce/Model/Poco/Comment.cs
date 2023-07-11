using System;
using System.ComponentModel.DataAnnotations;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class Comment
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Title { get; set; } 
        public virtual string Value { get; set; } //içerik
        public virtual decimal Rating { get; set; } //puan
        public virtual int RelationID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual int OwnerID { get; set; } //yorumlayan id
        public virtual string OwnerName { get; set; } //yorumlayan isim
        public virtual int EvaluatorID { get; set; } //Kim değerlendirdi
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual CommentStatus Status { get; set; }
    }
}