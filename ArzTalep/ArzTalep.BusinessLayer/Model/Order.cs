using Aware.Model;
using Aware.Util.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArzTalep.BL.Model
{
    [Table("MyOrders")]
    public class Order
    {
        [Key] //sets this field as primary key 
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //her insertte EF olustursun, default davranış
        public int Order_ID { get; set; }

        [Column("Aciklama")]
        public string Description { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        public DateTime RecordDate { get; set; }

        public StatusType Status { get; set; }
        
        public User Owner { get; set; } //Many-To-One

        public List<OrderItem> OrderItems { get; set; } //One-To-Many

        
    }

    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int Order_Id { get; set; }

        [Required(ErrorMessage = "Name is required")] //NULL değerlere izin vermez
        [MaxLength(50)]
        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        [NotMapped] //DB ye bu alanı kolon olarak eklemez
        public decimal Price
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
        
        public Order Order { get; set; } //Many-To-One

        public List<User> Buyers { get; set; } //Many-To-Many
    }
}
