using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Data;
using Aware.ECommerce.Enums;
using Aware.Language.Model;
using Aware.Util.Enums;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.Util.View;
using Aware.Authenticate;
using Aware.ECommerce.Util;
using Aware.Cache;
using Aware.ECommerce;
using Aware.Util;

namespace Aware.Language
{
    public class LanguageService : ILanguageService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IRepository<Model.Language> _languageRepository;
        private readonly IRepository<Model.LanguageValue> _languageValueRepository;
        private readonly IApplication _application;
        private readonly ICacher _cacher;
        private readonly ILogger _logger;

        public LanguageService(ISessionManager sessionManager, IRepository<Model.Language> languageRepository, IRepository<LanguageValue> languageValueRepository,
            IApplication application, ICacher cacher, ILogger logger)
        {
            _sessionManager = sessionManager;
            _languageRepository = languageRepository;
            _languageValueRepository = languageValueRepository;
            _application = application;
            _cacher = cacher;
            _logger = logger;
        }

        public List<Model.Language> GetLanguages(bool onlyActive = false)
        {
            if (onlyActive)
            {
                return _languageRepository.Where(i => i.Status == Statuses.Active).ToList();
            }
            return _languageRepository.GetAll();
        }

        public List<Model.Language> GetCachedLanguages()
        {
            if (_application.Site.UseMultiLanguage)
            {
                var result = _cacher.Get<List<Model.Language>>(Constants.CK_Languages);
                if (result == null)
                {
                    result = GetLanguages(true).OrderBy(i => i.SortOrder).ToList();
                    _cacher.Add(Constants.CK_Languages, result);
                }
                return result;
            }
            return new List<Model.Language>();
        }

        public List<LanguageValue> GetLanguageValues(int relationType, List<int> relationIDs, int languageID = 0)
        {
            if (_application.Site.UseMultiLanguage && relationIDs != null && relationIDs.Any())
            {
                if (languageID == Constants.DefaultLangID)
                {
                    var langCode = _sessionManager.GetCurrentLanguage();
                    var userLanguage = GetCachedLanguages().FirstOrDefault(i => i.Abbreviate == langCode);
                    languageID = userLanguage != null ? userLanguage.ID : languageID;
                }

                if (languageID > 0)
                {
                    return _languageValueRepository.Where(i => i.RelationType == relationType && i.LangID == languageID && relationIDs.Contains(i.RelationID)).ToList();
                }
                return _languageValueRepository.Where(i => i.RelationType == relationType && relationIDs.Contains(i.RelationID)).ToList();
            }
            return new List<LanguageValue>();
        }

        public string GetFieldContent(List<LanguageValue> valueList, int relationID, string fieldName, string defaultValue)
        {
            if (valueList != null && relationID>0)
            {
                var value = valueList.FirstOrDefault(lv => lv.RelationID == relationID && lv.FieldName == fieldName);
                return value != null ? value.Content : defaultValue;
            }
            return defaultValue;
        }

        public LanguageValueDisplayModel GetDisplayModel(int relationID, int relationType)
        {
            var result = new LanguageValueDisplayModel()
            {
                RelationID = relationID,
                RelationType = relationType,
                LanguageList = GetLanguages(true).Where(i => !i.IsDefault).ToList(),
                ValueList = GetLanguageValues(relationType, new List<int>() { relationID }),
                FieldHelper = GetFieldHelper(relationType)
            };
            return result;
        }

        public Result SaveValues(int relationID, int relationType, List<LanguageValue> valueList)
        {
            try
            {
                if (relationID > 0 && valueList != null && valueList.Any())
                {
                    var persistValueList = GetLanguageValues(relationType, new List<int>() { relationID });
                    foreach (var value in valueList)
                    {
                        var persistedValue = persistValueList.FirstOrDefault(i => i.LangID == value.LangID && i.FieldName == value.FieldName);
                        if (persistedValue == null)
                        {
                            value.RelationID = relationID;
                            value.RelationType = relationType;
                            SaveValue(value);
                        }
                        else
                        {
                            persistedValue.Content = value.Content;
                            SaveValue(persistedValue);
                        }
                    }
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageManager > SaveLanguageValue - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result Save(Model.Language model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID <= 0)
                    {
                        _languageRepository.Add(model);
                        return Result.Success(model.ID, Resource.General_Success);
                    }

                    var language = _languageRepository.Get(model.ID);
                    if (language != null)
                    {
                        language.Name = model.Name;
                        language.Abbreviate = model.Abbreviate;
                        language.ImageInfo = model.ImageInfo;
                        language.SortOrder = model.SortOrder;
                        language.Status = model.Status;

                        _languageRepository.Update(language);
                        return Result.Success(language.ID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageService > Save - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveValue(LanguageValue model)
        {
            try
            {
                if (model != null)
                {
                    if (model.ID <= 0)
                    {
                        _languageValueRepository.Add(model);
                        return Result.Success(model.ID, Resource.General_Success);
                    }

                    var languageValue = _languageValueRepository.Get(model.ID);
                    if (languageValue != null)
                    {
                        languageValue.Content = model.Content;
                        _languageValueRepository.Update(languageValue);
                        return Result.Success(languageValue.ID, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageService > SaveValue - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteLanguage(int languageID)
        {
            try
            {
                if (languageID > 0)
                {
                    _languageRepository.Delete(languageID);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageService > DeleteLanguage - Failed for id:{0}", ex, languageID);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SetAsDefault(int languageID)
        {
            try
            {
                if (languageID > 0)
                {
                    var sql = SqlHelper.SetDefaultLanguage(languageID);
                    _languageRepository.ExecuteSp(sql);
                    return Result.Success(null, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageService > SetAsDefault - Failed for id:{0}", ex, languageID);
            }
            return Result.Error(Resource.General_Error);
        }

        public FieldHelper GetFieldHelper(int relationType)
        {
            var result = new FieldHelper(FieldDirection.Horizantal);
            if (relationType == (int)RelationTypes.Product)
            {
                result.Text("Name", "İsim", string.Empty, string.Empty, 50);
                result.TxtArea("ShortDescription", "Kısa Açıklama", string.Empty, string.Empty, 500);
                result.TxtArea("Description", "Açıklama", string.Empty, string.Empty, 24000);
            }
            else if (relationType == (int)RelationTypes.Category)
            {
                result.Text("Name", "İsim", string.Empty, string.Empty, 50);
            }
            return result;
        }
    }
}
