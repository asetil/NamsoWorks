using System;
using System.Collections.Generic;
using Worchart.BL.Model;

namespace Worchart.BL.Manager
{
    public interface IResourceManager : IBaseManager<ResourceItem>
    {
        string GetValue(string code, string language = "");
        List<string> GetSupportedLanguages();
    }
}
