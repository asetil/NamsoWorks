using Aware.Util;
using System;
using System.Linq;

namespace Aware.Model
{
    public class ApplicationModel : BaseEntity
    {
        public string Name { get; set; }

        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string AllowedIps { get; set; }

        public short IsPublic { get; set; }

        //Mail settings
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public override bool IsValid()
        {
            return Name.Valid() && ClientID.Valid() && ClientSecret.Valid();
        }

        public bool IsIpAllowed(string ip)
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
