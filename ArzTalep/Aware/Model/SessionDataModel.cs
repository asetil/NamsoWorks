using Aware.Util;
using Aware.Util.Enum;

namespace Aware.Model
{
    public class SessionDataModel
    {
        public string SessionKey { get; set; }

        public string Name { get; set; }

        public CustomerRole Role { get; set; }

        public bool IsValid
        {
            get
            {
                return SessionKey.Valid() && Name.Valid();
            }
        }
    }
}
