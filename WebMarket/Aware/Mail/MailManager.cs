using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Aware.ECommerce;
using Aware.ECommerce.Manager;
using Aware.Util;
using Aware.Util.Log;

namespace Aware.Mail
{
    public class MailManager : IMailManager
    {
        private const int Interval = 30000; //30sn'de 1
        private readonly Queue<MailMessage> _mailQueue;
        private readonly IApplication _application;
        private readonly ILogger _logger;
        private Timer _timer;

        public MailManager(IApplication application, ILogger logger)
        {
            _application = application;
            _logger = logger;
            _mailQueue = new Queue<MailMessage>();
            _timer = new Timer(OnTimerTick, null, Interval, Interval);
        }

        public bool SendEmail(MailMessage mail)
        {
            try
            {
                if (_timer == null)
                {
                    _timer = new Timer(OnTimerTick, null, Interval, Interval);
                }

                _logger.Info("MailService > Mail Queued : {0}-{1}", mail.To, mail.Subject);
                _mailQueue.Enqueue(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        public bool SendEmail(string address, string subject, string body)
        {
            try
            {
                var mail = new MailMessage();
                string email = _application.Site.MailUser;
                mail.From = new MailAddress(email);
                mail.To.Add(new MailAddress(address));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                return SendEmail(mail);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendEmail(string toAdress, string subject, string templatePath, string mailLayout, params object[] paramList)
        {
            try
            {
                var mailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/" + templatePath));
                if (paramList != null && paramList.Length > 0)
                {
                    mailBody = string.Format(mailBody, paramList);
                }

                var mailContent = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/" + mailLayout));
                mailContent = string.Format(mailContent, mailBody);
                mailContent = mailContent.Replace("###", Config.DomainUrl);
                return SendEmail(toAdress, subject, mailContent);
            }
            catch (Exception ex)
            {
                _logger.Error("MailManager > SendEmail - failed", ex);
            }
            return false;
        }

        public bool SendEmail(string toAdress, string subject, string mailBody, params object[] paramList)
        {
            try
            {
                if (paramList != null && paramList.Length > 0)
                {
                    mailBody = string.Format(mailBody, paramList);
                }

                mailBody = mailBody.Replace("###", Config.DomainUrl);
                return SendEmail(toAdress, subject, mailBody);
            }
            catch (Exception ex)
            {
                _logger.Error("MailManager > SendEmail2 - failed", ex);
            }
            return false;
        }

        protected virtual void OnAfterSendMail(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        private void OnTimerTick(object input)
        {
            try
            {
                if (_mailQueue != null && _mailQueue.Any())
                {
                    _timer.Change(-1, -1);
                    var smtpClient = new SmtpClient(_application.Site.MailHost, _application.Site.MailPort.Int())
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_application.Site.MailUser, _application.Site.MailPassword)
                    };

                    smtpClient.SendCompleted += OnAfterSendMail;
                    while (_mailQueue.Count > 0)
                    {
                        var mail = _mailQueue.Dequeue();
                        smtpClient.Send(mail);
                        _logger.Info("Mailer > Mail Sended : {0}-{1}", mail.To, mail.Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("MailManager > OnTimerTick - Mail Send Failed", ex);
            }
            _timer.Change(Interval, Interval);
        }
    }
}