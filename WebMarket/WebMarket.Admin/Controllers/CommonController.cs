using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Authority.Model;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.Mail;
using Aware.Payment.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace WebMarket.Admin.Controllers
{
    [Aware.Util.Filter.Authorize(AuthorizeLevel.SuperUser, new[] { "Common/AuthorityUsage" })]
    public class CommonController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IPaymentService _paymentService;
        private readonly ICommonService _commonService;
        private readonly ILookupManager _lookupManager;
        public CommonController(IMailService mailService, IUserService userService, IPaymentService paymentService, ICommonService commonService, ILookupManager lookupManager)
        {
            _mailService = mailService;
            _userService = userService;
            _paymentService = paymentService;
            _commonService = commonService;
            _lookupManager = lookupManager;
        }

        #region Site Settings

        public ActionResult SiteSettings()
        {
            ViewBag.YesNoOptions = _lookupManager.GetLookups(LookupType.YesNoOptions);
            var model = _commonService.GetSiteSettings();
            return View(model);
        }

        [HttpPost]
        public ActionResult SiteSettings(SiteModel model)
        {
            var result = _commonService.SaveSiteSettings(model);
            ViewBag.SaveResult = result;
            ViewBag.YesNoOptions = _lookupManager.GetLookups(LookupType.YesNoOptions);

            return View(result.ValueAs<SiteModel>());
        }

        public ActionResult OrderSettings()
        {
            var model = _commonService.GetSimpleItems(ItemType.OrderSettings);
            ViewBag.PosList = _paymentService.GetPosDefinitions(true).Select(i => new Item(i.ID, i.Name)).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderSettings(List<SimpleItem> model)
        {
            foreach (var item in model)
            {
                _commonService.SaveSimpleItem(item);
            }

            model = _commonService.GetSimpleItems(ItemType.OrderSettings);
            ViewBag.PosList = _paymentService.GetPosDefinitions(true).Select(i => new Item(i.ID, i.Name)).ToList();
            ViewBag.SaveResult = Result.Success();
            return View(model);
        }

        public ActionResult SimpleItems(ItemType itemType = ItemType.None)
        {
            ArrangeSimpleItemsTitle(itemType);
            var model = new SimpleItemListModel()
            {
                StatusList = _lookupManager.GetLookups(LookupType.Status),
                List = _commonService.GetSimpleItems(itemType)
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult LoadSimpleItem(int itemID)
        {
            var item = new SimpleItem();
            if (itemID > 0)
            {
                item = _commonService.GetSimpleItem(itemID);
            }
            return Json(new { success = (item != null ? 1 : 0), item }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveSimpleItem(SimpleItem model)
        {
            var result = _commonService.SaveSimpleItem(model);
            return Json(new { success = result.IsSuccess, message = result.Message, itemID = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult RemoveSimpleItem(int itemID)
        {
            var result = _commonService.DeleteSimpleItem(itemID);
            return ResultValue(result);
        }

        #endregion

        #region Cache Management

        public ActionResult CacheManagement()
        {
            var url = string.Format("{0}/GetCacheKeys", ServiceUrl);
            var values = new NameValueCollection();
            values["ticket"] = GetTicket();

            var result = WebRequester.DoRequest<Result>(url, true, values);
            if (result != null && result.OK)
            {
                var model = result.Value as IEnumerable<object>;
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public JsonResult ClearCache(string cacheKey)
        {
            var isSuccess = 0;
            var url = string.Format("{0}/ClearCache", ServiceUrl);
            var values = new NameValueCollection();
            values["ticket"] = GetTicket();
            values["cacheKey"] = cacheKey;

            var result = WebRequester.DoRequest<Result>(url, true, values);
            if (result != null && result.OK)
            {
                isSuccess = 1;
            }
            return Json(new { isSuccess }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Mail Templates

        public ActionResult MailTemplates()
        {
            return View("MailTemplates", GetMailTemplates());
        }

        public ActionResult MailTemplate(int id)
        {
            var mailTemplates =GetMailTemplates();
            ViewBag.MasterTemplates = mailTemplates.Where(i => i.ParentID == 0).Select(i => new Item(i.ID, i.Name));

            var model = id > 0 ? mailTemplates.FirstOrDefault(i => i.ID == id) : new MailTemplate();
            return View("MailTemplate", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MailTemplate(MailTemplate model)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                result = _mailService.Save(model);
                if (result.OK) { model = result.ValueAs<MailTemplate>(); }
            }

            var mailTemplates = GetMailTemplates();
            ViewBag.MasterTemplates = mailTemplates.Where(i => i.ParentID == 0).Select(i => new Item(i.ID, i.Name));
            ViewBag.SaveResult = result;
            return View("MailTemplate", model);
        }

        private List<MailTemplate> GetMailTemplates()
        {
            var result = _mailService.GetAll(1,250).ToList();
            if (result.Any())
            {
                return result.Select(i =>
                {
                    i.Parent = result.FirstOrDefault(s => s.ID == i.ParentID);
                    return i;
                }).ToList();
            }
            return result;
        }

        #endregion

        #region Authority

        public ActionResult AuthorityDefinition()
        {
            var model = _commonService.GetAuthorityDefinitions(true);
            return View(model);
        }

        [Aware.Util.Filter.Authorize]
        public ActionResult AuthorityUsage(int relationID, int relationType)
        {
            ViewBag.RelationID = relationID;
            ViewBag.RelationType = relationType;
            ViewBag.AllowSave = CurrentUser.IsAdmin;
            if (relationType == (int)RelationTypes.User && relationID == CurrentUserID) { ViewBag.AllowSave = false; }

            var model = _commonService.GetAuthorityUsages(relationID, relationType, IsSuper(false));
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult GetAuthorityDetail(int id)
        {
            var authority = _commonService.GetAuthorityDefinition(id);
            return Json(new { authority }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveAuthority(int id, string title, AuthorityType type, AuthorityMode mode)
        {
            var result = _commonService.SaveAuthority(new AuthorityDefinition
            {
                ID = id,
                Title = title,
                Type = type,
                Mode = mode
            });
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult SaveAuthorityUsage(int relationID, int relationType, List<AuthorityUsage> authorities)
        {
            var enableQuota = IsSuper(false);
            var result = _commonService.SaveAuthorityUsage(relationID, relationType, authorities, enableQuota);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult DeleteAuthority(int authorityID)
        {
            var result = _commonService.DeleteAuthority(authorityID);
            return ResultValue(result);
        }

        #endregion

        #region Online Pos Management

        public ActionResult OnlinePos()
        {
            var model = _paymentService.GetPosDefinitions();
            return View(model);
        }

        public ActionResult OnlinePosDetail(int id)
        {
            var model = _paymentService.GetPosDefinitionDetail(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult OnlinePosDetail(PosDefinition model)
        {
            var result = Result.Error();
            if (ModelState.IsValid)
            {
                var isNew = model.ID == 0;
                result = _paymentService.SavePosDefinition(model);
                if (result.OK)
                {
                    model = result.ValueAs<PosDefinition>();
                    if (isNew)
                    {
                        return RedirectToRoute(Helper.RouteNames.OnlinePosDetailRoute, new { id = model.ID });
                    }
                }
            }

            ViewBag.SaveResult = result;
            var detailModel = _paymentService.GetPosDefinitionDetail(model.ID);
            return View(detailModel);
        }

        #endregion

        #region Helpers

        private string GetTicket()
        {
            var url = string.Format("{0}/GetTicket", ServiceUrl);
            var values = new NameValueCollection();

            var user = _userService.GetAdminUser(CurrentUserID);
            values["email"] = user.Email;
            values["password"] = user.Password;

            var authorizeResult = WebRequester.DoRequest<Result>(url, true, values);
            if (authorizeResult != null && authorizeResult.OK)
            {
                return authorizeResult.Value as string;
            }
            return string.Empty;
        }

        private string ServiceUrl
        {
            get { return string.Format("{0}/Service", Config.SiteUrl); }
        }

        private void ArrangeSimpleItemsTitle(ItemType itemType)
        {
            ViewBag.ItemType = (int)itemType;
            switch (itemType)
            {
                case ItemType.SiteSettings:
                    ViewBag.Title = "Site Ayarları"; break;
                case ItemType.UserPermissions:
                    ViewBag.Title = "Kullanıcı İzinleri"; break;
                case ItemType.OrderSettings:
                    ViewBag.Title = "Sipariş Ayarları"; break;
                default:
                    ViewBag.Title = "Çoklu Öğeler"; break;
            }
        }

        #endregion
    }
}