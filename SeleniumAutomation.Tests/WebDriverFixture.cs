using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumAutomation.Tests
{
    public class WebDriverFixture : IDisposable
    {
        public IWebDriver Driver { get; }
        public string ArtifactDir { get; }

        public WebDriverFixture()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--window-size=1920,1080");
            options.AddExcludedArgument("enable-automation");
            options.AddExcludedArgument("enable-logging");

            var (chromeBin, driverDir) = ResolveChromeAndDriver();
            if (!string.IsNullOrWhiteSpace(chromeBin) && File.Exists(chromeBin))
                options.BinaryLocation = chromeBin;

            RemovePathEntry(@"C:\Program Files\Google\Chrome\Application");

            var service = (!string.IsNullOrWhiteSpace(driverDir) && Directory.Exists(driverDir))
                ? ChromeDriverService.CreateDefaultService(driverDir)
                : ChromeDriverService.CreateDefaultService();

            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            Driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(60));

            ArtifactDir = Path.Combine(AppContext.BaseDirectory, "TestResults", DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(ArtifactDir);
        }

        public void Dispose() => Driver?.Quit();

        private static (string chromeBin, string driverDir) ResolveChromeAndDriver()
        {
            var envChrome = Environment.GetEnvironmentVariable("CHROME_BINARY");
            var envDriver = Environment.GetEnvironmentVariable("CHROMEDRIVER_DIR");
            if (!string.IsNullOrWhiteSpace(envChrome) || !string.IsNullOrWhiteSpace(envDriver))
                return (envChrome ?? "", envDriver ?? "");

            var baseDir = AppContext.BaseDirectory;
            string? cursor = baseDir;
            for (int i = 0; i < 8 && cursor != null; i++)
            {
                var tools = Path.Combine(cursor, "SeleniumAutomation", "tools");
                var chromeCandidate = Path.Combine(tools, "chrome-win64", "chrome.exe");
                var driverCandidate = Path.Combine(tools, "chromedriver-win64");
                if (File.Exists(chromeCandidate) && File.Exists(Path.Combine(driverCandidate, "chromedriver.exe")))
                    return (chromeCandidate, driverCandidate);
                cursor = Directory.GetParent(cursor)?.FullName;
            }
            return ("", "");
        }

        private static void RemovePathEntry(string pathToRemove)
        {
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            var parts = path.Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Where(p => !p.Trim().Equals(pathToRemove, StringComparison.OrdinalIgnoreCase));
            Environment.SetEnvironmentVariable("PATH", string.Join(';', parts));
        }
        public void SaveScreenshot(string name)
        {
            try
            {
                var shot = ((ITakesScreenshot)Driver).GetScreenshot();
                var file = Path.Combine(ArtifactDir, $"{name}_{DateTime.UtcNow:HHmmss}.png");
                shot.SaveAsFile(file);
                Console.WriteLine($"[artifact] screenshot: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[warn] SaveScreenshot failed: {ex.Message}");
            }
        }

        public void SavePageSource(string name)
        {
            try
            {
                var file = Path.Combine(ArtifactDir, $"{name}_{DateTime.UtcNow:HHmmss}.html");
                File.WriteAllText(file, Driver.PageSource);
                Console.WriteLine($"[artifact] html: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[warn] SavePageSource failed: {ex.Message}");
            }
        }

    }

    // Ensure collection definition present in same assembly for fixture injection
    [Xunit.CollectionDefinition("WebDriver collection")]
    public class WebDriverCollectionDefinition : Xunit.ICollectionFixture<WebDriverFixture> { }
}
