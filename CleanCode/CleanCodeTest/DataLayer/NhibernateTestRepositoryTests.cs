using System.Reflection;
using Aware.Dependency;
using CleanFramework.Business.Dependency;
using CleanFramework.Data.Repository;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CleanCodeTest.DataLayer
{
    [TestFixture]
    public class NhibernateTestRepositoryTests
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var dependencySetting = WindsorBootstrapper.Dependency()
                .SetCache(CacheMode.Empty)
                //.SetIntercepter()
                .SetORM(ORMType.Nhibernate)
                .SetAssembly(Assembly.GetAssembly(typeof(CleanFramework.Business.Model.Entry)));


            WindsorBootstrapper.Create(null);
            WindsorBootstrapper.InstallFor(new AwareInstaller(dependencySetting));
            WindsorBootstrapper.InstallFor(new CleanCodeInstaller());
            //WindsorBootstrapper.Container.Register(Component.For<IAuthorizationCustomerUserInfo>().ImplementedBy<AuthorizationCustomerUserInfo>().LifestyleTransient());
        }

        [Test,Ignore("")]
        public void GetWithPageTest()
        {
            var repository=new NhibernateTestRepository();

            var page1=repository.GetWithPage(1,5);
            var page2= repository.GetWithPage(2,5);
            var page12 = repository.GetWithPage(1, 10);
        }

        [Test,Ignore("")]
        public void GetEntryListWithCountTest()
        {
            var repository = new NhibernateTestRepository();

            long count = 0;
            var page1 = repository.GetEntryListWithCount(1, 5,out count);
        }

        [Test, Ignore("")]
        public void GetPagedDataTest()
        {
            var repository = new NhibernateTestRepository();

            long count = 0;
            var page1 = repository.GetPagedData(3, 8, out count);
        }
    }
}
