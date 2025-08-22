using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumAutomation.Tests
{
    [Collection("WebDriver collection")]
    public class FileUploadTests
    {
        private readonly WebDriverFixture _fx;
        public FileUploadTests(WebDriverFixture fx) => _fx = fx;

        [Fact(DisplayName = "Fájlfeltöltés működik")]
        public void File_Upload_Works()
        {
            var driver = _fx.Driver;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/upload");

            var tempPath = Path.Combine(Path.GetTempPath(), "upload-demo.txt");
            File.WriteAllText(tempPath, "hello from selenium upload");

            var input = wait.Until(d => d.FindElement(By.Id("file-upload")));
            input.SendKeys(tempPath);
            driver.FindElement(By.Id("file-submit")).Click();

            var heading = wait.Until(d => d.FindElement(By.TagName("h3")));
            Assert.Contains("File Uploaded!", heading.Text);

            _fx.SaveScreenshot("file_upload");
            _fx.SavePageSource("file_upload");
        }
    }
}
