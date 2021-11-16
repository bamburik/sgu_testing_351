using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.citilink.ru/");
            driver.FindElement(By.XPath("//button[@data-label='Я согласен']")).Click();
        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.CssSelector(".dy--PopularCategoryMain__link")).Click();
            driver.FindElement(By.CssSelector(".CatalogCategoryCard__link")).Click();
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@data-meta-name='FilterPriceGroup__input-min']")).Clear();
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@data-meta-name='FilterPriceGroup__input-min']")).SendKeys("1000");
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@data-meta-name='FilterPriceGroup__input-max']")).Clear();
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@data-meta-name='FilterPriceGroup__input-max']")).SendKeys("10000");
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@data-meta-name='FilterPriceGroup__input-max']")).SendKeys(Keys.Enter);
            new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                .Until(x => driver.FindElements(By.CssSelector(".StickyOverlayLoader__preloader")).Any());

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => driver.FindElements(By.CssSelector(".StickyOverlayLoader__preloader")).Count == 0);

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'ProductCardVerticalLayout__wrapper-cart')]//*[contains(@class,'ProductCardVerticalPrice__price-current_current-price')]"))
                .Select(webPrice => webPrice.Text.Trim()).ToArray<string>(), s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 1000 && actualPrice <= 10000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 1000 and less or equal than 10000"));
        }

        [Test]
        public void TestTooltipText()
        {
            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector(".IconFont_cart_add"))).Build().Perform();
            Assert.IsTrue(driver.FindElements(By.CssSelector(".Hint__block_active")).Any(),
                "Tooltip has not appeared.");
            Assert.AreEqual("Добавить в корзину", driver.FindElement(By.CssSelector(".Hint__block_active")).Text.Trim(),
                "Tooltip has not appeared.");
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.CssSelector(".AuthPopup__button")).Click();
            driver.FindElement(By.CssSelector(".AuthGroup__tab-sign-up")).Click();
            driver.FindElement(By.CssSelector(".js--SignUp__input-name__container-input")).SendKeys("Test");
            driver.FindElement(By.CssSelector(".js--SignUp__input-email__container-input")).SendKeys("vfbdhjsk57bs442@mail.ru");
            Assert.IsFalse(driver.FindElements(By.XPath("//button[contains(@class, 'SignUp__button-confirm-phone') and not(@disabled)]")).Any(),
                "Phone number confirmation button is enabel when phone number input has no value.");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
