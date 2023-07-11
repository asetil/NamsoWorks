using System.Collections.Generic;
using Aware.Authenticate.Model;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace Aware.Authenticate
{
    public interface IUserService
    {
        User GetUser(int userID);
        User GetUserByEmail(string email);
        User GetAdminUser(int userID);
        List<User> GetCustomerUsers(int customerID);
        List<User> GetUsersByID(List<int> idList);

        Result IsAuthorized(User model);
        Result Register(User user, bool isSocialUser = false);
        Result SaveUser(User model);
        Result ActivateUser(string data);
        Result DeleteUser(int userID, int currentUserID);

        Result CheckActivationData(string data);
        Result ChangePassword(int currentUserID, string currentPassword, string password,string data="");
        Result SendAuthenticationMail(string email, AuthenticationMailType mailType);

        Result GetAuthorizeTicket(string email, string password, int i);
        Result IsTicketAuthorized(string ticket);
    }
}
