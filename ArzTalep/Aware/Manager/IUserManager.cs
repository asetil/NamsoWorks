using Aware.Model;
using Aware.BL.Model;
using Aware.Util.Enum;

namespace Aware.Manager
{
    public interface IUserManager : IBaseManager<User>
    {
        OperationResult<SessionDataModel> Login(string userName, string password);

        OperationResult<User> Register(User model);

        OperationResult<bool> Logoff(int userID);

        OperationResult<User> ChangePassword(int userID, string currentPassword, string newPassword, string activationData = "");

        OperationResult<ActivationResultType> ActivateUser(string data);

        OperationResult<ActivationResultType> SendNewActivation(string email, bool forgotPassword = false);

        OperationResult<User> CheckActivationData(string data);
    }
}
