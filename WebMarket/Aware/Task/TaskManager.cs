using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aware.Cache;
using Aware.Dependency;
using Aware.Task.Model;
using Aware.Task.Web;
using Aware.Util;
using Aware.Util.Log;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Task
{
    public class TaskManager : ITaskManager
    {
        public List<ITask> TaskList { get; private set; }
        public bool IsRunning { get; private set; }
        private const int Interval = 60000;
        private Timer Timer { get; set; }
        private readonly ICacher _cacher;
        private readonly ILogger _logger;
        private readonly object _lock;

        public TaskManager()
        {
            _cacher = WindsorBootstrapper.Resolve<ICacher>();
            _logger = WindsorBootstrapper.Resolve<ILogger>();
            _lock = new object();

            Initialize();
        }

        public void Initialize()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                LoadTaskList(true);
                Timer = new Timer(TimerHandler, null, Interval, Interval);
            }
        }

        public void Start()
        {
            IsRunning = true;
            if (Timer == null)
            {
                Timer = new Timer(TimerHandler, null, Interval, Interval);
            }

            foreach (var task in TaskList)
            {
                task.SetStatus(TaskStatus.Waiting);
            }
        }

        public void Stop()
        {
            try
            {
                foreach (var task in TaskList)
                {
                    task.SetStatus(TaskStatus.Sleep);
                }
                IsRunning = false;
                Timer = null;
            }
            catch (Exception ex)
            {
                _logger.Error("TaskManager > Stop - Fail", ex);
            }
        }

        public void RunImmediately(TaskType taskType, params string[] taskParams)
        {
            lock (_lock)
            {
                var task = TaskList.FirstOrDefault(item => item.Type == taskType);
                if (task != null)
                {
                    task.RunNow = true;
                    task.SetRunParams(taskParams);
                    var thread = new Thread(task.Run);
                    thread.Start();
                }
            }
        }

        public void Refresh()
        {
            IsRunning = false;
            Stop();
            Initialize();
        }

        private void SaveTaskContext(ITask task)
        {
            //TODO# implemente edilecek
        }

        private void LoadTaskList(bool refresh = false)
        {
            TaskList = _cacher.Get<List<ITask>>("CACHED_TASKS");
            if (TaskList == null || refresh)
            {
                var elasticIndexerTask = WindsorBootstrapper.Resolve<ElasticIndexerTask>();
                var productRefreshTask = WindsorBootstrapper.Resolve<ProductRefreshTask>();
                TaskList = new List<ITask>
                {
                    elasticIndexerTask,productRefreshTask
                };

                var taskDefinitions = GetActiveTasks();
                foreach (var task in TaskList)
                {
                    var definition = taskDefinitions.FirstOrDefault(i => i.Type == task.Type);
                    if (definition != null)
                    {
                        task.SetDefinition(definition);
                        if (task.IsValid())
                        {
                            task.SetLastRunEvent += SaveTaskContext;
                        }
                        else
                        {
                            task.SetDefinition(null);
                        }
                    }
                }
                TaskList.RemoveAll(i => i.Definition == null);
                _cacher.Add("CACHED_TASKS", TaskList);
            }
        }

        private void TimerHandler(object state)
        {
            if (Timer != null) { Timer.Change(-1, -1); }
            foreach (var task in TaskList)
            {
                if (task.CanBeRun())
                {
                    var thread = new Thread(task.Run);
                    thread.Start();
                    Thread.Sleep(2000);
                }
            }

            if (Timer != null)
            {
                Timer.Change(Interval, Interval);
            }
        }

        public TaskDefinition GetTask(int taskID)
        {
            var task = TaskList.FirstOrDefault(i => i.Definition.ID == taskID);
            return task != null ? task.Definition : null;
        }

        public IEnumerable<TaskDefinition> GetActiveTasks()
        {
            //return Find(i => i.Status == Statuses.Active);
            var result = new List<TaskDefinition>
            {
                new TaskDefinition()
                {
                    ID = 1,
                    Name = "Elastic Indexer",
                    Description = "Elastic indexer",
                    Status = Statuses.Active,
                    Type = TaskType.ElasticIndexer,
                    TriggerDefinition =
                        @"[{'TriggerType':1,'BeginDate':null,'EndDate':null,'WeekDays':0,'StartTime':'00:10','EndTime':'','IntervalMinutes':10}]"
                },
                new TaskDefinition()
                {
                    ID = 2,
                    Name = "Product Refresh Task",
                    Description = "Product Refresh Task",
                    Status = Statuses.Active,
                    Type = TaskType.ProductRefresh,
                    TriggerDefinition =
                        @"[{'TriggerType':1,'BeginDate':null,'EndDate':null,'WeekDays':0,'StartTime':'00:10','EndTime':'','IntervalMinutes':0,'Value':0}]"
                }
            };

            return result;
        }
    }
}