using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Lookup;

namespace Aware.ECommerce.Service
{
    public class VariantService : IVariantService
    {
        private readonly IRepository<PropertyValue> _propertyRepository;
        private readonly IRepository<VariantProperty> _variantRepository;
        private readonly IRepository<VariantRelation> _variantRelationRepository;
        private readonly IRepository<VariantSelection> _variantSelectionRepository;
        private readonly IApplication _application;

        private readonly char seperator_c = ';';
        private readonly string seperator_s = ";";

        public VariantService(IRepository<PropertyValue> propertyRepository, IRepository<VariantProperty> variantRepository,
            IRepository<VariantRelation> variantRelationRepository, IRepository<VariantSelection> variantSelectionRepository, IApplication application)
        {
            _propertyRepository = propertyRepository;
            _variantRepository = variantRepository;
            _variantRelationRepository = variantRelationRepository;
            _variantSelectionRepository = variantSelectionRepository;
            _application = application;
        }

        public VariantListModel GetVariantListModel(List<int> idList = null, bool includeOptions = false)
        {
            return new VariantListModel()
            {
                VariantProperties = GetVariantProperties(),
                PropertyDisplayModes = _application.Lookup.GetLookups(LookupType.PropertyDisplayModes)
            };
        }

        private List<VariantProperty> GetVariantProperties(List<int> idList = null, bool includeOptions = false)
        {
            List<VariantProperty> result=null;
            try
            {
                if (idList != null && idList.Any())
                {
                    result = _variantRepository.Where(i => idList.Contains(i.ID)).ToList();
                }
                else
                {
                    result = _variantRepository.Where(i => i.ID > 0).ToList();
                }

                if (includeOptions)
                {
                    var parentIDs = result.Select(i => i.ID);
                    var optionList = _propertyRepository.Where(i => i.Type == PropertyType.VariantOption && parentIDs.Contains(i.ParentID)).ToList();
                    if (optionList.Any())
                    {
                        result = result.Select(variant =>
                        {
                            variant.OptionList = optionList.Where(op => op.ParentID == variant.ID).ToList();
                            return variant;
                        }).ToList();
                    }
                }
                return result.OrderBy(i => i.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                _application.Log.Error("VariantService > GetVariantProperties - failed",ex);
                result = null;
            }
            return result;
        }

        public List<VariantProperty> GetCachedVariants()
        {
            var result = _application.Cacher.Get<List<VariantProperty>>(Constants.CK_VariantProperties);
            if (result == null)
            {
                result = _variantRepository.Where(i => i.ID > 0).ToList();
                var parentIDs = result.Select(i => i.ID);
                var optionList = _propertyRepository.Where(i => i.Type == PropertyType.VariantOption && parentIDs.Contains(i.ParentID)).ToList();
                if (optionList.Any())
                {
                    result = result.Select(variant =>
                    {
                        variant.OptionList = optionList.Where(op => op.ParentID == variant.ID).ToList();
                        return variant;
                    }).OrderBy(i => i.SortOrder).ToList();
                }
                _application.Cacher.Add(Constants.CK_VariantProperties, result);
            }
            return result;
        }

        public VariantProperty GetVariantProperty(int propertyID, bool includeOptions = false)
        {
            if (propertyID > 0)
            {
                var result = GetVariantProperties(new List<int>() { propertyID }, includeOptions);
                return result.FirstOrDefault();
            }
            return null;
        }

        public VariantDetailModel GetVariantDetail(int id)
        {
            try
            {
                var property = id > 0 ? GetVariantProperty(id, true) : new VariantProperty();
                var result = new VariantDetailModel
                {
                    VariantProperty = property,
                    ChildList = property.OptionList,
                    PropertyDisplayModes = _application.Lookup.GetLookups(LookupType.PropertyDisplayModes),
                    StatusList = _application.Lookup.GetLookups(LookupType.Status)
                };
                return result;
            }
            catch (Exception ex)
            {
                _application.Log.Error("VariantService > GetVariantDetail - failed for id:{0}",ex,id);
            }
            return null;
        }

        public VariantRelationViewModel GetVariantRelations(int relationID, int relationType, bool useCache = false)
        {
            var result = new VariantRelationViewModel()
            {
                RelationID = relationID,
                RelationType = relationType,
                VariantProperties = useCache ? GetCachedVariants() : GetVariantProperties(null, true)
            };

            var variantsWithStock = result.VariantProperties.Where(i => i.TrackStock);
            foreach (var item in variantsWithStock)
            {
                result.MaxCombinations = Math.Max(result.MaxCombinations, 1) * item.OptionList.Count;
            }

            if (relationID > 0)
            {
                result.Relations = _variantRelationRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType).ToList();

                result.Selections = _variantSelectionRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType)
                                                               .ToList().Select(FillCombinationInfo).ToList();
            }
            return result;
        }

