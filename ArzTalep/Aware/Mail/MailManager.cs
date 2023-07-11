using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Aware.BL.Model;
using Aware.Data;
using Aware.Manager;
using Aware.Util;
using Aware.Util.Cache;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aware.Mail
{
    public class MailManager : BaseManager<MailTemplate>, IMailManager
    {
        private readonly IConfiguration _configuration;
        private readonly Queue<MailMessage> _mailQueue;
        private readonly IAwareLogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private Timer _timer;

        public MailManager(IRepository<MailTemplate> repository, IAwareLogger logger, IAwareCacher cacher, IServiceProvider serviceProvider, IConfiguration configuration) : base(repository, logger, cacher)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _mailQueue = new Queue<MailMessage>();
            _timer = new Timer(OnTimerTick, null, CommonConstants.MailQueueTimerInterval, CommonConstants.MailQueueTimerInterval);
        }

        public bool SendWithTemplate(string templateName, string toAdress, string subject = "", params object[] paramList)
        {
            try
            {
                var mailTemplate = First(i => i.Name == templateName);
                if (mailTemplate != null && mailTemplate.ParentID > 0)
                {
                    mailTemplate.Parent = First(i => i.ID == mailTemplate.ParentID);
                }

                if (mailTemplate != null && mailTemplate.ID > 0)
                {
                    subject = string.Format(mailTemplate.Subject, subject);
                    var mailContent = string.Empty;
                    if (mailTemplate.Parent != null)
                    {
                        mailContent = string.Format(mailTemplate.Parent.Content, mailTemplate.Content);
                    }

                    Send(toAdress, subject, mailContent, paramList);

                    //Thread email = new Thread(delegate ()
                    //{
                    //    var success = _mailManager.SendEmail(toAdress, subject, mailContent, paramList);
                    //    if (!success) { Logger.Error(string.Format("Error while sending email to : {0} with subject : {1} and templateName : {2}", toAdress, subject, templateName), null); }
                    //});

                    //email.IsBackground = true;
                    //email.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MailManager|SendWithTemplate", "To:{0}, Subject:{1}, templateName:{2}", ex, toAdress, subject, templateName);
                return false;
            }
            return true;
        }

        protected override ManagerCacheMode CacheMode => ManagerCacheMode.UseResponsiveCache;

        protected override OperationResult<MailTemplate> OnBeforeUpdate(ref MailTemplate existing, MailTemplate model)
        {
            if (existing != null && model != null)
            {
                existing.ParentID = model.ParentID;
                existing.Content = model.Content;
                existing.Description = model.Description;
                existing.Subject = model.Subject;
                return Success();
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }

        public bool Send(MailMessage mail)
        {
            try
            {
                if (_timer == null)
                {
                    _timer = new Timer(OnTimerTick, null, CommonConstants.MailQueueTimerInterval, CommonConstants.MailQueueTimerInterval);
                }

                _mailQueue.Enqueue(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        public bool Send(string to, string subject, string body)
        {
            try
            {
                var mail = new MailMessage();
                string email = "arztalep@outlook.com.tr";
                mail.From = new MailAddress(email, "ArzTalep");
                mail.To.Add(new MailAddress(to));
                //mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                return Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Send(string toAdress, string subject, string mailBody, params object[] paramList)
        {
            try
            {
                if (paramList != null && paramList.Length > 0)
                {
                    mailBody = string.Format(mailBody, paramList);
                }

                mailBody = mailBody.Replace("###", _configuration.GetValue("DomainUrl"));
                return Send(toAdress, subject, mailBody);
            }
            catch (Exception ex)
            {
                _logger.Error("MailManager > SendEmail2 - failed", ex);
            }
            return false;
        }

        private void OnTimerTick(object input)
        {
            try
            {
                if (_mailQueue != null && _mailQueue.Any())
                {
                    var applicationManager = _serviceProvider.GetService<IApplicationManager>();
                    var application = applicationManager.Get(_configuration.GetInt("ApplicationID"));
                    if (application == null)
                    {
                        return;
                    }

                    _timer.Change(-1, -1);

                    var smtpClient = new SmtpClient(application.SmtpServer, application.SmtpPort)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false, // true if you don't want to use the network credentials
                        Credentials = new NetworkCredential(application.SmtpUsername, application.SmtpPassword),
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };

                    smtpClient.SendCompleted += OnAfterSendMail;
                    while (_mailQueue.Count > 0)
                    {
                        var mail = _mailQueue.Dequeue();
                        smtpClient.Send(mail);
                        //_logger.Info("Mailer > Mail Sended : {0}-{1}", mail.To, mail.Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("MailManager > OnTimerTick - Mail Send Failed", ex);
            }
            finally
            {
                _timer.Change(CommonConstants.MailQueueTimerInterval, CommonConstants.MailQueueTimerInterval);
            }
        }

        protected virtual void OnAfterSendMail(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }
    }
}