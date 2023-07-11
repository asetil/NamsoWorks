using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aware.Dependency;
using Aware.Task.Model;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Task.Web
{
    public class BaseTask : ITask
    {
        public TaskStatus ExecutionStatus { get; set; }
        public bool RunNow { get; set; }
        public TaskDefinition Definition { get; set; }
        public int Counter { get; set; }
        public string[] ExecutionParams { get; private set; }

        public virtual Result Execute()
        {
           return Result.Error("Not executed!");
        }

        public virtual TaskType Type
        {
            get { return TaskType.None; }
        }

        public void SetDefinition(TaskDefinition definition)
        {
            Definition = definition;
            Definition.WorkTimesInfo = Definition!=null ? GetWorkTimeInfo():string.Empty;
        }

        private List<Trigger> _triggers;
        public List<Trigger> TriggerList
        {
            get
            {
                if (_triggers == null && !string.IsNullOrEmpty(Definition.TriggerDefinition))
                {
                    _triggers = Definition.TriggerDefinition.DeSerialize<IEnumerable<Trigger>>().ToList();
                }
                return _triggers;
            }
        }

        public void Run()
        {
            try
            {
                if (ExecutionStatus == TaskStatus.Executing || Definition.Status != Statuses.Active)
                {
                    ExecutionStatus = Definition.Status != Statuses.Active ? TaskStatus.Sleep : ExecutionStatus;
                    return;
                }

                var execute = RunNow;
                var executionParam = ExecutionParams;
                
                if (!execute)
                {
                    var trigger = ControlTriggers(DateTime.Now);
                    if (trigger != null)
                    {
                        execute = true;
                        executionParam = new string[] {trigger.Value};
                    }
                }

                if (execute)
                {
                    RunNow = false;
                    ExecutionStatus = TaskStatus.Executing;

                    Definition.LastBegin = DateTime.Now;
                    Definition.LastMessage = "Çalışıyor";
                    Counter++;

                    Logger.Info(string.Format("{0} started to run", Definition.Name));

                    SetRunParams(executionParam);
                    var result = Execute();
                    SetRunParams(null);

                    Logger.Info(string.Format("{0} finished", Definition.Name));
                    Logger.Info(
                        "----------------------------------------------------------------------------------------\n");

                    Definition.LastMessage = result.Message;
                    Definition.LastEnd = DateTime.Now;
                    ExecutionStatus = result.OK ? TaskStatus.Success : TaskStatus.Failed;
                }
                else
                {
                    ExecutionStatus = TaskStatus.Waiting;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("BaseTask > Run - Fail for {0}", Definition.Name), ex);
                Definition.LastEnd = DateTime.Now;
                Definition.LastMessage = string.Format("Error Executing Task : {0}, Exception :{1}", Definition.Name, ex.Message);
                ExecutionStatus = TaskStatus.Failed;
            }

            OnSetLastRunEvent(this);
        }

        private Trigger ControlTriggers(DateTime dateTimeNow)
        {
            Trigger result = null;
            if (TriggerList != null)
            {
                foreach (Trigger trigger in TriggerList)
                {
                    var willExecute = WillTriggerExecute(trigger, dateTimeNow);
                    if (willExecute)
                    {
                        result = trigger;
                        break;
                    }
                }
            }
            return result;
        }

        private bool WillTriggerExecute(Trigger trigger, DateTime dateTimeNow)
        {
            bool result = default(bool);
            if (IsDateInRange(trigger, dateTimeNow) && IsTimeInRange(trigger, dateTimeNow))
            {
                switch (trigger.TriggerType)
                {
                    case TriggerType.RunOnce:
                        if (Counter == 0) result = IsTimeValid(trigger, dateTimeNow);
                        break;
                    case TriggerType.RunDaily:
                        result = IsTimeValid(trigger, dateTimeNow);
                        break;
                    case TriggerType.RunWeekly:
                        DaysOfTheWeek daysOfTheWeek = (DaysOfTheWeek)Enum.Parse(typeof(DaysOfTheWeek), dateTimeNow.DayOfWeek.ToString());
                        result = ((trigger.WeekDays & daysOfTheWeek) == daysOfTheWeek) && IsTimeValid(trigger, dateTimeNow);
                        break;
                }
            }
            return result;
        }

        private static bool IsDateInRange(Trigger trigger, DateTime dateTimeNow)
        {
            return (!trigger.BeginDate.HasValue || (trigger.BeginDate <= dateTimeNow)) &&
                   (!trigger.EndDate.HasValue || (trigger.EndDate >= dateTimeNow));
        }

        private static bool IsTimeInRange(Trigger trigger, DateTime dateTimeNow)
        {
            DateTime startTime = Convert.ToDateTime(trigger.StartTime);
            bool result = dateTimeNow.TimeOfDay.Ticks > startTime.TimeOfDay.Ticks;

            if (result && !string.IsNullOrEmpty(trigger.EndTime))
            {
                DateTime endTime = Convert.ToDateTime(trigger.EndTime);
                if (endTime < startTime)
                {
                    result = dateTimeNow.TimeOfDay.Ticks > endTime.TimeOfDay.Ticks;
                }
                else
                {
                    result = dateTimeNow.TimeOfDay.Ticks < endTime.TimeOfDay.Ticks;
                }
            }
            return result;
        }

        private static bool IsTimeValid(Trigger trigger, DateTime dateTimeNow)
        {
            bool result = false;
            DateTime startTime = Convert.ToDateTime(trigger.StartTime);
            if (trigger.IntervalMinutes == 0)
            {
                if ((int)dateTimeNow.TimeOfDay.TotalMinutes == (int)startTime.TimeOfDay.TotalMinutes)
                {
                    result = true;
                }
            }
            else
            {
                int dif = (int)dateTimeNow.TimeOfDay.TotalMinutes - (int)startTime.TimeOfDay.TotalMinutes;
                if (dif < 0)
                {
                    dif += 24 * 60;
                }

                if ((dif % trigger.IntervalMinutes) == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        public void SetStatus(TaskStatus status)
        {
            ExecutionStatus = status;
        }

        public void SetRunParams(params string[] runParams)
        {
            ExecutionParams = runParams;
        }

        public string StatusString
        {
            get { return ExecutionStatus.ToString(); }
        }

        public delegate void SetLastRun(ITask task);
        public event SetLastRun SetLastRunEvent;
        protected virtual void OnSetLastRunEvent(ITask task)
        {
            var handler = SetLastRunEvent;
            if (handler != null)
            {
                handler(task);
            }
        }

        public bool IsValid()
        {
            bool result = true;
            if (Definition == null)
            {
                Logger.Error("Task definition is null",null);
                return false;
            }

            if (string.IsNullOrEmpty(Definition.Name))
            {
                Logger.Error("Null Task Name: {0}", null,Definition.Name);
                return false;
            }

            if (Definition.Type==TaskType.None)
            {
                Logger.Error("Task Type not valid", null);
                return false;
            }

            if (TriggerList == null || TriggerList.Count <= 0)
            {
                Logger.Error("No Trigger Found for {0}", null, Definition.Name);
                return false;
            }

            foreach (var trigger in TriggerList)
            {
                if (string.IsNullOrEmpty(trigger.StartTime))
                {
                    result = false;
                    Logger.Error(string.Format("Not Have StartTime for {0}", Definition.Name), null);
                }
                else
                {
                    DateTime startTime;
                    if (!DateTime.TryParse(trigger.StartTime, out startTime))
                    {
                        result = false;
                        Logger.Error(string.Format("Not Valid StartTime for {0}", Definition.Name), null);
                    }
                }

                if (!string.IsNullOrEmpty(trigger.EndTime))
                {
                    DateTime endTime;
                    if (!DateTime.TryParse(trigger.StartTime, out endTime))
                    {
                        result = false;
                        Logger.Error(string.Format("Not Valid EndTime for {0}", Definition.Name), null);
                    }
                }

                if (trigger.IntervalMinutes < 0)
                {
                    result = false;
                    Logger.Error(string.Format("Not Valid IntervalMinutes for {0}", Definition.Name), null);
                }

                if (!string.IsNullOrEmpty(trigger.StartTime) && !string.IsNullOrEmpty(trigger.EndTime) &&
                    trigger.IntervalMinutes <= 0)
                {
                    result = false;
                    Logger.Error(string.Format("Must Have IntervalMinutes for {0}", Definition.Name), null);
                }

                if (trigger.TriggerType == TriggerType.RunWeekly &&
                    string.IsNullOrEmpty(trigger.WeekDays.ToString()))
                {
                    result = false;
                    Logger.Error(string.Format("Not Have WeekDays for {0}", Definition.Name), null);
                }

                if (trigger.BeginDate.HasValue && trigger.EndDate.HasValue && trigger.BeginDate >= trigger.EndDate)
                {
                    result = false;
                    Logger.Error(string.Format("BeginDate >= EndDate for {0}", Definition.Name), null);
                }
            }
            return result;
        }

        public bool CanBeRun(bool runNow=false)
        {
            if (ExecutionStatus == TaskStatus.Executing || Definition.Status != Statuses.Active)
            {
                ExecutionStatus = Definition.Status != Statuses.Active ? TaskStatus.Sleep : ExecutionStatus;
                return false;
            }

            if (runNow || RunNow)
            {
                return true;
            }

            var trigger = ControlTriggers(DateTime.Now);
            if (trigger != null)
            {
                SetRunParams(new string[] { trigger.Value });
                return true;
            }

            ExecutionStatus = TaskStatus.Waiting;
            return false;
        }

        public string GetWorkTimeInfo()
        {
            var info = new StringBuilder();
            foreach (var trigger in TriggerList)
            {
                switch (trigger.TriggerType)
                {
                    case TriggerType.RunDaily:
                        info.Append("Günlük - "); break;
                    case TriggerType.RunOnce:
                        info.Append("Bir kez - "); break;
                    case TriggerType.RunWeekly:
                        info.Append("Her hafta - ");
                        info.Append(trigger.GetWeekDayName() + " - ");
                        break;
                }

                info.AppendFormat("({0}", trigger.StartTime);
                if (!string.IsNullOrEmpty(trigger.EndTime))
                {
                    info.AppendFormat(" -> {0}", trigger.EndTime);
                }

                if (trigger.IntervalMinutes > 0)
                {
                    info.AppendFormat(" - her {0} dk", trigger.IntervalMinutes);
                }
                info.Append(")<br/>");
            }
            return info.ToString();
        }

        protected ILogger Logger
        {
            get { return WindsorBootstrapper.Resolve<ILogger>(); }
        }
    }
}
