using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.Notification
{
    public class NotificationListModel
    {
        public List<Notification> NotificationList { get; set; }
        public List<Lookup> TargetList { get; set; }
        public List<Lookup> DisplayModeList { get; set; }
    }
}