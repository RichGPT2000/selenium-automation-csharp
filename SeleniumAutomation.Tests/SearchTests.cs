using System;
using Xunit;

namespace SeleniumAutomation.Tests
{
    public class SearchTests : IClassFixture<WebDriverFixture>
    {
        private readonly WebDriverFixture _fx;
        public SearchTests(WebDriverFixture fx) => _fx = fx;

        [Fact(DisplayName = "DuckDuckGo keresés működik")]
        public void DuckDuckGo_Kereses_Mukodik()
        {
            _fx.Driver.Navigate().GoToUrl("https://duckduckgo.com/");
            var box = _fx.Driver.FindElement(OpenQA.Selenium.By.Name("q"));
            box.SendKeys("Selenium WebDriver");
            box.Submit();

            Assert.Contains("Selenium WebDriver", _fx.Driver.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}
