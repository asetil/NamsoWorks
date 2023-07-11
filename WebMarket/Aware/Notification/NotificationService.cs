using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate;
using Aware.Data;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Log;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace Aware.Notification
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILookupManager _lookupManager;

        public NotificationService(ISessionManager sessionManager,IRepository<Notification> notificationRepository, ILogger logger, ILookupManager lookupManager)
            :base(notificationRepository,logger)
        {
            _lookupManager = lookupManager;
            _sessionManager = sessionManager;
        }

        public NotificationListModel GetAllNotifications()
        {
            return new NotificationListModel()
            {
                NotificationList = Repository.Where(i => i.ID > 0).ToList(),
                TargetList = _lookupManager.GetLookups(LookupType.NotificationTargets),
                DisplayModeList = _lookupManager.GetLookups(LookupType.NotificationDisplayModes),
            };
        }

        public NotificationDetailModel GetNotification(int notificationID)
        {
            try
            {
                var notification= notificationID > 0 ? Repository.Get(notificationID) : new Notification() { PublishDate = DateTime.Now };
                var result=new NotificationDetailModel()
                {
                     Notification = notification,
                     DisplayModeList = _lookupManager.GetLookups(LookupType.NotificationDisplayModes),
                     TargetList = _lookupManager.GetLookups(LookupType.NotificationTargets),
                     StatusList = _lookupManager.GetLookups(LookupType.Status),
                };
                return result;
            }
            catch (Exception ex)
            {
               Logger.Error("NotificationService > GetNotification - failed for id:{0}",ex,notificationID);
            }
            return null;
        }

        public Notification GetUserNotification(int userID)
        {
            try
            {
                var displayedList = _sessionManager.GetDisplayedNotifications();
                var activeNotifications = Repository.Where(i => !displayedList.Contains(i.ID) && i.DisplayMode == NotificationDisplayMode.Popup && i.Status == Statuses.Active
                                  && i.PublishDate <= DateTime.Now && i.PublishDate.AddDays(i.Expire) > DateTime.Now).ToList();

                if (userID > 0)
                {
                    activeNotifications = activeNotifications.Where(i =>
                        i.Target == NotificationTarget.All && ContainsUser(i.TargetList, userID)
                        || i.Target == NotificationTarget.Members && ContainsUser(i.TargetList, userID)
                        || i.Target == NotificationTarget.ExcludeMembers && !ContainsUser(i.TargetList, userID)).OrderByDescending(i => i.PublishDate).ToList();
                }
                else
                {
                    activeNotifications = activeNotifications.Where(i => i.Target == NotificationTarget.All || i.Target == NotificationTarget.NonMember).OrderByDescending(i => i.PublishDate).ToList();
                }

                var result = activeNotifications.FirstOrDefault();
                if (result != null)
                {
                    displayedList.Add(result.ID);
                    var hasNotification = activeNotifications.Count() > 1;
                    _sessionManager.SetNotificationCookie(displayedList,hasNotification);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("NotificationService > GetUserNotification - Failed userID:{0}", ex, userID);
            }
            return null;
        }
        
        private bool ContainsUser(string parentText, int userID)
        {
            if (string.IsNullOrEmpty(parentText))
            {
                return true;
            }
            return userID > 0 && parentText.S().IndexOf(userID.S(), StringComparison.Ordinal) > -1;
        }

        protected override void OnBeforeUpdate(ref Notification existing, Notification model)
        {
            if (existing != null && model != null)
            {
                existing.Name = model.Name;
                existing.Content = model.Content;
                existing.Target = model.Target;
                existing.TargetList = model.TargetList;
                existing.DisplayMode = model.DisplayMode;
                existing.PublishDate = model.PublishDate;
                existing.Expire = model.Expire;
                existing.Status = model.Status;
            } 
        }
    }
}