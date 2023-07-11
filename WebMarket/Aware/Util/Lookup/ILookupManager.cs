using System;
using System.Collections.Generic;
using Aware.Util.Model;

namespace Aware.Util.Lookup
{
    public interface ILookupManager
    {
        Lookup GetLookup(LookupType lookupType, int value, int langID = 0);
        Lookup GetLookup<T>(T lookupType, int value, int langID = 0) where T : struct, IConvertible;
        string GetLookupName(LookupType lookupType, int value, int langID = 0);

        List<Lookup> GetLookups(LookupType lookupType, int langID = 0);
        List<Item> GetLookupItems(LookupType lookupType, int langID = 0);
        List<Item> GetLookupItems<T>(T lookupType, int langID = 0) where T:struct, IConvertible;
    }
}