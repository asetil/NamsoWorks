using Aware.ECommerce.Model;
using System.Collections.Generic;
using Aware.Util.Model;

namespace Aware.ECommerce.Interface
{
    public interface IVariantService
    {
        VariantListModel GetVariantListModel(List<int> idList = null, bool includeOptions = false);
        VariantProperty GetVariantProperty(int propertyID, bool includeOptions = false);
        VariantDetailModel GetVariantDetail(int id);
        VariantRelationViewModel GetVariantRelations(int relationID, int relationType, bool useCache = false);
        VariantSelection GetVariantSelection(int relationID, int relationType, int code);
        VariantCheckResult CheckVariantSelection(string selectionString, int relationID, int relationType);
        int GetVariantCode(string variantString);

        Result SaveVariantProperty(VariantProperty model);
        Result SaveVariantRelation(VariantRelation relation);
        Result SaveVariantSelection(VariantSelection model);
        Result DeleteVariantProperty(int id);
        Result DeleteVariantRelation(int id);
        Result DeleteVariantSelection(int id);
        bool RefreshVariantStock(VariantSelection selection, decimal quantity);
    }
}
