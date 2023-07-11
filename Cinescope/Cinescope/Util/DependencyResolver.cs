using Aware.Dependency;
using Cinescope.Data;
using Cinescope.Manager;
using Microsoft.Extensions.DependencyInjection;

namespace Cinescope.Web.Util
{
    public class DependencyResolver : AwareDependencyInstaller
    {
        public override void Install(ref IServiceCollection services, DependencySetting dependencySetting)
        {
            base.InstallWithEF<CinescopeDbContext>(ref services, dependencySetting);

            //services.AddSingleton(typeof(IFakeDataProvider), typeof(Fake.ArzTalepFakeData));
            services.AddSingleton<IFilmManager, FilmManager>();
        }
    }
}
