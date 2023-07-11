using System.Linq;
using Aware.Authenticate;
using Aware.Data;
using Aware.ECommerce;
using CleanFramework.Business.Model;
using CleanFramework.Business.Service;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Aware.ECommerce.Interface;

namespace CleanCodeTest.ServiceTests
{
    [TestFixture]
    public class EntryServiceTests
    {
        private IFixture _fixture;
        private IEntryService _entryService;
        private Mock<IRepository<Entry>> _entryRepository;
        private Mock<ICategoryService> _categoryService;
        private Mock<IUserService> _userService;
        private Mock<IApplication> _applicationMock;


        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _entryRepository = new Mock<IRepository<Entry>>(MockBehavior.Strict);
            _categoryService = new Mock<ICategoryService>(MockBehavior.Strict);
            _userService = new Mock<IUserService>(MockBehavior.Strict);
            _applicationMock = new Mock<IApplication>(MockBehavior.Strict);
            _entryService = new EntryService(_entryRepository.Object,_categoryService.Object, _applicationMock.Object, _userService.Object);
        }

        [Test]
        public void GetListTest()
        {
            var expected = _fixture.CreateMany<Entry>().ToList();
            _entryRepository.Setup(i => i.GetAll()).Returns(expected);

            var result = _entryService.Search(null,false,false);
            Assert.AreEqual(expected, result);
        }
    }
}
