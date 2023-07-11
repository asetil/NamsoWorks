using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.ECommerce.Util;
using Aware.Util;
using System.Collections.Specialized;
using Aware.Authority;
using Aware.Authority.Model;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<SimpleItem> _simpleItemRepository;
        private readonly IAuthorityManager _authorityManager;
        private readonly IApplication _application;

        public CommonService(IRepository<SimpleItem> simpleItemRepository, IAuthorityManager authorityManager, IApplication application)
        {
            _simpleItemRepository = simpleItemRepository;
            _authorityManager = authorityManager;
            _application = application;
        }

        public SiteModel GetSiteSettings()
        {
            try
            {
                var settings = GetSimpleItems(ItemType.SiteSettings);
                var result = new SiteModel
                {
                    DisplayComments = ProcessSiteSetting(SiteSettingsType.DisplayComments, settings, "", true).ToLowerInvariant() == "true",
                    AllowNewComment = ProcessSiteSetting(SiteSettingsType.AllowNewComment, settings, "", true).ToLowerInvariant() == "true",
                    AllowSocialLogin = ProcessSiteSetting(SiteSettingsType.AllowSocialLogin, settings, "", true).ToLowerInvariant() == "true",
                    AllowSocialShare = ProcessSiteSetting(SiteSettingsType.AllowSocialShare, settings, "", true).ToLowerInvariant() == "true",
                    AllowProductCompare = ProcessSiteSetting(SiteSettingsType.AllowProductCompare, settings, "", true).ToLowerInvariant() == "true",
                    ShowProductInstallments = ProcessSiteSetting(SiteSettingsType.ShowProductInstallments, settings, "", true).ToLowerInvariant() == "true",
                    UseMultiLanguage = ProcessSiteSetting(SiteSettingsType.UseMultiLanguage, settings, "", true).ToLowerInvariant() == "true",

                    MailHost = ProcessSiteSetting(SiteSettingsType.MailHost, settings, "", true),
                    MailPort = ProcessSiteSetting(SiteSettingsType.MailPort, settings, "", true),
                    MailUser = ProcessSiteSetting(SiteSettingsType.MailUser, settings, "", true),
                    MailPassword = ProcessSiteSetting(SiteSettingsType.MailPassword, settings, "", true),

                    FacebookApiKey = ProcessSiteSetting(SiteSettingsType.FacebookApiKey, settings, "", true),
                    FacebookApiSecret = ProcessSiteSetting(SiteSettingsType.FacebookApiSecret, settings, "", true),
                    GoogleApiKey = ProcessSiteSetting(SiteSettingsType.GoogleApiKey, settings, "", true),
                    GoogleApiSecret = ProcessSiteSetting(SiteSettingsType.GoogleApiSecret, settings, "", true),
                    ReCaptchaSecret = ProcessSiteSetting(SiteSettingsType.ReCaptchaSecret, settings, "", true),
                };
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > GetSiteSettings - Fail", ex);
            }
            return new SiteModel();
        }

        public List<SimpleItem> GetSimpleItems(ItemType itemType, Statuses status = Statuses.None)
        {
            try
            {
                if (status != Statuses.None)
                {
                    return _simpleItemRepository.Where(i => i.Type == itemType && i.Status == status).SortBy(f => f.SortOrder).ToList();
                }
                return _simpleItemRepository.Where(i => i.Type == itemType).SortBy(f => f.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > GetSimpleItems - Fail for itemType:{0}", ex, itemType);
                return null;
            }
        }

        public List<SimpleItem> GetCachedSimpleItems(ItemType itemType)
        {
            try
            {
                var key = string.Format(Constants.CK_SimpleItems, itemType);
                var result = _application.Cacher.Get<List<SimpleItem>>(key);
                if (result == null)
                {
                    result = GetSimpleItems(itemType, Statuses.Active) ?? new List<SimpleItem>();
                    _application.Cacher.Add(key, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > GetCachedSimpleItems - Failed for itemType:{0}", ex, itemType);
            }
            return new List<SimpleItem>();
        }

        public AuthorityDefinition GetAuthorityDefinition(int id)
        {
            return _authorityManager.GetAuthorityDefinition(id);
        }

        public AuthorityDefinitionListModel GetAuthorityDefinitions(bool enableQuota)
        {
            var result = new AuthorityDefinitionListModel()
            {
                AuthorityDefinitions = _authorityManager.GetAuthorityDefinitions(enableQuota),
                AuthorityModeList = _application.Lookup.GetLookups(LookupType.AuthorityModes),
                AuthorityTypeList = _application.Lookup.GetLookups(LookupType.AuthorityTypes)
            };
            return result;
        }

        public List<AuthorityDefinition> GetAuthorityUsages(int relationID, int relationType, bool enableQuota)
        {
            return _authorityManager.GetAuthorityUsages(relationID, relationType, enableQuota);
        }

        public Result DeleteSimpleItem(int itemID)
        {
            try
            {
                if (itemID > 0)
                {
                    var simpleItem = _simpleItemRepository.Get(itemID);
                    if (simpleItem != null)
                    {
                        _simpleItemRepository.Delete(simpleItem);
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > DeleteSimpleItem - Failed for id:{0}", ex, itemID);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveSimpleItem(SimpleItem model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID <= 0)
                    {
                        _simpleItemRepository.Add(model);
                        return Result.Success(model, Resource.General_Success);
                    }

                    var item = _simpleItemRepository.Get(model.ID);
                    if (item != null)
                    {
                        Mapper.Map(ref item, model);
                        _simpleItemRepository.Update(item);
                        return Result.Success(item, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > SaveSimpleItem - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveSiteSettings(SiteModel model)
        {
            try
            {
                if (model != null)
                {
                    var simpleItems = GetSimpleItems(ItemType.SiteSettings);
                    ProcessSiteSetting(SiteSettingsType.DisplayComments, simpleItems, model.DisplayComments.ToString());
                    ProcessSiteSetting(SiteSettingsType.AllowNewComment, simpleItems, model.AllowNewComment.ToString());
                    ProcessSiteSetting(SiteSettingsType.AllowSocialLogin, simpleItems, model.AllowSocialLogin.ToString());
                    ProcessSiteSetting(SiteSettingsType.AllowSocialShare, simpleItems, model.AllowSocialShare.ToString());
                    ProcessSiteSetting(SiteSettingsType.AllowProductCompare, simpleItems, model.AllowProductCompare.ToString());
                    ProcessSiteSetting(SiteSettingsType.ShowProductInstallments, simpleItems, model.ShowProductInstallments.ToString());
                    ProcessSiteSetting(SiteSettingsType.UseMultiLanguage, simpleItems, model.UseMultiLanguage.ToString());

                    ProcessSiteSetting(SiteSettingsType.MailHost, simpleItems, model.MailHost);
                    ProcessSiteSetting(SiteSettingsType.MailPort, simpleItems, model.MailPort);
                    ProcessSiteSetting(SiteSettingsType.MailUser, simpleItems, model.MailUser);
                    ProcessSiteSetting(SiteSettingsType.MailPassword, simpleItems, model.MailPassword);

                    ProcessSiteSetting(SiteSettingsType.FacebookApiKey, simpleItems, model.FacebookApiKey);
                    ProcessSiteSetting(SiteSettingsType.FacebookApiSecret, simpleItems, model.FacebookApiSecret);
                    ProcessSiteSetting(SiteSettingsType.GoogleApiKey, simpleItems, model.GoogleApiKey);
                    ProcessSiteSetting(SiteSettingsType.GoogleApiSecret, simpleItems, model.GoogleApiSecret);
                    ProcessSiteSetting(SiteSettingsType.ReCaptchaSecret, simpleItems, model.ReCaptchaSecret);

                    _simpleItemRepository.Save();
                    return Result.Success(model, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("CommonService > SaveSiteSettings - Failed", ex);
            }
            return Result.Error(Resource.General_Error, model);
        }

        public SimpleItem GetSimpleItem(int itemID)
        {
            return _simpleItemRepository.Get(itemID);
        }

        public Result DeleteAuthority(int authorityID)
        {
            return _authorityManager.DeleteAuthority(authorityID);
        }

        public Result SaveAuthority(AuthorityDefinition model)
        {
            return _authorityManager.SaveAuthority(model);
        }

        public Result SaveAuthorityUsage(int relationID, int relationType, List<AuthorityUsage> authorities, bool enableQuota)
        {
            return _authorityManager.SaveAuthorityUsage(relationID, relationType, authorities, enableQuota);
        }

        private string ProcessSiteSetting(SiteSettingsType settingType, IEnumerable<SimpleItem> settings, string value, bool getValue = false)
        {
            var setting = settings.FirstOrDefault(i => i.SubType == (int)settingType);
            if (getValue) { return setting != null && !string.IsNullOrEmpty(setting.Value) ? setting.Value : string.Empty; }

            if (setting != null)
            {
                setting.Value = value;
                _simpleItemRepository.Update(setting, false);
            }
            else
            {
                setting = new SimpleItem { Title = settingType.ToString(), Value = value, SubType = (int)settingType, Type = ItemType.SiteSettings, Status = Statuses.Active };
                _simpleItemRepository.Add(setting, false);
            }
            return setting.Value;
        }

        public Result CheckCaptcha(string capcthaRequest, string ipAddress)
        {
            if (!string.IsNullOrEmpty(capcthaRequest) && !string.IsNullOrEmpty(ipAddress))
            {
                var values = new NameValueCollection();
                values["secret"] = _application.Site.ReCaptchaSecret;
                values["response"] = capcthaRequest;
                values["remoteip"] = ipAddress;
                var response = WebRequester.DoRequest(Constants.GoogleRecaptchaValidateUrl, "", true, values);

                var resultDictionary = response.DeSerialize<Dictionary<string, string>>();
                if (resultDictionary.ContainsKey("success") && Convert.ToBoolean(resultDictionary["success"]))
                {
                    return Result.Success();
                }
            }
            return Result.Error("Güvenlik doğrulaması başarısız. Lütfen tekrar deneyin");
        }
    }
}