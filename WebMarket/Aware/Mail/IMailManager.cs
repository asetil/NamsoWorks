using System.Net.Mail;

namespace Aware.Mail
{
    public interface IMailManager
    {
        bool SendEmail(MailMessage mail);
        bool SendEmail(string address, string subject, string body);
        bool SendEmail(string toAdress, string subject, string templatePath, string mailLayout, params object[] paramList);
        bool SendEmail(string toAdress, string subject, string mailBody, params object[] paramList);
    }
}