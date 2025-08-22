using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumAutomation.Tests
{
    [Collection("WebDriver collection")]
    public class GoogleSearchTests
    {
        private readonly WebDriverFixture _fx;

        public GoogleSearchTests(WebDriverFixture fx) => _fx = fx;

        [Fact(DisplayName = "Google keresés működik")]
        public void Google_Search_Works()
        {
            var driver = _fx.Driver;
            driver.Navigate().GoToUrl("https://www.google.com/");
            try { driver.FindElement(By.Id("L2AGLb")).Click(); } catch { }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var box = wait.Until(d => d.FindElement(By.Name("q")));
            box.SendKeys("Selenium WebDriver");
            box.SendKeys(Keys.Enter);
            wait.Until(d => d.Title.Contains("Selenium", StringComparison.OrdinalIgnoreCase));

            _fx.SaveScreenshot("google_search");
            _fx.SavePageSource("google_search");
        }
    }
}
