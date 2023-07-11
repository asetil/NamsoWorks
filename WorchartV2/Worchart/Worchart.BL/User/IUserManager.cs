using Worchart.BL.Manager;
using Worchart.BL.Model;

namespace Worchart.BL.User
{
    public interface IUserManager : IBaseManager<UserModel>
    {
        OperationResult Login(LoginRequestModel requestModel);

        OperationResult Register(RegisterModel model);

        OperationResult Logoff(string authorizeToken);

        OperationResult ChangePassord(string authorizeToken, string newPassword);
    }
}
