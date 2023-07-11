using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.Notification
{
    public class NotificationDetailModel
    {
        public Notification Notification { get; set; }
        public List<Lookup> TargetList { get; set; }
        public List<Lookup> DisplayModeList { get; set; }
        public List<Lookup> StatusList { get; set; }
    }
}