        public VariantSelection GetVariantSelection(int relationID, int relationType, int code)
        {
            if (relationID > 0 && code > 0)
            {
                var result = _variantSelectionRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType && i.Code == code).First();
                return result;
            }
            return null;
        }

        public VariantCheckResult CheckVariantSelection(string selectionString, int relationID, int relationType)
        {
            var result = new VariantCheckResult();
            try
            {
                if (!string.IsNullOrEmpty(selectionString))
                {
                    var selectionInfo = GetVariantInfo(selectionString);
                    var properties = GetCachedVariants();
                    var relations = _variantRelationRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType).ToList();
                    var variantIDs = relations.Select(i => i.VariantID).Distinct().OrderBy(o => o);

                    string stockSelectionString = string.Empty, description = string.Empty, message = string.Empty;
                    decimal price = 0;

                    foreach (var variantID in variantIDs)
                    {
                        var found = false;
                        var variant = properties.FirstOrDefault(i => i.ID == variantID);

                        if (selectionInfo.ContainsKey(variantID) && selectionInfo[variantID] != null && selectionInfo[variantID].Any())
                        {
                            var rels = relations.Where(i => i.VariantID == variantID && (i.VariantValue == 0 || selectionInfo[variantID].Contains(i.VariantValue)));
                            if (rels.Any())
                            {
                                price += rels.Sum(i => i.VariantValue == 0 ? i.Price * selectionInfo[variantID].Count() : i.Price);
                                var options = variant.OptionList.Where(o => selectionInfo[variantID].Contains(o.ID));
                                var optionInfo = options.Select(o =>
                                {
                                    var relation = rels.FirstOrDefault(i => i.VariantID == variantID && (i.VariantValue == 0 || i.VariantValue == o.ID));
                                    return o.Name + (relation != null && relation.Price != 0 ? "(+" + relation.Price.ToPrice() + ")" : "");
                                });
                                description += string.Format("{0}:{1}, ", variant.DisplayName, string.Join(", ", optionInfo));

                                if (variant.TrackStock)
                                {
                                    stockSelectionString += string.Format("{0}:{1};", variantID, string.Join(",", selectionInfo[variantID]));
                                }
                                found = true;
                            }
                        }

                        if (!found && (variant.IsRequired || variant.TrackStock))
                        {
                            message += string.Format("{0} gerekli!", variant.DisplayName);
                        }
                    }

                    result.Price = price;
                    result.Description = description;
                    result.Message = message;

                    if (result.Valid && !string.IsNullOrEmpty(stockSelectionString))
                    {
                        var code = GetVariantCode(stockSelectionString.Trim(seperator_c));
                        result.VariantSelection = GetVariantSelection(relationID, relationType, code);
                    }
                }
                else
                {
                    result.Message = Resource.Basket_VariantSelectionRequired;
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("VariantService > CheckVariantSelection - Fail with : {0}!", ex, selectionString);
            }
            return result;
        }

        public int GetVariantCode(string variantString)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(variantString))
            {
                var info = GetVariantInfo(variantString);
                var subSum = info.Aggregate(1, (current, item) => Math.Max((current * item.Value.Sum(i => i) % 997), 1));
                result += info.Sum(item => (item.Key * item.Value.Sum(i => i)));
                result += subSum + 1;
            }
            return result;
        }

        public Result SaveVariantProperty(VariantProperty model)
        {
            try
            {
                if (model == null) { return Result.Error(); }
                if (model.ID > 0)
                {
                    var variant = _variantRepository.Get(model.ID);
                    if (variant != null)
                    {
                        Mapper.Map(ref variant, model);
                        _variantRepository.Update(variant);
                    }
                    else { throw new Exception("Property not found!"); }
                }
                else
                {
                    _variantRepository.Add(model);
                }
                return Result.Success(model.ID, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > SaveVariantProperty - Fail with : {0}!", model.ID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveVariantRelation(VariantRelation model)
        {
            try
            {
                if (model != null)
                {
                    var relation = _variantRelationRepository.Where(i => i.RelationID == model.RelationID && i.RelationType == model.RelationType
                            && i.VariantID == model.VariantID && i.VariantValue == model.VariantValue).First();

                    if (relation != null)
                    {
                        relation.Price = model.Price;
                        relation.Status = model.Status;
                        _variantRelationRepository.Update(relation);
                        return Result.Success(relation.ID, Resource.General_Success);
                    }
                    else
                    {
                        model.ID = 0;
                        _variantRelationRepository.Add(model);
                        return Result.Success(model.ID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > SaveVariantRelation - Fail with : {0}!", model.ID), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveVariantSelection(VariantSelection model)
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.VariantCombination))
                {
                    model.VariantCombination = model.VariantCombination.Trim(seperator_c);
                    model.Code = GetVariantCode(model.VariantCombination);

                    var selection = _variantSelectionRepository.Where(i => i.RelationID == model.RelationID && i.RelationType == model.RelationType && i.Code == model.Code).First();
                    if (selection != null)
                    {
                        selection.Stock = model.Stock;
                        selection.Price = model.Price;
                        selection.Code = model.Code;
                        _variantSelectionRepository.Update(selection);
                        return Result.Success(selection.ID, Resource.General_Success);
                    }

                    model.ID = 0;
                    _variantSelectionRepository.Add(model);
                    return Result.Success(model.ID, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > SaveVariantSelection - Fail with : {0}, {1}!", model.ID, model.VariantCombination), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteVariantProperty(int id)
        {
            try
            {
                if (id > 0)
                {
                    var subProperties = _propertyRepository.Where(i => i.ParentID == id && i.Type == PropertyType.VariantOption).ToList();
                    if (subProperties != null && subProperties.Any())
                    {
                        return Result.Error("Özelliğin alt özellikleri olduğundan silinemez!");
                    }

                    //TODO#:Relaiton olduğu durum incelenmeli
                    //var relations = _propertyRelationRepository.Find(i => i.PropertyValueID == id);
                    //if (relations != null && relations.Any() && !deleteWithRelations)
                    //{
                    //    return Result.Error("Bu özelliği kullanan ürünler olduğundan özellik silinemiyor!");
                    //}

                    //foreach (var relation in relations)
                    //{
                    //    _propertyRelationRepository.Delete(relation.ID);
                    //    RefreshPropertyRelationInfo(relation);
                    //}

                    _variantRepository.Delete(id);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > DeleteVariantProperty - Fail with : {0}!", id), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteVariantRelation(int id)
        {
            try
            {
                if (id > 0)
                {
                    _variantRelationRepository.Delete(id);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > DeleteVariantRelation - Fail with : {0}!", id), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteVariantSelection(int id)
        {
            try
            {
                if (id > 0)
                {
                    _variantSelectionRepository.Delete(id);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error(string.Format("VariantService > DeleteVariantSelection - Fail with : {0}!", id), ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public bool RefreshVariantStock(VariantSelection selection, decimal quantity)
        {
            try
            {
                if (selection != null && selection.ID > 0)
                {
                    if (selection.Stock == -1)
                    {
                        return true;
                    }

                    var newStock = selection.Stock - quantity;
                    if (newStock < 0) { throw new Exception(); }

                    selection.Stock = newStock;
                    _variantSelectionRepository.Update(selection);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _application.Log.Error("VariantService > RefreshVariantStock - Fail with : {0}-{1}-{2}!", ex, selection.RelationID, selection.Code, selection.VariantCombination);
                throw ex;
            }
            return false;
        }

        private VariantSelection FillCombinationInfo(VariantSelection selection)
        {
            if (!string.IsNullOrEmpty(selection.VariantCombination))
            {
                selection.CombinationInfo = selection.VariantCombination.Split(seperator_s).Select(c => new
                {
                    property = c.Split(':')[0],
                    option = c.Split(':')[1]
                }).ToList<object>();
            }
            return selection;
        }

        private Dictionary<int, List<int>> GetVariantInfo(string selectionString)
        {
            var result = new Dictionary<int, List<int>>();
            if (!string.IsNullOrEmpty(selectionString))
            {
                var parts = selectionString.Split(seperator_s);
                foreach (var part in parts)
                {
                    var variantParts = part.Split(":");
                    var variantID = variantParts[0].Int();

                    if (variantParts.Count() > 1)
                    {
                        var options = variantParts[1].Trim(',').Split(",").Select(i => i.Int()).ToList();
                        result.Add(variantID, options);
                    }
                }
            }
            return result;
        }
    }
}
