using ArzTalep.BL.Manager;
using Aware.Data.EF;
using Aware.Data.Fake;
using Aware.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace ArzTalep.BL.Dependency
{
    public class LibraryInstaller : AwareDependencyInstaller
    {
        public override void Install(ref IServiceCollection services, DependencySetting dependencySetting)
        {
            base.InstallWithEF<ArzTalep.BL.Data.ArzTalepDbContext>(ref services, dependencySetting);
            services.AddSingleton(typeof(IDbContextFactory), typeof(Data.DbContextFactory));
            services.AddSingleton(typeof(IFakeDataProvider), typeof(Fake.ArzTalepFakeData));
            services.AddSingleton<ICampaignManager, CampaignManager>();
        }
    }
}
