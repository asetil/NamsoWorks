using System;
using System.Collections.Generic;
using Worchart.BL.Manager;
using Worchart.BL.Model;

namespace Worchart.BL.Lookup
{
    public interface ILookupManager : IBaseManager<Lookup>
    {
        Lookup GetLookup(LookupType lookupType, int value, string language);
        string GetLookupName(LookupType lookupType, int value, string language);

        List<Lookup> GetLookups(LookupType lookupType, string language);
        List<Item> GetLookupItems(LookupType lookupType, string language);
    }
}