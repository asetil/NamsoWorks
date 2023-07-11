using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using System;
using Aware.Data;
using Aware.Util;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model.Custom;
using Aware.ECommerce.Search;
using Aware.ECommerce.Util;
using Aware.Search;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class PropertyService : IPropertyService
    {
        private readonly IApplication _application;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PropertyRelation> _propertyRelationRepository;
        private readonly IRepository<PropertyValue> _propertyRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IWebHelper _webHelper;

        public PropertyService(IApplication application, IRepository<PropertyRelation> propertyRelationRepository, IRepository<Comment> commentRepository, IRepository<Product> productRepository, IRepository<PropertyValue> propertyRepository, IWebHelper webHelper)
        {
            _application = application;
            _propertyRelationRepository = propertyRelationRepository;
            _commentRepository = commentRepository;
            _productRepository = productRepository;
            _propertyRepository = propertyRepository;
            _webHelper = webHelper;
        }

        public List<PropertyValue> GetProperties(int parentID = 0)
        {
            List<PropertyValue> result = null;
            if (parentID > 0)
            {
                result = _propertyRepository.Where(i => i.ParentID == parentID).SortBy(i => i.SortOrder).ToList();
            }
            else
            {
                result = _propertyRepository.Where(i => i.Type == PropertyType.PropertyGroup ||
                    (i.Type != PropertyType.PropertyOption && i.Type != PropertyType.VariantProperty && i.Type != PropertyType.VariantOption))
                    .SortBy(i => i.SortOrder).ToList();
            }
            return result;
        }

        public List<PropertyValue> GetAllCachedProperties()
        {
            var result = _application.Cacher.Get<List<PropertyValue>>(Constants.CK_Properties);
            if (result == null)
            {
                result = _propertyRepository.Where(i => i.Type != PropertyType.VariantOption && i.Type != PropertyType.VariantProperty).ToList();
                _application.Cacher.Add(Constants.CK_Properties, result);
            }
            return result;
        }

        public PropertyViewModel GetPropertyDetail(int propertyID)
        {
            List<PropertyValue> propertyValues = null;
            if (propertyID > 0)
            {
                propertyValues = _propertyRepository.Where(i => i.ID == propertyID || i.Type == PropertyType.PropertyGroup || (i.ParentID == propertyID && i.Type != PropertyType.VariantProperty && i.Type != PropertyType.VariantOption)).ToList();
            }
            else
            {
                propertyValues = _propertyRepository.Where(i => i.Type == PropertyType.PropertyGroup).ToList();
            }

            var model = new PropertyViewModel()
            {
                Property = propertyID > 0 ? propertyValues.FirstOrDefault(i => i.ID == propertyID) : new PropertyValue(),
                ParentList = propertyValues.Where(i => i.Type == PropertyType.PropertyGroup).ToList(),
                ChildList = propertyID > 0 ? propertyValues.Where(i => i.ParentID == propertyID).ToList() : null,
                PropertyTypeList = _application.Lookup.GetLookups(LookupType.PropertyTypes),
                StatusList = _application.Lookup.GetLookups(LookupType.Status)
            };
            return model;
        }

        public List<PropertyRelation> GetProductRelations(int productID, bool onlyActive = false)
        {
            if (productID > 0)
            {
                var criteriaHelper = _propertyRelationRepository.CriteriaHelper
                                    .WithAlias("PropertyValue", "pv")
                                    .Eq("RelationID", productID)
                                    .Eq("RelationType", (int)RelationTypes.Product);

                if (onlyActive)
                {
                    criteriaHelper.Eq("Status", Statuses.Active);
                }

                var relations = _propertyRelationRepository.GetWithCriteria(criteriaHelper);
                return relations;
            }
            return null;
        }

        public ProductRelationsModel GetProductRelationProperties(int productID)
        {
            if (productID > 0)
            {
                var relations = GetProductRelations(productID);
                var propertyList = _propertyRepository.Where(i => i.Type != PropertyType.PropertyGroup && i.Type != PropertyType.VariantProperty && i.Type != PropertyType.VariantOption).ToList();

                return new ProductRelationsModel
                {
                    ProductID = productID,
                    PropertyList = propertyList.Where(i => i.Type != PropertyType.PropertyOption).ToList(),
                    SelectionList = propertyList.Where(i => i.Type == PropertyType.PropertyOption).ToList(),
                    RelationList = relations,
                    HasNoOptionList = _application.Lookup.GetLookupItems(LookupType.HasNoOptions)
                };
            }
            return null;
        }

        public SearchResult<Comment> GetComments(CommentSearchParams searchParams)
        {
            var result = _commentRepository.Find(searchParams);
            return result;
        }

        public List<Comment> GetProductComments(int productID, CommentStatus status = CommentStatus.Approved, int page = 1)
        {
            var searchParams = new CommentSearchParams
            {
                IDs = new List<int>() { productID },
                RelationType = (int)RelationTypes.Product,
                CommentStatus = status
            };
            searchParams.SetPaging(page, Constants.COMMENT_PAGE_SIZE).SortBy(i=>i.DateCreated,true);

            var result = _commentRepository.Find(searchParams);
            if (result.Success)
            {
                return result.Results;
            }
            return new List<Comment>();
        }

        public CommentViewModel GetProductCommentStats(int productID, int ownerID, int page = 1)
        {
            try
            {
                if (productID > 0)
                {
                    var result = new CommentViewModel();
                    var sql = SqlHelper.GetCommentStats(ownerID, productID, (int)RelationTypes.Product);
                    var statValues = _propertyRelationRepository.GetWithSql<Item>(sql, false);

                    var stats = new Dictionary<int, int>();
                    for (int i = 0; i <= 6; i++)
                    {
                        var stat = 0;
                        if (statValues.Any(s => s.ID == i))
                        {
                            stat = statValues.FirstOrDefault(s => s.ID == i).Value.Int();
                        }
                        stats.Add(i, stat);
                    }

                    result.Stats = stats;
                    result.RelationID = productID;
                    result.CommentCount = stats.Where(i => i.Key <= 5).Sum(i => i.Value);
                    result.AllowNewComment = _application.Site.AllowNewComment && stats.ContainsKey(6) && stats[6] == 0;

                    if (result.CommentCount > 0 && _application.Site.DisplayComments)
                    {
                        result.Comments = GetProductComments(productID, CommentStatus.Approved, page);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("PropertyInfoService > GetProductCommentStats - Fail with : {0}!", ex, productID);
            }
            return null;
        }

        public Result SaveProperty(PropertyValue model)
        {
            try
            {
                if (model == null) { return Result.Error(); }
                if (model.ID > 0)
                {
                    var property = _propertyRepository.Get(model.ID);
                    if (property != null)
                    {
                        Mapper.Map(ref property, model);
                        _propertyRepository.Update(property);
                    }
                    else { throw new Exception("Property not found!"); }
                }
                else
                {
                    model.SortOrder = model.SortOrder ?? string.Empty;
                    _propertyRepository.Add(model);
                }
                return Result.Success(model.ID, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error("PropertyService > SaveProperty - Fail with : {0}!",ex, model.ID);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SavePropertyOption(PropertyValue model, bool isVariant)
        {
            try
            {
                if (model == null || model.ParentID <= 0) { return Result.Error(Resource.General_Error); }
                if (model.ID > 0)
                {
                    var property = _propertyRepository.Get(model.ID);
                    if (property != null)
                    {
                        property.Name = model.Name;
                        property.Type = isVariant ? PropertyType.VariantOption : PropertyType.PropertyOption;
                        property.SortOrder = model.SortOrder;
                        _propertyRepository.Update(property);
                    }
                    else { throw new Exception("Property Not Found"); }
                }
                else
                {
                    if (!isVariant)
                    {
                        var parentValue = _propertyRepository.Get(model.ParentID);
                        if (parentValue == null) { throw new Exception("Parent Property Not Found"); }
                    }

                    model.Status = Statuses.Active;
                    model.Type = isVariant ? PropertyType.VariantOption : PropertyType.PropertyOption; ;
                    _propertyRepository.Add(model);
                }
                return Result.Success(model, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("Product > SavePropertyOption - Fail with : {0}!", model.ID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SavePropertyRelation(int propertyID, string value, string sortOrder, int relationID, int relationType)
        {
            try
            {
                if (relationID > 0 && propertyID > 0 && !string.IsNullOrEmpty(value))
                {
                    var relation = _propertyRelationRepository.Where(i => i.PropertyValueID == propertyID && i.RelationID == relationID && i.RelationType == relationType).First();
                    var propertyValue = _propertyRepository.Get(propertyID);
                    if (relation != null)
                    {
                        relation.Value = value;
                        relation.SortOrder = sortOrder;
                        relation.Status = Statuses.Active;
                        relation.PropertyValue = propertyValue;
                        _propertyRelationRepository.Update(relation);
                    }
                    else
                    {
                        relation = new PropertyRelation()
                        {
                            RelationID = relationID,
                            RelationType = relationType,
                            PropertyValueID = propertyID,
                            SortOrder = sortOrder,
                            Value = value,
                            PropertyValue = propertyValue,
                            Status = Statuses.Active
                        };
                        _propertyRelationRepository.Add(relation);
                    }

                    RefreshPropertyRelationInfo(relation);
                    return Result.Success(relation, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("Product > SavePropertyRelation - Fail with propertyID: {0} and relation : {1}-{2}!", propertyID, relationID, relationType), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveComment(Comment model)
        {
            try
            {
                var validation = CheckComment(model);
                if (!validation.OK) { return validation; }

                if (model.ID > 0)
                {
                    var comment = _commentRepository.Get(model.ID);
                    if (comment != null)
                    {
                        comment.Title = model.Title;
                        comment.Value = model.Value;
                        comment.Rating = model.Rating;
                        comment.DateModified = DateTime.Now;
                        _commentRepository.Update(comment);
                    }
                    else { throw new Exception("comment not found!"); }
                }
                else
                {
                    model.DateModified = DateTime.Now;
                    model.DateCreated = DateTime.Now;
                    model.Status = CommentStatus.WaitingApproval;
                    _commentRepository.Add(model);
                }
                return Result.Success(model.ID, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("PropertyService > SaveComment - Fail with : {0}!", model.ID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        private Result CheckComment(Comment model)
        {
            if (model == null) { return Result.Error(); }

            var wordCount = 0;
            var titleAbuseList = _application.CheckForBlackList(model.Title, ref wordCount);
            if (titleAbuseList.Any())
            {
                //var message = string.Format(Resource.Comment_TitleContainsAbusiveWord, string.Join(", ", titleAbuseList));
                return Result.Error(Resource.Comment_TitleContainsAbusiveWord);
            }

            var contentAbuseList = _application.CheckForBlackList(model.Value, ref wordCount);
            if (contentAbuseList.Any())
            {
                //var message=string.Format(Resource.Comment_ContentContainsAbusiveWord,string.Join(", ", contentAbuseList));
                return Result.Error(Resource.Comment_ContentContainsAbusiveWord);
            }

            if (wordCount < 5)
            {
                return Result.Error(Resource.Comment_ContentMinLengthError);
            }
            return Result.Success();
        }

        public Result DeletePropertyRelation(int propertyID, int relationID, int relationType)
        {
            try
            {
                if (relationID > 0 && propertyID > 0)
                {
                    var relation = _propertyRelationRepository.Where(i => i.PropertyValueID == propertyID && i.RelationID == relationID && i.RelationType == relationType).First();
                    if (relation != null && relation != null)
                    {
                        relation.Status = Statuses.Passive;
                        _propertyRelationRepository.Update(relation);
                        RefreshPropertyRelationInfo(relation);
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("Product > DeletePropertyRelation - Fail with propertyID: {0} for relation {1}-{2}!", propertyID, relationID, relationType), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteProperty(int id, bool deleteWithRelations)
        {
            try
            {
                if (id > 0)
                {
                    var subProperties = _propertyRepository.Where(i => i.ParentID == id && i.Type == PropertyType.PropertyOption).ToList();
                    if (subProperties != null && subProperties.Any())
                    {
                        return Result.Error("Özelliğin alt özellikleri olduğundan silinemez!");
                    }

                    var relations = _propertyRelationRepository.Where(i => i.PropertyValueID == id).ToList();
                    if (relations != null && relations.Any() && !deleteWithRelations)
                    {
                        return Result.Error("Bu özelliği kullanan ürünler olduğundan özellik silinemiyor!");
                    }

                    foreach (var relation in relations)
                    {
                        _propertyRelationRepository.Delete(relation.ID);
                        RefreshPropertyRelationInfo(relation);
                    }

                    _propertyRepository.Delete(id);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("Product > DeleteProperty - Fail with : {0}!", id), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public bool RefreshProductProperties(int productID = 0)
        {
            try
            {
                var sql = SqlHelper.RefreshProductProperties(productID);
                var result = _propertyRepository.ExecuteSp(sql);
                return result > 0;
            }
            catch (Exception ex)
            {
                _application.Log.Error("Product > RefreshProductProperties - Fail with productID: {0}!", ex, productID);
            }
            return false;
        }

        public bool RefreshProductImages(int productID = 0)
        {
            try
            {
                var sql = SqlHelper.RefreshProductImages(productID);
                var result = _propertyRepository.ExecuteSp(sql);
                return result > 0;
            }
            catch (Exception ex)
            {
                _application.Log.Error("Product > RefreshProductImages - Fail with productID: {0}!", ex, productID);
            }
            return false;
        }

        private bool RefreshPropertyRelationInfo(PropertyRelation model)
        {
            if (model != null)
            {
                var relations = GetProductRelations(model.RelationID, true);
                var propertyView = relations.Select(i => new PropertyView()
                {
                    ID = i.ID,
                    Parent = i.PropertyValue.ParentID,
                    Sort = i.SortOrder,
                    Value = i.Value
                });

                var definition = _webHelper.Serialize(propertyView);
                switch (model.RelationType)
                {
                    case (int)RelationTypes.Product:
                        var product = _productRepository.Get(model.RelationID);
                        if (product != null)
                        {
                            product.PropertyInfo = definition;
                            product.DateModified = DateTime.Now;
                            _productRepository.Update(product);
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        public bool IsUserCommented(int userID, int productID)
        {
            if (userID > 0 && productID > 0)
            {
                var userComments = _commentRepository.Where(i => i.OwnerID == userID && i.RelationType == (int)RelationTypes.Product && i.RelationID == productID).ToList();
                return userComments != null && userComments.Any();
            }
            return false;
        }

        public Result ChangeCommentStatus(int commentID, CommentStatus status, int evaluatorID)
        {
            try
            {
                if (commentID > 0 && evaluatorID > 0)
                {
                    var comment = _commentRepository.Get(commentID);
                    if (comment != null && comment.ID > 0)
                    {
                        comment.Status = status;
                        comment.EvaluatorID = evaluatorID;
                        comment.DateModified = DateTime.Now;
                        _commentRepository.Update(comment);
                        return Result.Success(null, Resource.Comment_ChangeStatus_Successfull);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("Product > ChangeCommentStatus - Fail with : {0}, evaluatorID:{1}!", ex, commentID, evaluatorID);
            }
            return Result.Error(Resource.General_Error);
        }
    }
}