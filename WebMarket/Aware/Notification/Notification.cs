using System;
using System.Web.Mvc;
using Aware.ECommerce.Model;
using Aware.Util.Enums;

namespace Aware.Notification
{
    public class Notification : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }

        [AllowHtml]
        public virtual string Content { get; set; }
        public virtual NotificationTarget Target { get; set; }
        public virtual string TargetList { get; set; }
        public virtual NotificationDisplayMode DisplayMode { get; set; }
        public virtual DateTime PublishDate { get; set; }
        public virtual int Expire { get; set; }
        public virtual Statuses Status { get; set; }
    }

    public enum NotificationTarget
    {
        None=0,
        All=1,
        NonMember=2,
        Members=3,
        ExcludeMembers=4
    }

    public enum NotificationDisplayMode
    {
        None = 0,
        Message = 1,
        Popup = 2,
    }
}