using Microsoft.AspNetCore.Mvc;
using Worchart.BL.Constants;
using Worchart.BL.Model;
using Worchart.BL.User;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using Worchart.BL.Token;
using System;
using Worchart.BL.Manager;
using Microsoft.AspNetCore.Http;
using Worchart.BL;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Worchart.API.Controllers
{
    //[EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private AuthorizedCustomerModel _customerModel = null;

        public AuthorizedCustomerModel Customer
        {
            get
            {
                if (_customerModel == null)
                {
                    if (Request.Headers.ContainsKey(CommonConstants.CustomerUserToken))
                    {
                        var userToken = Request.Headers[CommonConstants.CustomerUserToken].ToString();
                        var tokenManager = GetService<ITokenManager>();
                        _customerModel = tokenManager.GetUserAuthorization(userToken);
                    }
                }
                return _customerModel;
            }
        }

        public int CustomerID
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.ID;
                }
                return 0;
            }
        }

        protected OperationResult Validate()
        {
            if (ModelState.IsValid)
            {
                return Success();
            }

            var result = Failed();
            result.ValidationInfo = GetValidationInfo(ModelState);
            return result;
        }

        protected OperationResult WithResult(OperationResult operationResult)
        {
            if (operationResult != null)
            {
                operationResult.Message = GetMessage(operationResult.Code);
            }
            return operationResult;
        }

        protected OperationResult Success(object value = null)
        {
            return new OperationResult(ErrorConstants.OperationSuccess) { Value = value };
        }

        protected OperationResult Failed(string code = ErrorConstants.OperationFailed)
        {
            return new OperationResult(code);
        }

        private List<ValidationItem> GetValidationInfo(ModelStateDictionary modelState)
        {
            var result = new List<ValidationItem>();
            foreach (var item in modelState)
            {
                result.Add(new ValidationItem()
                {
                    Key = item.Key,
                    Message = string.Join("|", item.Value.Errors.Select(i => i.ErrorMessage))
                });
            }
            return result;
        }

        private string GetMessage(string code)
        {
            var resourceManager = GetService<IResourceManager>();
            return resourceManager.GetValue(code, UserLanguage);
        }

        protected string UserLanguage
        {
            get
            {
                if (Request.Headers.ContainsKey(CommonConstants.LanguageHeader))
                {
                    return Request.Headers[CommonConstants.LanguageHeader].ToString();
                }
                return CommonConstants.LanguageEN;
            }
        }

        protected string CurrentIpAddress
        {
            get
            {
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                return remoteIpAddress != null ? remoteIpAddress.ToString() : CommonConstants.DefaultIp;
            }
        }

        protected string GetFilePath(IFormFile file, string relationName, string currentPath)
        {
            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var pattern = string.Format("{0}/(.*){1}", relationName, extension);

                if (!currentPath.Valid() || !Regex.IsMatch(currentPath, pattern))
                {
                    var name = Guid.NewGuid().ToString().Replace("-", "").Replace(" ", "").Trim();
                    var newPath = string.Format("{0}/{1}.{2}", relationName, name, file, extension);
                    return newPath;
                }
            }
            return currentPath;
        }

        protected OperationResult SaveFile(IFormFile file, string path)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var hostingEnvironment = GetService<IHostingEnvironment>();
                    var directory = Path.Combine(hostingEnvironment.WebRootPath, "img/", path.Split('/')[0]);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var filePath = Path.Combine(hostingEnvironment.WebRootPath, "img/", path);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyToAsync(fileStream);
                    }
                    return new OperationResult(ErrorConstants.OperationSuccess);
                }
            }
            catch (Exception ex)
            {
                var logger = GetService<Worchart.BL.Log.ILogger>();
                logger.Error("BaseController|SaveFile", path, ex);
            }
            return new OperationResult(ErrorConstants.OperationFailed);
        }

        protected bool RemoveFile(string path)
        {
            try
            {
                if (path.Valid())
                {
                    var hostingEnvironment = GetService<IHostingEnvironment>();
                    var filePath = Path.Combine(hostingEnvironment.WebRootPath, "img/", path);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                var logger = GetService<Worchart.BL.Log.ILogger>();
                logger.Error("BaseController|RemoveFile", path, ex);
            }
            return false;
        }

        protected T GetService<T>()
        {
            var services = this.HttpContext.RequestServices;
            var service = (T)services.GetService(typeof(T));
            return service;
        }
    }
}