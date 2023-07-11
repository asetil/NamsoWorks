using System;
using Aware.Util;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.Task.Model
{
    public class TaskDefinition
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual TaskType Type { get; set; }
        public virtual Statuses Status { get; set; }
        public virtual string WorkTimesInfo { get; set; }
        public virtual string TriggerDefinition { get; set; }
        public virtual DateTime? LastBegin { get; set; }
        public virtual DateTime? LastEnd { get; set; }
        public virtual string LastMessage { get; set; }
    }
}