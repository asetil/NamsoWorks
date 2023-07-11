using Aware.Data;
using Aware.Model;
using Aware.Util;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Exceptions;
using Aware.Util.Log;
using System;
using Aware.BL.Model;
using Aware.Mail;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Aware.Manager
{
    public class UserManager : BaseManager<User>, IUserManager
    {
        private readonly IEncryptManager _encryptManager;
        private readonly ISessionManager _sessionManager;
        private readonly IMailManager _mailManager;
        private readonly IConfiguration _configuration;

        public UserManager(ISessionManager sessionManager, IEncryptManager encryptManager, IMailManager mailManager,
            IRepository<User> repository, IAwareLogger logger, IConfiguration configuration) : base(repository, logger)
        {
            _sessionManager = sessionManager;
            _encryptManager = encryptManager;
            _mailManager = mailManager;
            _configuration = configuration;
        }

        public OperationResult<SessionDataModel> Login(string userName, string password)
        {
            try
            {
                if (!userName.Valid())
                {
                    return Failed<SessionDataModel>(ResultCodes.Error.Login.InvalidUsernamePassword);
                }

                var user = First(i => i.Email == userName.Trim() || i.PhoneNumber == userName.Trim());
                if (user == null || user.Status != StatusType.Active)
                {
                    return Failed<SessionDataModel>(ResultCodes.Error.Login.UserNotFound);
                }
                else if (user.Password != _encryptManager.Encrypt(password))
                {
                    return Failed<SessionDataModel>(ResultCodes.Error.Login.PasswordDismatch);
                }

                //if (requestModel.CompanyCode.Valid()) //Check company
                //{
                //    var company = _companyManager.First(i => i.ID == user.CompanyID && i.CompanyCode == requestModel.CompanyCode.Trim());
                //    if (company == null || company.Status != Enum.StatusType.Active)
                //    {
                //        return Failed(ErrorConstants.Login.CompanyNotFound);
                //    }
                //}
                //else if (user.CompanyID > 0)
                //{
                //    return Failed(ErrorConstants.Login.CompanyLoginNotAllowed);
                //}

                var sessionData = new SessionDataModel()
                {
                    SessionKey = user.ID.ToString(),
                    Name = user.Name,
                    Role = user.Role
                };

                var success = _sessionManager.Open(sessionData);
                if (success)
                {
                    user.LastVisit = DateTime.Now;
                    Save(user);
                    return Success(sessionData);
                }
                return Failed<SessionDataModel>(ResultCodes.Error.Login.AuthorizationFailed);
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Login", "companyCode:{0}, userName:{1}, passw:{2}", ex, "", userName, password);
            }
            return Failed<SessionDataModel>(ResultCodes.Error.Login.InvalidUsernamePassword);
        }

        public OperationResult<bool> Logoff(int userID)
        {
            try
            {
                if (userID > 0)
                {
                    var success = _sessionManager.Terminate(userID.ToString());
                    if (success)
                    {
                        return OperationResult<bool>.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Logoff", "userID:{0}", ex, userID);
            }
            return OperationResult<bool>.Error(ResultCodes.Error.Login.LogoffFailed);
        }

        public OperationResult<User> Register(User model)
        {
            try
            {
                if (model != null && model.IsValid())
                {
                    var result = Save(model);
                    if (result.Ok)
                    {
                        SendActivation(result.Value);
                    }
                    return Success();
                }
                return Failed(ResultCodes.Error.CheckParameters);
            }
            catch (AwareException wex)
            {
                Logger.Error("UserManager|Register", wex);
                return Failed(wex.Code);
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|Register", ex);
            }
            return Failed(ResultCodes.Error.OperationFailed);
        }

        public OperationResult<User> ChangePassword(int userID, string currentPassword, string newPassword, string activationData = "")
        {
            var result = Failed(ResultCodes.Error.CheckParameters);
            try
            {
                if (!string.IsNullOrEmpty(activationData))
                {
                    result = CheckActivationData(activationData);
                    if (result.Ok)
                    {
                        userID = result.Value.ID;
                        currentPassword = _encryptManager.Decrypt(result.Value.Password);
                    }
                }

                if (userID > 0 && !string.IsNullOrEmpty(newPassword))
                {
                    var user = Get(userID);
                    if (user != null && user.ID > 0)
                    {
                        if (user.Password != _encryptManager.Encrypt(currentPassword))
                        {
                            return Failed(ResultCodes.Error.Login.InvalidCurrentPassword);
                        }

                        if (currentPassword == newPassword)
                        {
                            return Failed(ResultCodes.Error.Login.CurrentPasswordAndNewPasswordAreSame);
                        }

                        user.Password = _encryptManager.Encrypt(newPassword);
                        user.DateModified = DateTime.Now;
                        user.Status = user.Status == StatusType.WaitingActivation ? StatusType.Active : user.Status;
                        Save(user);

                        _mailManager.SendWithTemplate(MailTemplates.NotifyPasswordChanged, user.Email, string.Empty, user.Name);
                        return Success(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|ChangePassword", "userID:{0}, currentPassword : {1}, newPassword:{2}", ex, userID, currentPassword, newPassword);
                result = Failed(ResultCodes.Error.OperationFailed);
            }
            return result;
        }

        public OperationResult<ActivationResultType> ActivateUser(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    return Failed<ActivationResultType>(ResultCodes.Error.Login.InvalidActivationCode);
                }

                var result = CheckActivationData(data);
                if (result.Ok)
                {
                    var user = result.Value;
                    if (user.Status == StatusType.WaitingActivation)
                    {
                        user.Status = StatusType.Active;
                        Save(user);
                        return Success(ActivationResultType.ActivationSuccessfull);
                    }
                    return Failed<ActivationResultType>(ResultCodes.Error.Login.AlreadyActivated);
                }
                return Failed<ActivationResultType>(result.Code);
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|ActivateUser", "data:{0}", ex, data);
            }
            return Failed<ActivationResultType>(ResultCodes.Error.CheckParameters);
        }

        public OperationResult<ActivationResultType> SendNewActivation(string emailOrPhoneNumber, bool forgotPassword = false)
        {
            try
            {
                if (emailOrPhoneNumber.Valid())
                {
                    emailOrPhoneNumber = emailOrPhoneNumber.Trim();
                    var user = First(i => i.Email == emailOrPhoneNumber || i.PhoneNumber == emailOrPhoneNumber);
                    if (user == null || user.ID == 0)
                    {
                        return Failed<ActivationResultType>(ResultCodes.Error.Login.UserNotFound);
                    }

                    if (!forgotPassword && user.Status != StatusType.WaitingActivation)
                    {
                        return Failed<ActivationResultType>(ResultCodes.Error.Login.AlreadyActivated);
                    }

                    var linkPage = forgotPassword ? _configuration.GetValue("PasswordChangeUrl", "Account/ChangePassword")
                        : _configuration.GetValue("AccountActivationUrl", "Account/Activation");
                    var activationLink = GetActivationLink(user, linkPage);

                    var success = false;
                    if (!forgotPassword)
                    {
                        success = _mailManager.SendWithTemplate(MailTemplates.ActivationMail, user.Email, string.Empty, user.Name, activationLink);
                    }
                    else
                    {
                        success = _mailManager.SendWithTemplate(MailTemplates.ForgotPasswordMail, user.Email, string.Empty, user.Name, user.Email, activationLink);
                    }
                    return success ? Success(ActivationResultType.ActivationSend) : Failed<ActivationResultType>(ResultCodes.Error.Login.ActivationMailSendFailure);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserService|SendAuthenticationMail", "emailOrPhoneNumber:{0}, forgotPassword:{1}", ex, emailOrPhoneNumber, forgotPassword);
            }
            return Failed<ActivationResultType>(ResultCodes.Error.OperationFailed);
        }

        protected override OperationResult<User> OnBeforeCreate(ref User model)
        {
            if (model != null && model.IsValid())
            {
                if (IsEmailInUse(model.ID, model.Email))
                {
                    return Failed(ResultCodes.Error.Login.EmailInUse);
                }

                if (IsPhoneNumberInUse(model.ID, model.PhoneNumber))
                {
                    return Failed(ResultCodes.Error.Login.PhoneNumberInUse);
                }

                model.Password = _encryptManager.Encrypt(model.Password);
                model.Status = StatusType.WaitingActivation;
                return Success(model);
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }

        protected override OperationResult<User> OnBeforeUpdate(ref User existing, User model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                if (model.LastVisit > DateTime.MinValue)
                {
                    existing.LastVisit = model.LastVisit;
                    return OperationResult<User>.Success(null);
                }

                if (existing.Email != model.Email && IsEmailInUse(model.ID, model.Email))
                {
                    return Failed(ResultCodes.Error.Login.EmailInUse);
                }

                if (existing.PhoneNumber != model.PhoneNumber && IsPhoneNumberInUse(model.ID, model.PhoneNumber))
                {
                    return Failed(ResultCodes.Error.Login.PhoneNumberInUse);
                }

                existing.Name = model.Name;
                existing.Email = model.Email;
                existing.Status = model.Status;
                return Success(existing);
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }

        public OperationResult<User> CheckActivationData(string data)
        {
            try
            {
                var dataValues = _encryptManager.Decrypt(data).Split('|');
                var activationDate = DateTime.ParseExact(dataValues[0], CommonConstants.TimeStampFormat, CultureInfo.InvariantCulture);

                var passedTime = DateTime.Now.Subtract(activationDate).TotalSeconds;
                if (passedTime > CommonConstants.ActivationCodeExpire)
                {
                    return Failed(ResultCodes.Error.Login.ActivationLinkExpired);
                }

                var userID = dataValues[1].ToInt();
                var email = dataValues[2];
                var user = First(i => i.ID == userID && i.Email == email);
                if (user != null)
                {
                    return Success(user);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|DecryptActivationData", "data : {0}", ex, data);
            }
            return Failed(ResultCodes.Error.Login.InvalidActivationCode);
        }

        #region Helper

        private bool IsEmailInUse(int userID, string email)
        {
            if (email.Valid())
            {
                var emailExists = First(x => x.ID != userID && x.Email == email && x.Status == StatusType.Active);
                return emailExists != null;
            }
            return false;
        }

        private bool IsPhoneNumberInUse(int userID, string phoneNumber)
        {
            if (phoneNumber.Valid())
            {
                var phoneNumberExists = First(x => x.ID != userID && x.PhoneNumber == phoneNumber && x.Status == StatusType.Active);
                return phoneNumberExists != null;
            }
            return false;
        }

        private void SendActivation(User user)
        {
            try
            {
                //mail yada sms dogrulama kodu gonderilir
                var linkPage = _configuration.GetValue("AccountActivationUrl", "Account/Activation");
                var activationLink = GetActivationLink(user, linkPage);
                _mailManager.Send(user.Email, "Üyelik Aktivasyonu", $"<a href='{activationLink}' target='_blank'>Üyeliğimi Aktifleştir</a>");
            }
            catch (Exception ex)
            {
                Logger.Error("UserManager|SendActivation", ex);
            }
        }

        private string GetActivationLink(User user, string linkPage)
        {
            if (user == null || user.ID == 0)
            {
                throw new Exception("Error while creating activation data!");
            }

            var activationData = string.Format("{0}|{1}|{2}", DateTime.Now.ToString(CommonConstants.TimeStampFormat), user.ID, user.Email);
            activationData = System.Web.HttpUtility.UrlEncode(_encryptManager.Encrypt(activationData));
            return string.Format("{0}/{1}?data={2}", _configuration.GetValue("DomainUrl"), linkPage, activationData);
        }

        #endregion
    }
}
