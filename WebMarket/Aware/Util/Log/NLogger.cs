using System;
using Aware.Dependency;
using Aware.ECommerce.Enums;
using Aware.Mail;

namespace Aware.Util.Log
{
    public class NLogger : ILogger
    {
        private NLog.Logger _logger;
        public void Error(string message, Exception ex)
        {
            if (LogMode!= LogMode.Disabled)
            {
                if (LogMode == LogMode.Detailed)
                {
                    Instance.Error("{0} -- {1}", message, ex.ToString());
                }
                else
                {
                    Instance.Error("{0} -- {1}", message, ex != null ? ex.Message : string.Empty);
                }
                Instance.Error("------------------------------------------------------------------------\n");
            }
        }

        public void Error(string format, Exception ex, params object[] param)
        {
            Error(string.Format(format, param), ex);
        }

        public void Info(string message, params object[] param)
        {
            if (LogMode != LogMode.Disabled)
            {
                Instance.Info(message, param);
                Instance.Info("-------------------------------------------------------------------------------\n");
            }
        }

        public void Warn(string message, string title, bool sendMail = false)
        {
            if (LogMode != LogMode.Disabled)
            {
                if (!string.IsNullOrEmpty(title))
                {
                    Instance.Warn("------------------------------[ " + title + " ]--------------------------------");
                }

                Instance.Warn(message);
                Instance.Warn("-----------------------------------------------------------------------------\n");

                if (sendMail)
                {
                    var mailService = WindsorBootstrapper.Resolve<IMailService>();
                    mailService.SendWarningMail(title, message);
                }
            }
        }

        public void Critical(string title, string message, bool sendMail, params object[] param)
        {
            if (LogMode != LogMode.Disabled)
            {
                Instance.Fatal("-------------------------------> KRİTİK HATA :" + title + " <---------------------------------");
                Instance.Fatal(message, param);
                Instance.Fatal("-----------------------------------------------------------------------------\n");

                if (sendMail)
                {
                    var mailService = WindsorBootstrapper.Resolve<IMailService>();
                    mailService.SendWarningMail("Kritik Bildirim:" + title, message);
                }
            }
        }

        private NLog.Logger Instance
        {
            get
            {
                if (_logger == null)
                {
                    _logger = NLog.LogManager.GetCurrentClassLogger();
                }
                return _logger;
            }
        }

        private LogMode LogMode
        {
            get
            {
                if (Instance != null)
                {
                    return Config.LogMode;
                }
                return LogMode.Disabled;
            }
        }
    }
}