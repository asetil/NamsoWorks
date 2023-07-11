using Aware.Manager;
using Aware.Model;
using System.Collections.Generic;

namespace Aware.Util.Lookup
{
    public interface ILookupManager : IBaseManager<Lookup>
    {
        List<Lookup> GetLookups(LookupType lookupType);

        Lookup GetLookup(LookupType lookupType, string value);

        string GetLookupName(LookupType lookupType, string value);

        List<AwareItem> GetLookupItems(LookupType lookupType);
    }
}