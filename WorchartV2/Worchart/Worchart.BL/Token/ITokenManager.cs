using Worchart.BL.Model;
using Worchart.BL.User;

namespace Worchart.BL.Token
{
    public interface ITokenManager
    {
        OperationResult<TokenResponse> GetToken(TokenRequest request);

        bool CheckToken(TokenRequest tokenRequest);

        OperationResult AuthorizeUser(UserModel user);

        OperationResult LogoffUser(string authorizeToken);

        AuthorizedCustomerModel GetUserAuthorization(string userToken);

        OperationResult<TokenResponse> Simulate();
    }
}
