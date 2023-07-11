using System;
using System.Collections.Generic;
using Worchart.BL.Cache;
using Worchart.BL.Constants;
using Worchart.BL.Enum;
using Worchart.BL.Log;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Manager
{
    public class ResourceManager : BaseManager<ResourceItem>, IResourceManager
    {
        public ResourceManager(IRepository<ResourceItem> repository, ILogger logger, ICacher cacher) :
            base(repository, logger, cacher)
        {
        }

        public string GetValue(string code, string language = CommonConstants.LanguageEN)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var item = First(i => i.Code == code);
                    return item != null ? item[language] : code;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ResourceManager|Get", "code:{0}, lang:{1}", ex, code, language);
            }
            return code;
        }

        public List<string> GetSupportedLanguages()
        {
            return new List<string>()
            {
                CommonConstants.LanguageTR, CommonConstants.LanguageEN
            };
        }

        protected override OperationResult OnBeforeUpdate(ref ResourceItem existing, ResourceItem model)
        {
            if (existing != null && model != null)
            {
                existing.Code = model.Code;
                existing.Tr = model.Tr;
                existing.En = model.En;
                existing.Scope = model.Scope;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override ManagerCacheMode CacheMode
        {
            get
            {
                return ManagerCacheMode.UseResponsiveCache;
            }
        }
    }
}