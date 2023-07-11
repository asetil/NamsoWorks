using System;
using System.Collections.Generic;
using System.Linq;
using Aware.BL.Model;
using Aware.Data;
using Aware.Manager;
using Aware.Model;
using Aware.Util.Cache;
using Aware.Util.Constants;
using Aware.Util.Enum;
using Aware.Util.Log;

namespace Aware.Util.Lookup
{
    public class LookupManager : BaseManager<Lookup>, ILookupManager
    {
        public LookupManager(IRepository<Lookup> repository, IAwareCacher cacher, IAwareLogger logger) : base(repository, logger, cacher)
        {

        }

        public List<Lookup> GetLookups(LookupType lookupType)
        {
            try
            {
                var lookupList = SearchBy(i => i.Type == (int)lookupType).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookups", "{0}", ex, lookupType);
            }
            return new List<Lookup>();
        }

        public Lookup GetLookup(LookupType lookupType, string value)
        {
            try
            {
                var type = Convert.ToInt32(lookupType);
                var lookup = First(i => i.Type == type && i.Value == value);
                return lookup;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookupItems", "{0}", ex, lookupType);
            }
            return null;
        }

        public string GetLookupName(LookupType lookupType, string value)
        {
            var lookup = GetLookup(lookupType, value);
            if (lookup != null) { return lookup.Name; }
            return string.Empty;
        }

        public List<AwareItem> GetLookupItems(LookupType lookupType)
        {
            try
            {
                var lookupList = SearchBy(i => i.Type == (int)lookupType).ToList()
                    .Select(i => new AwareItem(i.Value, i.Name)).ToList();
                return lookupList;
            }
            catch (Exception ex)
            {
                Logger.Error("LookupManager|GetLookupItems", "lookupType:{0}", ex, lookupType);
            }
            return new List<AwareItem>();
        }

        protected override ManagerCacheMode CacheMode => ManagerCacheMode.UseResponsiveCache;

        protected override OperationResult<Lookup> OnBeforeCreate(ref Lookup model)
        {
            if (model != null && model.IsValid())
            {
                return Success(model);
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }

        protected override OperationResult<Lookup> OnBeforeUpdate(ref Lookup existing, Lookup model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.Status = model.Status;
                existing.Order = model.Order;
                existing.Type = model.Type;
                existing.Value = model.Value;
                existing.ExtraData = model.ExtraData;
                return Success(existing);
            }
            return Failed(ResultCodes.Error.CheckParameters);
        }
    }
}
