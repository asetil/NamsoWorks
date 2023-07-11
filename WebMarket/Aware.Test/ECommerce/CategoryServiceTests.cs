using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Service;
using Aware.ECommerce.Util;
using Aware.Search;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Aware.ECommerce.Enums;
using Aware.File;
using Aware.Language;
using Aware.Language.Model;
using Aware.Util.Enums;
using Aware.Util.Log;
using FluentAssertions;

namespace Aware.Test.ECommerce
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private ICategoryService _categoryService;
        private Mock<IRepository<Category>> _categoryRepository;
        private Mock<ILanguageService> _languageService;
        private Mock<IApplication> _applicationMock;
        private Mock<IFileService> _fileService;

        private Mock<ICacher> _cacher;
        private Mock<ILogger> _logger;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(_fixture.Behaviors.FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _categoryRepository = new Mock<IRepository<Category>>(MockBehavior.Strict);
            _languageService = new Mock<ILanguageService>(MockBehavior.Strict);
            _fileService = new Mock<IFileService>(MockBehavior.Strict);
            _applicationMock = new Mock<IApplication>(MockBehavior.Strict);
            _cacher = new Mock<ICacher>(MockBehavior.Strict);
            _logger = new Mock<ILogger>(MockBehavior.Strict);

            _categoryService = new CategoryService(_languageService.Object, _categoryRepository.Object, _applicationMock.Object, _fileService.Object);
        }

        [Test]
        public void GetCategoryZeroMinusTest()
        {
            var result = _categoryService.GetCategory(0);
            Assert.IsNull(result);

            result = _categoryService.GetCategory(-5);
            Assert.IsNull(result);
        }

        [Test]
        public void GetCategoryTest()
        {
            var languageValues = new List<LanguageValue>();
            var categoryList = _fixture.CreateMany<Category>().ToList();
            var expected = categoryList.FirstOrDefault();

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategory(expected.ID, CategoryHierarchy.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.ID, result.ID);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.ParentID, result.ParentID);
        }

        [Test]
        public void GetCategoryWithAllDescendantsTest()
        {
            var languageValues = new List<LanguageValue>();
            var categoryList = _fixture.CreateMany<Category>().ToList();
            var expected = categoryList.FirstOrDefault();

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategory(expected.ID, CategoryHierarchy.AllDescendants);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.ID, result.ID);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.ParentID, result.ParentID);
        }

        [Test]
        public void GetCategoryWithOnlyChildrenTest()
        {
            var languageValues = new List<LanguageValue>();
            var categoryList = _fixture.CreateMany<Category>().ToList();
            var expected = categoryList.FirstOrDefault();

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategory(expected.ID, CategoryHierarchy.OnlyChildren);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.ID, result.ID);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.ParentID, result.ParentID);
        }


        [Test]
        public void GetCategoriesCachedTest()
        {
            var level = 0;
            var expected = _fixture.CreateMany<Category>().ToList();
            var languageValues = new List<LanguageValue>();

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(expected);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategories(level);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetCategoriesNotCachedTest()
        {
            var level = 0;
            var expected = _fixture.CreateMany<Category>().ToList();
            var languageValues = new List<LanguageValue>();
            var searchHelper = _fixture.Create<SearchHelper<Category>>();
            searchHelper.SetRepository(_categoryRepository.Object);

            List<Category> cacheResult = null;
            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(cacheResult);
            _cacher.Setup(i => i.Add(Constants.CK_CategoryList, expected, 1440)).Returns(true);
            _categoryRepository.Setup(i => i.Where(It.IsAny<Expression<Func<Category, bool>>>())).Returns(searchHelper);
            _categoryRepository.Setup(i => i.Find(searchHelper)).Returns(expected);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategories(level);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetCategoriesNotCached_WithPredicateTest()
        {
            var level = 0;
            var expected = _fixture.CreateMany<Category>().ToList();
            var languageValues = new List<LanguageValue>();
            var searchHelper = _fixture.Create<SearchHelper<Category>>();
            searchHelper.SetRepository(_categoryRepository.Object);

            List<Category> cacheResult = null;
            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(cacheResult);
            _cacher.Setup(i => i.Add(Constants.CK_CategoryList, expected, 1440)).Returns(true);
            _categoryRepository.Setup(i => i.Where(It.IsAny<Expression<Func<Category, bool>>>())).Returns(searchHelper);
            _categoryRepository.Setup(i => i.Find(searchHelper)).Returns(expected);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategories(level, i => i.Status == Statuses.Active);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Where(i => i.Status == Statuses.Active).ToList(), result);
        }

        [Test]
        public void GetCategoriesNotCached_WithLevelTest()
        {
            var level = _fixture.Create<int>();
            var expected = _fixture.CreateMany<Category>().ToList();
            var languageValues = new List<LanguageValue>();
            var searchHelper = _fixture.Create<SearchHelper<Category>>();
            searchHelper.SetRepository(_categoryRepository.Object);

            List<Category> cacheResult = null;
            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(cacheResult);
            _cacher.Setup(i => i.Add(Constants.CK_CategoryList, expected, 1440)).Returns(true);
            _categoryRepository.Setup(i => i.Where(It.IsAny<Expression<Func<Category, bool>>>())).Returns(searchHelper);
            _categoryRepository.Setup(i => i.Find(searchHelper)).Returns(expected);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetCategories(level);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Where(i => i.Level == level).ToList(), result);
        }

        [Test]
        public void GetMainCategories_WithCached_Test()
        {
            var expected = _fixture.CreateMany<Category>().ToList();
            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_MainCategories)).Returns(expected);

            var result = _categoryService.GetMainCategories();

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetMainCategories_WithNonCached_Test()
        {
            var languageValues = new List<LanguageValue>();
            List<Category> cacheResult = null;

            var categoryList = _fixture.CreateMany<Category>().ToList();
            categoryList.FirstOrDefault().SortOrder = "001";
            categoryList.FirstOrDefault().Status = Statuses.Active;

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_MainCategories)).Returns(cacheResult);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Add(Constants.CK_MainCategories, It.IsAny<List<Category>>(), 1440)).Returns(true);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);

            var result = _categoryService.GetMainCategories();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CategorySaveNullTest()
        {
            var result = _categoryService.Save(null);
            Assert.IsNull(result);
        }

        [Test]
        public void CategorySave_NewWithNoParent_Test()
        {
            var category = _fixture.Create<Category>();
            category.ID = 0;
            category.ParentID = 0;

            var categoryList = _fixture.CreateMany<Category>().ToList();
            var languageValues = new List<LanguageValue>();

            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);
            _categoryRepository.Setup(i => i.Add(category, true));

            var result = _categoryService.Save(category);
            Assert.IsNotNull(result);
        }

        [Test]
        public void CategorySave_NewWithParent_Test()
        {
            var categoryList = _fixture.CreateMany<Category>(10).ToList();
            for (var i = 1; i < 4; i++)
            {
                categoryList[i].ParentID = categoryList[0].ID;
            }

            var category = _fixture.Create<Category>();
            category.ID = 0;
            category.ParentID = categoryList[0].ID;

            var languageValues = new List<LanguageValue>();
            _applicationMock.Setup(i => i.Cacher).Returns(_cacher.Object);
            _cacher.Setup(i => i.Get<List<Category>>(Constants.CK_CategoryList)).Returns(categoryList);
            _cacher.Setup(i => i.Get<List<LanguageValue>>(Constants.CK_CategoryLanguageValues)).Returns(languageValues);
            _categoryRepository.Setup(i => i.Add(category, true));

            var result = _categoryService.Save(category);
            Assert.IsNotNull(result);
        }

        [Test]
        public void CategorySaveTest()
        {
            var category = _fixture.Create<Category>();
            var categoryList = _fixture.CreateMany<Category>().ToList();
            var expected = categoryList.FirstOrDefault();

            //_categoryRepository.Setup(p => p.Find(It.IsAny<Expression<Func<Category, bool>>>(), null, null, "")).Returns(categoryList);
            _categoryRepository.Setup(p => p.Update(expected, true));
            var result = _categoryService.Save(category);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ID, expected.ID);
        }

        //[Test]
        //public void CategorySaveNotFoundTest()
        //{
        //    var category = _fixture.Create<Category>();
        //    var expected = new List<Category>() { };

        //    _logger.Setup(i => i.Error(It.IsAny<string>(), It.IsAny<Exception>()));
        //    _categoryRepository.Setup(p => p.Find(It.IsAny<Expression<Func<Category, bool>>>(), null, null, "")).Returns(expected);
        //    var result = _categoryService.Save(category);

        //    Assert.IsNull(result);
        //}




        //[Test]
        //public void DeleteCategoryTest()
        //{
        //    IEnumerable<Product> productList = null;
        //    var category = _fixture.Create<Category>();
        //    var hierarchy = new List<Category>() { category };

        //    //_productRepository.Setup(p => p.Find(It.IsAny<Expression<Func<Product, bool>>>(), null, null, "")).Returns(productList);
        //    _categoryRepository.Setup(i => i.Find(It.IsAny<Expression<Func<Category, bool>>>(), null, null, "")).Returns(hierarchy);
        //    _categoryRepository.Setup(c => c.Delete(category.ID)).Returns(true);

        //    var result = _categoryService.Delete(category.ID);

        //    Assert.IsNotNull(result);
        //    Assert.True(result.OK);
        //}

        [Test]
        public void DeleteCategoryZeroTest()
        {
            var categoryID = 0;
            var result = _categoryService.Delete(categoryID);

            Assert.IsNotNull(result);
            Assert.False(result.OK);
        }

        [Test, Ignore("")]
        public void DeleteCategoryHasProductTest()
        {
            var categoryID = _fixture.Create<int>();
            var productList = _fixture.CreateMany<Product>();
            //_productRepository.Setup(p => p.Find(It.IsAny<Expression<Func<Product, bool>>>(), null, null, "")).Returns(productList);

            var result = _categoryService.Delete(categoryID);

            Assert.IsNotNull(result);
            Assert.False(result.OK);
        }

        //[Test]
        //public void DeleteCategoryHasSubCategoryTest()
        //{
        //    List<Product> productList = null;
        //    var category = _fixture.Create<Category>();
        //    var hierarchy = _fixture.CreateMany<Category>().Select(i =>
        //    {
        //        i.ParentID = category.ID;
        //        return i;
        //    }).ToList();
        //    hierarchy.Add(category);


        //    //_productRepository.Setup(p => p.Find(It.IsAny<Expression<Func<Product, bool>>>(), null, null, "")).Returns(productList);
        //    _categoryRepository.Setup(i => i.Find(It.IsAny<Expression<Func<Category, bool>>>(), null, null, "")).Returns(hierarchy);
        //    var result = _categoryService.Delete(category.ID);

        //    Assert.IsNotNull(result);
        //    Assert.False(result.OK);
        //}

        [TearDown]
        public void TearDown()
        {
            _applicationMock.VerifyAll();
            _languageService.VerifyAll();
            _fileService.VerifyAll();
            _categoryRepository.VerifyAll();
        }
    }
}
