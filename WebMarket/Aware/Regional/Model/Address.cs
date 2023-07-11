using System.ComponentModel.DataAnnotations.Schema;
using Aware.Authenticate.Model;
using Aware.Util.Enums;

namespace Aware.Regional.Model
{
    public class Address
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual int CityID { get; set; }
        public virtual int CountyID { get; set; }
        public virtual int DistrictID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Phone { get; set; }
        public virtual int UserID { get; set; }
        public virtual Statuses Status { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public virtual string DisplayText { get; set; }
    }
}