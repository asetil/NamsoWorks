using System;
using Worchart.BL.Constants;
using Worchart.BL.Exceptions;

namespace Worchart.BL.Log
{
    public class WorchartLogger : ILogger
    {
        private readonly NLog.Logger _logger;
        private string FORMAT_ERROR = "{0}\nErrorCode:{1}";
        private string FORMAT_ERROR_DETAILED = "{0}\nErrorCode:{1}\nErrorDetail:{2}";
        private string FORMAT_WARN = "{0}\n{1}";
        private string FORMAT_CRITICAL = "[KRİTİK HATA]|{0}\n{1}";
        
        public WorchartLogger()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void Error(string key, Exception ex)
        {
            var errorCode = ErrorConstants.OperationFailed;
            if (ex != null && ex is WorchartException)
            {
                errorCode = ((WorchartException)ex).Code;
            }

            _logger.Error(ex, FORMAT_ERROR, key, errorCode);
        }

        public void Error(string key, string message, Exception ex, params object[] param)
        {
            var errorCode = ErrorConstants.OperationFailed;
            if (ex != null && ex is WorchartException)
            {
                errorCode = ((WorchartException)ex).Code;
            }

            _logger.Error(ex, FORMAT_ERROR_DETAILED, key, errorCode, message.FormatWith(param));
        }

        public void Info(string key, string message, params object[] param)
        {
            _logger.Info(FORMAT_WARN, key, message.FormatWith(param));
        }

        public void Warn(string key, string message, bool sendMail, params object[] param)
        {
            _logger.Warn(FORMAT_WARN, key, message.FormatWith(param));

            if (sendMail)
            {
                //var mailService = WindsorBootstrapper.Resolve<IMailManager>();
                //mailService.SendWarningMail(title, message);
            }
        }

        public void Critical(string key, string message, bool sendMail, params object[] param)
        {
            _logger.Fatal(FORMAT_CRITICAL, key, message.FormatWith(param));

            if (sendMail)
            {
                //var mailService = WindsorBootstrapper.Resolve<IMailManager>();
                //mailService.SendWarningMail("Kritik Bildirim:" + title, message);
            }
        }
    }
}