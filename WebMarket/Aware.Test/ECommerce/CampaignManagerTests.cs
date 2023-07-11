using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aware.Authenticate.Model;
using Aware.Cache;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Service;
using Aware.Mail;
using Aware.Test.Util;
using Aware.Util.Log;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Aware.ECommerce.Enums;
using Aware.Authenticate;
using Aware.ECommerce;
using Aware.File;

namespace Aware.Test.ECommerce
{
    [TestFixture]
    public class CampaignServiceTests
    {
        private ICampaignService _campaignService;
        private Mock<ISessionManager> _sessionManager;
        private Mock<IRepository<Campaign>> _campaignRepository;
        private Mock<IRepository<Discount>> _discountRepository;
        private Mock<IRepository<User>> _userRepository;
        private Mock<IApplication> _applicationMock;
        private Mock<IMailService> _mailService;
        private Mock<ICategoryService> _categoryService;

        private IFixture _fixture;
        private CampaignTestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(_fixture.Behaviors.FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            HttpContextManager.SetHttpContext();

            _testHelper = new CampaignTestHelper(_fixture);
            _sessionManager = new Mock<ISessionManager>(MockBehavior.Strict);
            _campaignRepository = new Mock<IRepository<Campaign>>(MockBehavior.Strict);
            _discountRepository = new Mock<IRepository<Discount>>(MockBehavior.Strict);
            _userRepository = new Mock<IRepository<User>>(MockBehavior.Strict);
            _categoryService = new Mock<ICategoryService>(MockBehavior.Strict);
            _applicationMock = new Mock<IApplication>(MockBehavior.Strict);
            _mailService = new Mock<IMailService>(MockBehavior.Strict);

            _campaignService = new CampaignService(_sessionManager.Object, _campaignRepository.Object, _discountRepository.Object, _userRepository.Object,
                _mailService.Object, _categoryService.Object, _applicationMock.Object);
        }

        [Test]
        public void NullBasketTest()
        {
            Basket basket = null;
            TestCampaign(basket, 0, 0);
        }

        [Test]
        public void EmptyBasketTest()
        {
            var basket = new Basket();
            var campaigns = _fixture.CreateMany<Campaign>().ToList();
            //_cacher.Setup(i => i.Get<List<Campaign>>(It.IsAny<string>())).Returns(campaigns);

            TestCampaign(basket, 0, 0);
        }

        //[Test]
        //public void OrderedBasketTest()
        //{
        //    var basket = _testHelper.GetBasket(3, 100);
        //    basket.Status = Enums.Statuses.OrderedBasket;
        //    var discounts = _fixture.CreateMany<Discount>().ToList();

        //    _discountRepository.Setup(i => i.Find(It.IsAny<Expression<Func<Discount, bool>>>(),null,null,"")).Returns(discounts);
        //    TestCampaign(basket, discounts.Sum(i => i.Total), discounts.Count());
        //}

        [Test]
        public void FixedAmountCampaignTest() //80 TL üzeri alışverişlerde 12 TL indirim
        {
            var campaign = _testHelper.GetCampaign("all_amount", 12, 80, 4);
            var expected = new KeyValuePair<int, decimal>(1, -1);
           
            BaseTest(campaign, expected, 5, 100);
        }

        [Test] //250 TL üzeri alışverişlerde 20 TL indirim
        public void FixedAmountCampaignNotApplicableTest()
        {
            var campaign = _testHelper.GetCampaign("all_amount", 20, 250, 0);

            var expected = new KeyValuePair<int, decimal>(0, 0);
            BaseTest(campaign, expected, 3, 200);
        }

        [Test] //250 TL üzeri alışverişlerde 20 TL indirim, En az 2 ürün
        public void FixedAmountCampaignQuantityTest()
        {
            var campaign = _testHelper.GetCampaign("all_rate", 20, 200, 2);

            var expected = new KeyValuePair<int, decimal>(1, -1);
            BaseTest(campaign, expected, 3, 250);

            expected = new KeyValuePair<int, decimal>(0, 0);
            BaseTest(campaign, expected, 1, 250, 0);
        }

        [Test] //200 TL üzeri alışverişlerde %20 indirim
        public void FixedRateCampaignTest()
        {
            var campaign = _testHelper.GetCampaign("all_rate", 20, 200, 0);

            var expected = new KeyValuePair<int, decimal>(1, -1);
            BaseTest(campaign, expected, 3, 250);
        }

