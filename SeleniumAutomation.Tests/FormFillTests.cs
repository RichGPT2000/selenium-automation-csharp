using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumAutomation.Tests
{
    [Collection("WebDriver collection")]
    public class FormFillTests
    {
        private readonly WebDriverFixture _fx;
        public FormFillTests(WebDriverFixture fx) => _fx = fx;

        [Fact(DisplayName = "Login űrlap – hibás belépés üzenet")]
        public void Form_Fill_Shows_Error_On_Invalid_Login()
        {
            var driver = _fx.Driver;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");

            var user = wait.Until(d => d.FindElement(By.Id("username")));
            var pass = driver.FindElement(By.Id("password"));
            var btn = driver.FindElement(By.CssSelector("button[type='submit']"));

            user.SendKeys("wrong");
            pass.SendKeys("credentials");
            btn.Click();

            var flash = wait.Until(d => d.FindElement(By.Id("flash")));
            Assert.Contains("Your username is invalid!", flash.Text);

            _fx.SaveScreenshot("login_invalid");
            _fx.SavePageSource("login_invalid");
        }
    }
}
