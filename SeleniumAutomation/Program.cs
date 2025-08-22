using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--window-size=1920,1080");

        var baseDir   = AppContext.BaseDirectory;
        var chromeBin = Path.Combine(baseDir, "tools", "chrome-win64", "chrome.exe");
        var driverDir = Path.Combine(baseDir, "tools", "chromedriver-win64");
        if (File.Exists(chromeBin))
            options.BinaryLocation = chromeBin;

        var service = Directory.Exists(driverDir)
            ? ChromeDriverService.CreateDefaultService(driverDir)
            : ChromeDriverService.CreateDefaultService();

        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        using var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(60));

        // DuckDuckGo demo (Google helyett, CAPTCHA nélkül)
        driver.Navigate().GoToUrl("https://duckduckgo.com/");
        var box = driver.FindElement(By.Name("q"));
        box.SendKeys("Selenium WebDriver");
        box.Submit();

        Console.WriteLine("Title: " + driver.Title);
        driver.Quit();
    }
}
