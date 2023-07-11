using System;
using System.Collections.Generic;
using System.Linq;
using Worchart.BL.Cache;
using Worchart.BL.Constants;
using Worchart.BL.Enum;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Lookup
{
    public class LookupManager : BaseManager<Lookup>, ILookupManager
    {
        private readonly IResourceManager _resourceManager;
        public LookupManager(IRepository<Lookup> repository, ICacher cacher, ILogger logger, IResourceManager resourceManager)
        : base(repository, logger, cacher)
        {
            _resourceManager = resourceManager;
        }

        public List<Lookup> GetLookups(LookupType lookupType, string language)
        {
            try
            {
                var list = GetLookupList(null, language);
                var lookupList = list.Where(i => i.Type == lookupType).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookups", "LookupType:{0}", ex, lookupType);
            }
            return new List<Lookup>();
        }

        public string GetLookupName(LookupType lookupType, int value, string language)
        {
            var lookup = GetLookup(lookupType, value, language);
            if (lookup != null) { return lookup.Name; }
            return string.Empty;
        }

        public Lookup GetLookup(LookupType lookupType, int value, string language)
        {
            try
            {
                var list = GetLookupList(i => i.Type == lookupType && i.Value == value, language);
                return list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookupItems", "LookupType:{0}", ex, lookupType);
            }
            return null;
        }

        public List<Item> GetLookupItems(LookupType lookupType, string language)
        {
            try
            {
                var list = GetLookupList(i => i.Type == lookupType, language);
                var lookupList = list.Select(i => new Item(i.Value, i.Name)).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookupItems", "LookupType:{0}", ex, lookupType);
            }
            return new List<Item>();
        }

        private List<Lookup> GetLookupList(Func<Lookup, bool> expression = null, string currentLanguage = CommonConstants.LanguageEN)
        {
            try
            {
                var cacheKey = string.Format(CommonConstants.LookupListKey, currentLanguage);

                var list = Cacher.Get<List<Lookup>>(cacheKey);
                if (list == null)
                {
                    list = Repository.Where(i => i.IsActive).ToList();
                    var langList = _resourceManager.GetSupportedLanguages();
                    foreach (var lang in langList)
                    {
                        var convertedList = list.Select(l =>
                        {
                            var clone = l.Clone();
                            clone.Name = _resourceManager.GetValue(l.Name, lang);
                            return clone;
                        }).OrderBy(l => l.Order).ToList();

                        cacheKey = string.Format(CommonConstants.LookupListKey, lang);
                        Cacher.Add(cacheKey, convertedList, CommonConstants.DailyCacheTime);

                        if (currentLanguage == lang)
                        {
                            list = convertedList;
                        }
                    }
                }

                if (expression != null)
                {
                    return list.Where(expression).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager > GetList - failed", ex);
            }
            return new List<Lookup>();
        }

        protected override OperationResult OnBeforeUpdate(ref Lookup existing, Lookup model)
        {
            if (existing != null && model != null)
            {
                existing.Name = model.Name;
                existing.Order = model.Order;
                existing.Type = model.Type;
                existing.IsActive = model.IsActive;
                existing.Value = model.Value;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override Lookup GetClone(Lookup model)
        {
            if (model != null)
            {
                return model.Clone();
            }
            return base.GetClone(model);
        }


        protected override ManagerCacheMode CacheMode
        {
            get { return ManagerCacheMode.UseResponsiveCache; }
        }
    }
}
