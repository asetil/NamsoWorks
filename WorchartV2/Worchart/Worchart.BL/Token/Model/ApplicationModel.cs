using System;
using System.Linq;
using Worchart.BL.Enum;
using Worchart.BL.Model;

namespace Worchart.BL.Token
{
    public class ApplicationModel : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ClientID { get; set; }
        public virtual string ClientSecret { get; set; }
        public virtual string AllowedIps { get; set; }
        public virtual short IsPublic { get; set; }

        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            return Name.Valid() && ClientID.Valid() && ClientSecret.Valid();
        }

        public virtual bool IsIpAllowed(string ip)
        {
            if (!ip.Valid())
            {
                return false;
            }

            if (AllowedIps.Valid())
            {
                var ipList = AllowedIps.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                return ipList.Contains(ip);
            }
            return true;
        }
    }
}
