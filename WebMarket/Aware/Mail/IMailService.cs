using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Model;

namespace Aware.Mail
{
    public interface IMailService:IBaseService<MailTemplate>
    {
        bool SendWelcomeMail(string toAdress,string userName,string activationLink);
        bool SendForgotPasswordMail(string toAdress, string userName, string activationLink);
        bool NotifyPasswordChanged(string toAdress,string userName);
        bool SendActivationMail(string toAdress, string userName, string activationLink);
        bool SendCouponMail(string toAdress, string userName,string description,string expireDate,string couponCode);
        bool SendWarningMail(string title,string message);
        Result SendContactUsMail(ContactModel model);
        bool SendOrderMail(string toAdress, string subject, string html);
    }
}