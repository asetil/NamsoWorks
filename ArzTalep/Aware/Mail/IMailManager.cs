using Aware.Manager;
using System.Net.Mail;

namespace Aware.Mail
{
    public interface IMailManager : IBaseManager<MailTemplate>
    {
        bool Send(MailMessage mail);
        bool Send(string address, string subject, string body);
        bool Send(string toAdress, string subject, string mailBody, params object[] paramList);
        bool SendWithTemplate(string templateName, string toAdress, string subject = "", params object[] paramList);
    }
}