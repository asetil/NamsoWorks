using System;

namespace Aware.Authenticate.Model
{
    [Serializable]
    public class FacebookProfileModel
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string birthday { get; set; }
    }
}