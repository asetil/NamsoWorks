using System.Collections.Generic;
using Aware.Authority.Model;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Interface
{
    public interface ICommonService
    {
        SiteModel GetSiteSettings();
        List<SimpleItem> GetSimpleItems(ItemType itemType, Statuses status = Statuses.None);
        List<SimpleItem> GetCachedSimpleItems(ItemType itemType);
        AuthorityDefinition GetAuthorityDefinition(int id);
        AuthorityDefinitionListModel GetAuthorityDefinitions(bool enableQuota);
        List<AuthorityDefinition> GetAuthorityUsages(int relationID, int relationType, bool enableQuota);
        Result DeleteSimpleItem(int itemID);
        Result SaveSimpleItem(SimpleItem model);
        Result SaveSiteSettings(SiteModel model);
        SimpleItem GetSimpleItem(int itemID);
        Result DeleteAuthority(int authorityID);
        Result SaveAuthority(AuthorityDefinition model);
        Result SaveAuthorityUsage(int relationID, int relationType, List<AuthorityUsage> authorities, bool enableQuota);
        Result CheckCaptcha(string capcthaRequest, string ipAddress);
    }
}