        [Test] //200 TL üzeri alışverişlerde %20 indirim, Sadece X marketinde
        public void FixedRateCampaignStoreTest()
        {
            var campaign = _testHelper.GetCampaign("all_rate", 20, 200, 1);
            campaign.FilterInfo = "sid=5,6";

            var expected = new KeyValuePair<int, decimal>(1, -1);
            BaseTest(campaign, expected, 3, 250, 5);

            expected = new KeyValuePair<int, decimal>(0, 0);
            BaseTest(campaign, expected, 3, 250, 1);
        }

        [Test] //500 TL üzeri alışverişlerde %40 indirim, Sadece X marketinde Şu şu kategorilerde
        public void FixedRateCampaignCategoryTest()
        {
            var campaign = _testHelper.GetCampaign("all_rate", 40, 500, 1);
            campaign.FilterInfo = "sid=5,6&cid=2,6,9";

            List<int> relatedCategories = new List<int>() { 2, 6, 9 };
            _categoryService.Setup(i => i.GetRelatedCategoryIDs(It.IsAny<List<int>>())).Returns(relatedCategories);

            var expected = new KeyValuePair<int, decimal>(1, -1);
            BaseTest(campaign, expected, 15, 5000, 6, "basket_filter_test");

            expected = new KeyValuePair<int, decimal>(0, 0);
            BaseTest(campaign, expected, 15, 5000, 6, "basket_filter_fail_test");
        }

        [Test] //100 TL Üzeri Alışverişlerde Kargo Bedava
        public void FreeShippingCampaignTest()
        {
            var discount = _fixture.Create<int>() % 100;
            var campaign = _testHelper.GetCampaign("all_shipping", discount, 100, 0);

            var expected = new KeyValuePair<int, decimal>(1, 0);
            var result = BaseTest(campaign, expected, 15, 5000, 6);

            Assert.AreEqual(DiscountType.Shipping, result.FirstOrDefault().DiscountType);
            Assert.AreEqual(discount, result.FirstOrDefault().Factor);
        }

        [Test] //100 TL Üzeri Alışverişlerde Kargo Sadece 1 TL
        public void FixedPriceShippingCampaignTest()
        {
            var discount = _fixture.Create<int>() % 10;
            var campaign = _testHelper.GetCampaign("all_fixedpriceshipping", discount, 100, 0);

            var expected = new KeyValuePair<int, decimal>(1, 0);
            var result = BaseTest(campaign, expected, 15, 5000, 6);

            Assert.AreEqual(DiscountType.FixedPriceShipping, result.FirstOrDefault().DiscountType);
            Assert.AreEqual(discount, result.FirstOrDefault().Factor);
        }

        private IEnumerable<Discount> BaseTest(Campaign campaign, KeyValuePair<int, decimal> expected, int itemCount, decimal basketTotal, int storeID = 0, string basketType = "")
        {
            itemCount = Math.Max(1, itemCount);
            var minPrice = basketTotal / itemCount - 5;
            var maxPrice = basketTotal / itemCount + 5;
            Basket basket;

            var searchHelper = _fixture.Create<Search.SearchHelper<Discount>>();
            searchHelper.SetRepository(_discountRepository.Object);

            if (!string.IsNullOrEmpty(basketType))
            {
                basket = _testHelper.GetBasketWithName(basketType, itemCount, maxPrice, minPrice, storeID);
            }
            else
            {
                basket = _testHelper.GetBasket(itemCount, maxPrice, minPrice, storeID);
            }

            var discount = expected.Value;
            if (discount < 0)
            {
                discount = campaign.Discount;
                if (campaign.DiscountType == DiscountType.Rate)
                {
                    discount = basket.GrossTotal * campaign.Discount / 100;
                }
            }

            var campaigns = new List<Campaign>() { campaign };
            List<Discount> usedCoupons = null;

            //_cacher.Setup(i => i.Get<List<Campaign>>(It.IsAny<string>())).Returns(campaigns);
            _discountRepository.Setup(i => i.Where(It.IsAny<Expression<Func<Discount, bool>>>())).Returns(searchHelper);
            _discountRepository.Setup(i => i.Find(searchHelper)).Returns(usedCoupons);

            return TestCampaign(basket, discount, expected.Key);
        }

        private IEnumerable<Discount> TestCampaign(Basket basket, decimal expectedDiscount, int expectedDiscountCount)
        {
            var result = _campaignService.CalculateBasketDiscounts(basket);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedDiscount, result.Sum(i => i.Total));
            Assert.AreEqual(expectedDiscountCount, result.Count());
            return result;
        }

        [TearDown]
        public void TearDown()
        {
            _campaignRepository.VerifyAll();
            _discountRepository.VerifyAll();
            _userRepository.VerifyAll();
            _mailService.VerifyAll();
            _categoryService.VerifyAll();
        }
    }
}
