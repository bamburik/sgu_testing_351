using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
