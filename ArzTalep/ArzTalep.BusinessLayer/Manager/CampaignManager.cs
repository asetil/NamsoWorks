using ArzTalep.BL.Model;
using Aware.Data;
using Aware.Manager;
using Aware.Util.Cache;
using Aware.Util.Log;
using Aware.BL.Model;

namespace ArzTalep.BL.Manager
{
    public class CampaignManager : BaseManager<Campaign>, ICampaignManager
    {
        public CampaignManager(IRepository<Campaign> repository, IAwareLogger logger, IAwareCacher cacher) : base(repository, logger, cacher)
        {
        }

        protected override OperationResult<Campaign> OnBeforeUpdate(ref Campaign existing, Campaign model)
        {
            throw new System.NotImplementedException();
        }
    }
}
