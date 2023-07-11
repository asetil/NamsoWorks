using Aware.Model;
using Aware.Util;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArzTalep.BL.Model
{
    public class Campaign : BaseEntity
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Product Product { get; set; }

        public override bool IsValid()
        {
            return Name.Valid() && UserCreated > 0;
        }
    }
}
