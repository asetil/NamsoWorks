using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Util;
using Aware.Util.Log;
using Aware.Util.Model;

namespace Aware.Util.Lookup
{
    public class LookupManager : ILookupManager
    {
        private readonly IRepository<Lookup> _repository;
        private readonly ICacher _cacher;
        private readonly ILogger _logger;

        public LookupManager(IRepository<Lookup> repository, ICacher cacher, ILogger logger)
        {
            _repository = repository;
            _cacher = cacher;
            _logger = logger;
        }

        public List<Lookup> GetLookups(LookupType lookupType, int langID = 0)
        {
            try
            {
                var list = GetLookupList();
                var lookupList = list.Where(i => i.Type == (int)lookupType && (langID == 0 || i.LangID == langID)).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                _logger.Error("LookupManager > GetLookups - failed for:{0}", ex, lookupType);
            }
            return new List<Lookup>();
        }

        public List<Item> GetLookupItems(LookupType lookupType, int langID = 0)
        {
            return GetLookupItems<LookupType>(lookupType, langID);
        }

        public string GetLookupName(LookupType lookupType, int value, int langID = 0)
        {
            var lookup = GetLookup(lookupType, value, langID);
            if (lookup != null) { return lookup.Name;}
            return string.Empty;
        }

        public Lookup GetLookup(LookupType lookupType, int value, int langID = 0)
        {
            return GetLookup<LookupType>(lookupType, value, langID);
        }

        public Lookup GetLookup<T>(T lookupType, int value, int langID = 0) where T:struct, IConvertible
        {
            try
            {
                var type = Convert.ToInt32(lookupType);
                var list = GetLookupList(i => i.Type == type && i.Value == value && (langID == 0 || i.LangID == langID));
                return list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("LookupManager > GetLookupItems - failed for:{0}", ex, lookupType);
            }
            return null;
        }

        public List<Item> GetLookupItems<T>(T lookupType, int langID = 0) where T : struct, IConvertible
        {
            try
            {
                var enumID = Convert.ToInt32(lookupType);
                var list = GetLookupList(i => i.Type == enumID && (langID == 0 || i.LangID == langID));
                var lookupList = list.Select(i => new Item(i.Value, i.Name, string.Empty)).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                _logger.Error("LookupManager > GetLookupItems - failed for:{0}", ex, lookupType);
            }
            return new List<Item>();
        }

       

        private List<Lookup> GetLookupList(Func<Lookup, bool> expression = null)
        {
            try
            {
                var list = _cacher.Get<List<Lookup>>(Constants.CK_Lookups);
                if (list == null)
                {
                    list = _repository.Where(i => i.IsActive).ToList();
                    _cacher.Add(Constants.CK_Lookups, list);
                }

                if (expression != null)
                {
                    return list.Where(expression).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.Error("LookupManager > GetList - failed", ex);
            }
            return new List<Lookup>();
        }
    }
}
