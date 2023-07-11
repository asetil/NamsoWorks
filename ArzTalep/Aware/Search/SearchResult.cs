using Aware.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aware.Search
{
    public class SearchResult<T> where T : IEntity
    {
        public ISearchParams<T> SearchParams { get; set; }
        public List<T> Results { get; set; }
        public bool Success { get; set; }
        public long TotalSize { get; set; }
        public int Took { get; set; }
        public string RequestBody { get; set; }

        private Dictionary<string, List<CommonItem>> _lookupDictionary;
        private Dictionary<string, object> _dataDictionary;

        public bool HasResult
        {
            get
            {
                return Results != null && Results.Any();
            }
        }

        public List<T> ToList()
        {
            return HasResult ? Results : new List<T>();
        }

        public virtual bool HasMore
        {
            get
            {
                return HasResult && SearchParams != null && TotalSize > ((SearchParams.Page + 1) * SearchParams.Size);
            }
        }


        #region Lookups

        public SearchResult<T> AddLookup(string name, CommonItem lookup)
        {
            if (!string.IsNullOrEmpty(name) && lookup != null)
            {
                return AddLookups(name, new List<CommonItem>() { lookup });
            }
            return this;
        }

        public SearchResult<T> AddLookups(string name, List<CommonItem> lookups)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _lookupDictionary = _lookupDictionary ?? new Dictionary<string, List<CommonItem>>();
                if (_lookupDictionary.ContainsKey(name))
                {
                    _lookupDictionary[name] = lookups;
                }
                else
                {
                    _lookupDictionary.Add(name, lookups);
                }
            }
            return this;
        }

        public bool HasLookup(string name)
        {
            return _lookupDictionary != null && !string.IsNullOrEmpty(name) && _lookupDictionary.ContainsKey(name);
        }

        public CommonItem GetLookup(string name, int id, string defaultValue = "")
        {
            var lookups = GetLookups(name);
            if (lookups.Any(i => i.ID == id))
            {
                return lookups.FirstOrDefault(i => i.ID == id);
            }
            return new CommonItem(0, defaultValue);
        }

        public List<CommonItem> GetLookups(string name)
        {
            if (HasLookup(name))
            {
                return _lookupDictionary[name];
            }
            return new List<CommonItem>();
        }

        #endregion

        #region Data
        public SearchResult<T> AddData(string key, object data)
        {
            if (!string.IsNullOrEmpty(key) && data != null)
            {
                _dataDictionary = _dataDictionary ?? new Dictionary<string, object>();
                _dataDictionary.Add(key, data);
            }
            return this;
        }

        public bool HasData(string key)
        {
            return _dataDictionary != null && !string.IsNullOrEmpty(key) && _dataDictionary.ContainsKey(key);
        }

        public TX GetData<TX>(string key)
        {
            try
            {
                if (HasData(key))
                {
                    var value = _dataDictionary[key];
                    return (TX)value;
                }
            }
            catch (Exception ex)
            {

            }
            return default(TX);
        }

        #endregion
    }
}