using System;
using Worchart.BL.Company;
using Worchart.BL.Constants;
using Worchart.BL.Encryption;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.BL.Token;
using Worchart.Data;

namespace Worchart.BL.User
{
    public class UserManager : BaseManager<UserModel>, IUserManager
    {
        private readonly ITokenManager _sessionManager;
        private readonly ICompanyManager _companyManager;
        private readonly IEncryptManager _encryptManager;

        public UserManager(ITokenManager sessionManager, ICompanyManager companyManager, IEncryptManager encryptManager, IRepository<UserModel> repository, ILogger logger) : base(repository, logger)
        {
            _sessionManager = sessionManager;
            _companyManager = companyManager;
            _encryptManager = encryptManager;
        }

        public OperationResult Login(LoginRequestModel requestModel)
        {
            try
            {
                if (!requestModel.IsValid())
                {
                    return Failed(ErrorConstants.Login.InvalidUsernamePassword);
                }

                var user = First(i => i.Email == requestModel.Email.Trim());
                if (user == null || user.Status != Enum.StatusType.Active)
                {
                    return Failed(ErrorConstants.Login.UserNotFound);
                }
                else if (user.Password != _encryptManager.Encrypt(requestModel.Password))
                {
                    return Failed(ErrorConstants.Login.PasswordDismatch);
                }

                if (requestModel.CompanyCode.Valid()) //Check company
                {
                    var company = _companyManager.First(i => i.ID == user.CompanyID && i.CompanyCode == requestModel.CompanyCode.Trim());
                    if (company == null || company.Status != Enum.StatusType.Active)
                    {
                        return Failed(ErrorConstants.Login.CompanyNotFound);
                    }
                }
                else if (user.CompanyID > 0)
                {
                    return Failed(ErrorConstants.Login.CompanyLoginNotAllowed);
                }

                var authorizeResult = _sessionManager.AuthorizeUser(user);
                if (authorizeResult.Success)
                {
                    user.LastVisit = DateTime.Now;
                    Save(user);
                }

                return authorizeResult;
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Login", "companyCode:{0}, email:{1}, passw:{2}", ex, requestModel.CompanyCode, requestModel.Email, requestModel.Password);
            }
            return Failed();
        }

        public OperationResult Logoff(string authorizeToken)
        {
            try
            {
                if (authorizeToken.Valid())
                {
                    return _sessionManager.LogoffUser(authorizeToken);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Logoff", "authorizeToken:{0}", ex, authorizeToken);
            }
            return Failed();
        }

        public OperationResult Register(RegisterModel model)
        {
            try
            {
                if (model != null && model.IsValid())
                {
                    var userModel = new UserModel()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = _encryptManager.Encrypt(model.Password),
                        Status = Enum.StatusType.Active
                    };

                    var result = Save(userModel);
                    result.Value = null;
                    return result;
                }
                return Failed(ErrorConstants.CheckParameters);
            }
            catch (Exceptions.WorchartException wex)
            {
                Logger.Error("UserManager|Register", wex);
                return Failed(wex.Code);
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Register", ex);
            }
            return Failed(ErrorConstants.OperationFailed);
        }

        public OperationResult ChangePassord(string authorizeToken, string newPassword)
        {
            try
            {
                if (authorizeToken.Valid() && newPassword.Valid())
                {
                    //var userInfo = _tokenManager.GetAuthorization(authorizeToken);
                    //if(userInfo!=null && userInfo.ID > 0)
                    //{
                    //    var user = Get(userInfo.ID);
                    //    if (user != null)
                    //    {
                    //        user.Password = _encryptManager.Encrypt(newPassword);
                    //        return Save(user);
                    //    }
                    //}
                }
                else
                {
                    return Failed(ErrorConstants.CheckParameters);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|ChangePassord", "authorizeToken:{0} and newPassword:{1}", ex, authorizeToken, newPassword);
            }
            return Failed();
        }

        protected override OperationResult OnBeforeCreate(ref UserModel model)
        {
            if (model != null && model.IsValid())
            {
                if (IsEmailInUse(model.ID, model.Email))
                {
                    return Failed(ErrorConstants.Login.EmailInUse);
                }

                model.DateCreated = DateTime.Now;
                model.DateModified = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override OperationResult OnBeforeUpdate(ref UserModel existing, UserModel model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                if (model.LastVisit > DateTime.MinValue)
                {
                    existing.LastVisit = model.LastVisit;
                    return Success();
                }

                if (existing.Email != model.Email && IsEmailInUse(model.ID, model.Email))
                {
                    return Failed(ErrorConstants.Login.EmailInUse);
                }

                existing.Name = model.Name;
                existing.Email = model.Email;
                existing.Status = model.Status;
                existing.DateModified = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        private bool IsEmailInUse(int userID, string email)
        {
            if (email.Valid())
            {
                var emailExists = First(x => x.ID != userID && x.Email == email && x.Status == Enum.StatusType.Active);
                return emailExists != null;
            }
            return false;
        }
    }
}
