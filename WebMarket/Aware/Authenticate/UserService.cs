using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.Data;
using Aware.ECommerce;
using Aware.ECommerce.Util;
using Aware.Mail;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Authenticate
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMailService _mailService;
        private readonly IApplication _application;

        public UserService(IMailService mailService, IRepository<User> userRepository, IApplication application)
        {
            _mailService = mailService;
            _userRepository = userRepository;
            _application = application;
        }

        public User GetUser(int userID)
        {
            if (userID > 0)
            {
                return _userRepository.Where(i => i.ID == userID && i.Status != Statuses.Passive).First();
            }
            return null;
        }

        public User GetAdminUser(int userID)
        {
            if (userID > 0)
            {
                return _userRepository.First(i => i.ID == userID && i.Role != UserRole.User);
            }
            return null;
        }

        public User GetUserByEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return _userRepository.First(i => i.Email == email && i.Status != Statuses.Passive);
            }
            return null;
        }

        public List<User> GetUsersByID(List<int> idList)
        {
            if (idList != null && idList.Any())
            {
                return _userRepository.Where(i => idList.Contains(i.ID)).ToList();
            }
            return new List<User>();
        }

        public List<User> GetCustomerUsers(int customerID)
        {
            if (customerID > 0)
            {
                return _userRepository.Where(i => i.CustomerID == customerID && i.Role == UserRole.AdminUser).ToList();
            }
            if (customerID == 0)
            {
                return _userRepository.Where(i => i.Role == UserRole.SuperUser || i.Role == UserRole.ServiceUser).ToList();
            }
            return null;
        }

        public Result IsAuthorized(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                User persistedUser;
                if (Config.IsAdmin)
                {
                    persistedUser = _userRepository.First(i => i.Email == user.Email && i.Status != Statuses.Deleted && (i.Role == UserRole.SuperUser || i.Role == UserRole.AdminUser));
                }
                else
                {
                    persistedUser = _userRepository.Where(i => i.Email == user.Email && i.Status != Statuses.Passive).First();
                }

                var isAuthorized = persistedUser != null && persistedUser.ID > 0 && (persistedUser.Password == user.Password || persistedUser.Password == Encryptor.Encrypt(user.Password));
                if (isAuthorized)
                {
                    if (persistedUser.Status == Statuses.WaitingActivation)
                    {
                        return Result.Error((int)Statuses.WaitingActivation, Resource.User_ActivationRequired);
                    }

                    if (persistedUser.Status == Statuses.Passive)
                    {
                        return Result.Error((int)Statuses.Passive, Resource.User_UnAuthorized);
                    }
                    return Result.Success(persistedUser);
                }
            }
            return Result.Error(Resource.User_WrongInformation);
        }

        public Result GetAuthorizeTicket(string email, string password, int duration)
        {
            var persistedUser = _userRepository.Where(i => i.Email == email && i.Status == Statuses.Active && (i.Role == UserRole.SuperUser || i.Role == UserRole.ServiceUser)).First();
            if (persistedUser == null || persistedUser.ID == 0 || persistedUser.Password != password)
            {
                return Result.Error(Resource.User_WrongInformation);
            }

            var ticketList = _application.Cacher.Get<Dictionary<Guid, Ticket>>(Constants.CK_TicketCache) ?? new Dictionary<Guid, Ticket>();
            var ticketItem = ticketList.FirstOrDefault(i => i.Value.ID == persistedUser.ID && i.Value.Value == persistedUser.Email);


            var ticket = !ticketItem.Equals(default(KeyValuePair<Guid, Ticket>)) ? ticketItem.Value : new Ticket()
            {
                ID = persistedUser.ID,
                Value = persistedUser.Email,
                Type = TicketType.Authorize,
            };
            ticket.ExpireDate = DateTime.Now.AddHours(duration);

            var guid = Guid.Empty;
            if (!ticketItem.Equals(default(KeyValuePair<Guid, Ticket>)))
            {
                guid = ticketItem.Value.ExpireDate > DateTime.Now ? ticketItem.Key : guid;
                ticketList.Remove(ticketItem.Key);
            }
            else
            {
                guid = Guid.NewGuid();
            }

            if (guid == Guid.Empty)
            {
                return Result.Error(Resource.General_Error);
            }

            ticketList.Add(guid, ticket);
            _application.Cacher.Add(Constants.CK_TicketCache, ticketList);
            return Result.Success(guid, Resource.General_Success);
        }

        public Result IsTicketAuthorized(string ticketKey)
        {
            try
            {
                var guid = Guid.Parse(ticketKey);
                var ticketList = _application.Cacher.Get<Dictionary<Guid, Ticket>>(Constants.CK_TicketCache);
                if (ticketList != null && ticketList.ContainsKey(guid))
                {
                    var ticket = ticketList[guid];
                    if (ticket != null && ticket.ExpireDate >= DateTime.Now)
                    {
                        ticketList.Remove(guid);
                        ticket.ExpireDate = DateTime.Now.AddHours(3);
                        ticketList.Add(guid, ticket);
                        _application.Cacher.Add(Constants.CK_TicketCache, ticketList, 720);
                        return Result.Success(ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("UserService > IsTicketAuthorized - fail for ticket:{0}!", ex, ticketKey);
            }
            return Result.Error(Resource.User_TicketIsNotValid);
        }

        public Result Register(User user, bool isSocialUser = false)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
                {
                    return Result.Error(Resource.User_MissignInfo);
                }

                var userToAdd = _userRepository.Where(i => i.Email == user.Email).First();
                if (userToAdd != null && userToAdd.ID > 0)
                {
                    return Result.Error(Resource.User_EmailExist);
                }

                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = Encryptor.Encrypt(user.Password);
                }

                user.BirthDate = user.BirthDate == DateTime.MinValue ? DateTime.Now.AddYears(-100) : user.BirthDate;
                user.Status = isSocialUser ? Statuses.Active : Statuses.WaitingActivation;
                user.DateModified = DateTime.Now;
                _userRepository.Add(user);

                if (!isSocialUser)
                {
                    var activationLink = GetActivationLink(user);
                    _mailService.SendWelcomeMail(user.Email, user.Name, activationLink);
                }
                return Result.Success(user, Resource.User_SuccessfullyCreated);
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("UserService > RegisterUser - Fail with email:{0}", user.Email), ex);
                return Result.Error(Resource.General_ExceptionThrowed);
            }
        }

        public Result SaveUser(User user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Email.Trim())) { return Result.Error(Resource.User_EmailIsNotValid); }
                if (!string.IsNullOrEmpty(user.Permissions))
                {
                    user.Permissions = user.Permissions.Trim(',').Trim();
                }

                if (user.ID == 0)
                {
                    return Register(user);
                }

                var userList = _userRepository.Where(i => i.Email == user.Email).ToList();
                if (userList != null && userList.Any(i => i.ID != user.ID)) { return Result.Error(Resource.User_EmailExist); }

                var userToRefresh = userList.FirstOrDefault(i => i.ID == user.ID);
                if (userToRefresh != null)
                {
                    var emailChanged = userToRefresh.Email != user.Email && !string.IsNullOrEmpty(user.Email);
                    Mapper.Map(ref userToRefresh, user, Config.IsAdmin);

                    if (emailChanged) { userToRefresh.Status = Statuses.WaitingActivation; }
                    else if (user.Status >= Statuses.Active) { userToRefresh.Status = user.Status; }
                    _userRepository.Update(userToRefresh);

                    if (emailChanged)
                    {
                        var activationLink = GetActivationLink(user);
                        _mailService.SendActivationMail(userToRefresh.Email, userToRefresh.Name, activationLink);
                    }
                    return Result.Success(userToRefresh, Resource.User_SuccessfullyUpdated);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("UserService > SaveUser - Fail with userID:{0}, email:{1}", ex, user.ID, user.Email);
                return Result.Error(Resource.General_ExceptionThrowed);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result ActivateUser(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data)) { return Result.Error(Resource.User_InvalidActivationCode); }
                var result = DecryptActivationData(data);
                if (result.OK)
                {
                    var user = result.ValueAs<User>();
                    if (user.Status == Statuses.WaitingActivation)
                    {
                        user.Status = Statuses.Active;
                        _userRepository.Update(user);
                        result.Message = Resource.User_ActivationCompletedSuccessfully;
                    }
                    else
                    {
                        result.ResultCode = 4;
                        result.Message = Resource.User_AlreadyActivated;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("UserService > ActivateUser - Fail with data:{0}", ex, data);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result ChangePassword(int userID, string currentPassword, string newPassword, string data = "")
        {
            var result = Result.Error(Resource.User_PasswordRefreshFailed);
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    result = CheckActivationData(data);
                    if (result.OK)
                    {
                        var user = result.ValueAs<User>();
                        userID = user.ID;
                        currentPassword = Encryptor.Decrypt(user.Password);
                    }
                }

                if (userID > 0 && !string.IsNullOrEmpty(newPassword))
                {
                    var user = GetUser(userID);
                    if (user != null && user.ID > 0)
                    {
                        if (user.Password != Encryptor.Encrypt(currentPassword))
                        {
                            return Result.Error(Resource.User_InvalidCurrentPassword);
                        }

                        if (currentPassword == newPassword)
                        {
                            return Result.Error(Resource.User_CurrentPasswordAndNewPasswordAreSame);
                        }

                        user.Password = Encryptor.Encrypt(newPassword);
                        user.DateModified = DateTime.Now;
                        user.Status = user.Status == Statuses.WaitingActivation ? Statuses.Active : user.Status;
                        _userRepository.Update(user);

                        _mailService.NotifyPasswordChanged(user.Email, user.Name);
                        return Result.Success(user, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("UserService > ChangePassword - Fail with userID:{0}, currentPassword : {1}, newPassword:{2}", userID, currentPassword, newPassword), ex);
                result = Result.Error(Resource.User_PasswordRefreshFailed);
            }
            return result;
        }

        public Result SendAuthenticationMail(string email, AuthenticationMailType mailType)
        {
            try
            {
                var user = _userRepository.Where(i => i.Email == email).First();
                if (user == null || user.ID == 0) { return Result.Error(0, Resource.User_EmailNotExist); }
                if (mailType == AuthenticationMailType.ActivationMail && user.Status != Statuses.WaitingActivation) { return Result.Error(2, Resource.User_AlreadyActivated); }

                var linkPage = mailType == AuthenticationMailType.ForgotPasswordMail ? "Account/ChangePassword" : "Account/Activation";
                var activationLink = GetActivationLink(user, linkPage);
                if (mailType == AuthenticationMailType.ActivationMail)
                {
                    _mailService.SendActivationMail(user.Email, user.Name, activationLink);
                    return Result.Success(1, string.Format(Resource.User_ActivationLinkSend2, user.Email), 1);
                }

                _mailService.SendForgotPasswordMail(user.Email, user.Name, activationLink);
                return Result.Success(1, string.Format(Resource.User_ActivationLinkSend1, user.Email), 1);
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("UserService > SendAuthenticationMail - Fail with email:{0}, mailType:{1}", email, mailType), ex);
                return Result.Error(0, Resource.General_ExceptionThrowed);
            }
        }

        public Result CheckActivationData(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data)) { return Result.Error(Resource.User_InvalidActivationCode); }
                var result = DecryptActivationData(data);
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("UserService > CheckActivationData - Fail with data:{0}", data), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        private string GetActivationLink(User user, string linkPage = "Account/Activation")
        {
            if (user == null || user.ID == 0) { throw new Exception("Error while creating activation data!"); }
            var activationData = string.Format("{0}#{1}#{2}", user.ID, user.Email, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            activationData = _application.WebHelper.UrlEncode(Encryptor.Encrypt(activationData));
            return string.Format("{0}/{1}?data={2}", Config.DomainUrl, linkPage, activationData);
        }

        private Result DecryptActivationData(string data)
        {
            try
            {
                //data = Aware.Common.UrlDecode(data);
                var dataValues = Encryptor.Decrypt(data).Split('#');
                DateTime activationDate;
                DateTime.TryParse(dataValues[2], out activationDate);

                var passedTime = DateTime.Now.Subtract(activationDate).TotalMinutes;
                if (passedTime > Constants.ACTVATION_CODE_EXPIRE)
                {
                    return Result.Error(Resource.User_ActivationLinkExpired);
                }

                var userID = dataValues[0].Int();
                var email = dataValues[1];
                var user = _userRepository.Where(i => i.ID == userID && i.Email == email).First();
                if (user != null && user.ID > 0)
                {
                    return Result.Success(user);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("UserService > DecryptActivationData - Fail with data : {0}", ex, data);
            }
            return Result.Error(Resource.User_InvalidActivationCode);
        }

        public Result DeleteUser(int userID, int deletedBy)
        {
            try
            {
                if (userID > 0 && deletedBy > 0)
                {
                    var user = _userRepository.Get(userID);
                    if (user != null && user.IsAdmin)
                    {
                        user.Status = Statuses.Deleted;
                        user.DateModified = DateTime.Now;
                        _userRepository.Update(user);

                        _application.Log.Info("UserService > DeleteUser - Success for user:{0} by {1}", userID, deletedBy);
                        return Result.Success(null, Resource.User_SuccessfullyDeleted);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("UserService > DeleteUser - Fail with data : {0}", ex, userID);
            }
            return Result.Error(Resource.General_Error);
        }
    }
}
