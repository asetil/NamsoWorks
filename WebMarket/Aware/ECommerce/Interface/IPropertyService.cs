using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.ECommerce.Model.Custom;
using Aware.ECommerce.Search;
using Aware.Search;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Interface
{
    public interface IPropertyService
    {
        List<PropertyValue> GetProperties(int parentID = 0);
        List<PropertyValue> GetAllCachedProperties();
        PropertyViewModel GetPropertyDetail(int propertyID);
        List<PropertyRelation> GetProductRelations(int productID, bool onlyActive=false);
        ProductRelationsModel GetProductRelationProperties(int productID);
        SearchResult<Comment> GetComments(CommentSearchParams searchParams);
        List<Comment> GetProductComments(int productID, CommentStatus status = CommentStatus.Approved,int page=1);
        CommentViewModel GetProductCommentStats(int productID, int ownerID,int page=1);
        Result SaveProperty(PropertyValue model);
        Result SavePropertyOption(PropertyValue model,bool isVariant);
        Result SavePropertyRelation(int propertyID, string value, string sortOrder, int relationID, int relationType);
        Result SaveComment(Comment model);
        Result DeletePropertyRelation(int propertyID, int relationID, int relationType);
        Result DeleteProperty(int id, bool deleteWithRelations);
        bool IsUserCommented(int userID, int productID);
        Result ChangeCommentStatus(int commentID, CommentStatus status, int evaluatorID);
        bool RefreshProductProperties(int productID = 0);
        bool RefreshProductImages(int productID = 0);
       
    }
}