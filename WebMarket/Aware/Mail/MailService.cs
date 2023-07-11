using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;

namespace Aware.Mail
{
    public class MailService : BaseService<MailTemplate>, IMailService
    {
        private readonly IApplication _application;
        private readonly IMailManager _mailManager;

        public MailService(IApplication application, IRepository<MailTemplate> mailRepository, IMailManager mailManager, ILogger logger):base(mailRepository,logger)
        {
            _application = application;
            _mailManager = mailManager;
        }
        
        public bool SendWelcomeMail(string toAdress, string userName, string activationLink)
        {
            return SendEmail(Constants.WelcomeMailTemplate, toAdress, string.Empty, userName, activationLink);
        }

        public bool SendForgotPasswordMail(string toAdress, string userName, string activationLink)
        {
            return SendEmail(Constants.ForgotPasswordMail, toAdress, string.Empty, new[] { userName, toAdress, activationLink });
        }

        public bool NotifyPasswordChanged(string toAdress, string userName)
        {
            return SendEmail(Constants.PasswordChangedMail, toAdress, string.Empty, new[] { userName });
        }

        public bool SendActivationMail(string toAdress, string userName, string activationLink)
        {
            return SendEmail(Constants.ActivationMail, toAdress, string.Empty, new[] { userName, activationLink });
        }

        public bool SendCouponMail(string toAdress, string userName, string description, string expireDate, string couponCode)
        {
            return SendEmail(Constants.CouponMail, toAdress, string.Empty, new[] { userName, description, expireDate, couponCode });
        }

        public bool SendWarningMail(string title, string message)
        {
            return SendEmail(Constants.WarningMail, _application.Site.MailUser, title, message);
        }

        public Result SendContactUsMail(ContactModel model)
        {
            try
            {
                if (model != null)
                {
                    var toAdress = _application.Site.MailUser;
                    var paramList = new[] { model.Name, model.Email, model.Subject, model.Message, DateTime.Now.ToString(), model.UserID.ToString() };

                    SendEmail(Constants.ContactUsMail, toAdress, string.Empty, paramList);
                    Logger.Info(string.Format("MailService > SendContactUsMail - Fail for name:{0},email:{1},subject:{2}, message:{3}", model.Name, model.Email, model.Subject, model.Message));
                    return Result.Success(null, Resource.User_ContactUsMailSended);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MailService > SendContactUsMail - Fail for name:{0},email:{1},subject:{2}", ex, model.Name, model.Email, model.Subject);
            }
            return Result.Error(Resource.General_Error);
        }

        public bool SendOrderMail(string toAdress, string subject, string html)
        {
            return SendEmail(Constants.OrderMail, toAdress, subject, new[] { subject, html });
        }

        private bool SendEmail(string templateName, string toAdress, string subject = "", params object[] paramList)
        {
            try
            {
                var template = GetMailTemplate(templateName);
                if (template != null && template.ID > 0)
                {
                    subject = string.Format(template.Subject, subject);
                    var mailContent = string.Empty;
                    if (template.Parent != null)
                    {
                        mailContent = string.Format(template.Parent.Content, template.Content);
                    }

                    Thread email = new Thread(delegate()
                    {
                        var success = _mailManager.SendEmail(toAdress, subject, mailContent, paramList);
                        if (!success) { Logger.Error(string.Format("Error while sending email to : {0} with subject : {1} and templateName : {2}", toAdress, subject, templateName), null); }
                    });

                    email.IsBackground = true;
                    email.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error while sending email to : {0} with subject : {1} and templateName : {2}", toAdress, subject, templateName), ex);
                return false;
            }
            return true;
        }

        private MailTemplate GetMailTemplate(string templateName)
        {
            var mailTemplates = _application.Cacher.Get<IEnumerable<MailTemplate>>(Constants.CK_MailTemplates);
            if (mailTemplates == null || !mailTemplates.Any())
            {
                mailTemplates = Repository.Where(i => i.ID > 0).ToList();
                _application.Cacher.Add(Constants.CK_MailTemplates, mailTemplates);
            }

            var mailTemplate = mailTemplates.FirstOrDefault(i => i.Name == templateName);
            if (mailTemplate != null && mailTemplate.ParentID > 0)
            {
                mailTemplate.Parent = mailTemplates.FirstOrDefault(i => i.ID == mailTemplate.ParentID);
            }
            return mailTemplate;
        }

        protected override void OnBeforeUpdate(ref MailTemplate existing, MailTemplate model)
        {
            if (existing != null && model != null)
            {
                existing.ParentID = model.ParentID;
                existing.Content = model.Content;
                existing.Description = model.Description;
                existing.Subject = model.Subject;
            }
        }
    }
}
