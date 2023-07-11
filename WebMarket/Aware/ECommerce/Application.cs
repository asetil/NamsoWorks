using System;
using System.Collections;
using System.Linq;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model.Custom;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using System.Collections.Generic;
using Aware.Cache;
using Aware.ECommerce.Model;
using Aware.Util.Log;
using Aware.Util.Lookup;

namespace Aware.ECommerce
{
    public class Application : IApplication
    {
        private SiteModel _site;
        private OrderSettingsModel _orderSettings;
        private Hashtable _blackList;

        private readonly ICacher _cacher;
        private readonly ILogger _logger;
        private readonly ILookupManager _lookupManager;
        private readonly IWebHelper _webHelper;

        public Application(ICacher cacher, ILogger logger, ILookupManager lookupManager, IWebHelper webHelper)
        {
            _cacher = cacher;
            _logger = logger;
            _lookupManager = lookupManager;
            _webHelper = webHelper;
        }

        public ICacher Cacher { get { return _cacher; } }
        public ILogger Log { get { return _logger; } }
        public ILookupManager Lookup { get { return _lookupManager; } }
        public IWebHelper WebHelper { get { return _webHelper; } }

        public SiteModel Site
        {
            get
            {
                if (_site == null)
                {
                    var commonService = WindsorBootstrapper.Resolve<ICommonService>();
                    _site = commonService.GetSiteSettings();
                }
                return _site;
            }
        }

        public OrderSettingsModel Order
        {
            get
            {
                if (_orderSettings == null)
                {
                    var commonService = WindsorBootstrapper.Resolve<ICommonService>();
                    var paymentService = WindsorBootstrapper.Resolve<IPaymentService>();

                    var settings = commonService.GetSimpleItems(ItemType.OrderSettings);
                    var bankList = paymentService.GetBankList(Statuses.Active);
                    
                    _orderSettings = new OrderSettingsModel().Set(settings).SetBankList(bankList);
                    var creditCard = _orderSettings.GetPayment(PaymentType.CreditCard);
                    if (creditCard != null)
                    {
                        var posList = paymentService.GetPosDefinitions(true);
                        posList = _orderSettings.UseOOSPayment ? posList.Where(i => i.PaymentMethod == PosPaymentMethod.OOS_PAY || i.PaymentMethod == PosPaymentMethod.Secure3D_OOS).ToList()
                                                               : posList.Where(i => i.PaymentMethod != PosPaymentMethod.OOS_PAY && i.PaymentMethod != PosPaymentMethod.Secure3D_OOS).ToList();
                        _orderSettings.SetPosList(posList);
                    }
                }
                return _orderSettings;
            }
        }

        public void ClearCache(ItemType cacheType)
        {
            if (cacheType == ItemType.None || cacheType == ItemType.SiteSettings)
            {
                _site = null;
            }

            if (cacheType == ItemType.None || cacheType == ItemType.OrderSettings)
            {
                _orderSettings = null;
            }
        }

        private Hashtable BlackList
        {
            get
            {
                if (_blackList == null)
                {
                    _blackList = new Hashtable();
                    LoadBlackList();
                }
                return _blackList;
            }
        }

        public List<string> CheckForBlackList(string content, ref int wordCount)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.ToLowerInvariant().Replace("/", " ").Replace("_", " ");
                var words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                wordCount = words.Count();

                foreach (var word in words)
                {
                    var key = word.Trim().Substring(0, 1);
                    if (BlackList.ContainsKey(key))
                    {
                        var value = BlackList[key].ToString();
                        if (value.IndexOf(word.S()) > -1) { result.Add(word); }
                    }

                    key = string.Format("_{0}", key);
                    if (BlackList.ContainsKey(key))
                    {
                        var values = BlackList[key].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Any(i => word.StartsWith(i))) { result.Add(word); }
                    }
                }
            }
            return result;
        }

        private void LoadBlackList()
        {
            _blackList.Add("a", ",am,amcık,abaza,");
            _blackList.Add("g", ",göt,götveren,");
            _blackList.Add("p", ",pezevenk,");
            _blackList.Add("s", ",sik,sikik,");
            _blackList.Add("y", ",yarrak,");
            _blackList.Add("_s", ",sikil,sikti"); //ile başlayan
            _blackList.Add("_y", ",yarrak,"); //ile başlayan
        }
    }
}
