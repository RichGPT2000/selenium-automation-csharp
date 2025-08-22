# SeleniumAutomation (C# / .NET 8)

Egyszerű Selenium példa: megnyitja a **DuckDuckGo** keresőt és rákeres a **"Selenium WebDriver"** kifejezésre, valamint tartalmaz xUnit teszteket.

> Miért DuckDuckGo? A Google a Selenium-os (automatizált) kereséseket sokszor reCAPTCHA oldalra irányítja (“unusual traffic”), ezért a tesztek megbízhatósága érdekében DuckDuckGo-t használunk.

## Gyors indítás – környezeti változók (repo gyökérből)

```powershell
$env:CHROME_BINARY    = (Resolve-Path .\SeleniumAutomation\tools\chrome-win64\chrome.exe)
$env:CHROMEDRIVER_DIR = (Resolve-Path .\SeleniumAutomation\tools\chromedriver-win64)

# (Ajánlott) régi, rendszerszintű ChromeDriver kiiktatása, hogy ne hijack-eljen
$p = "C:\Program Files\Google\Chrome\Application\chromedriver.exe"
if (Test-Path $p) { Rename-Item $p "chromedriver-old.exe" -Force }
```

## Követelmények
- .NET 8 SDK (futtatáshoz/tesztekhez)
- Windows (a mellékelt updater PowerShell szkripthez)
- Internetkapcsolat (ha a `tools/` mappában nincs Chrome + Chromedriver)

## Chrome + Chromedriver kezelése

A projekt **saját Chrome/Chromedriver példányt** tart a `SeleniumAutomation/tools/` mappában, így nincs szükség globális driver telepítésére.

### Frissítés
A `tools/Update-ChromeForTesting.ps1` script tölti le a megfelelő verziót. A script idempotens (csak akkor tölt le, ha kell) és **mindig kiírja a telepített verziókat**.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\SeleniumAutomation\tools\Update-ChromeForTesting.ps1 `
  -Version "139.0.7258.128" -Platform win64 -OutDir .\SeleniumAutomation\tools
```

## Futtatás

```powershell
cd SeleniumAutomation
dotnet run
```

Várható kimenet:
```
Title: DuckDuckGo
```

## Tesztek

A tesztek automatikusan a `tools/` mappából használják a Chrome/Chromedriver példányt, és a kimenetben **HTML + screenshot** artefaktumokat is elmentenek:

```
SeleniumAutomation.Tests\bin\Debug\net8.0\TestResults\<timestamp>\*.png / *.html
```

Futtatás:

```powershell
cd SeleniumAutomation.Tests
dotnet test
```

## Git ignore

A nagy binárisokat ne tedd Git-be. A `.gitignore`-ba tedd be (ha még nincs):

```gitignore
SeleniumAutomation/tools/
!SeleniumAutomation/tools/Update-ChromeForTesting.ps1
!SeleniumAutomation/tools/VERSION.txt
```

Így csak a script és a verziófájl marad a repo-ban.

---

✅ Ezzel a beállítással az **alkalmazás és a tesztek ugyanazt a Chrome/Chromedriver buildet** használják, és nem kapunk többé Google reCAPTCHA-t a kereséses példára.
