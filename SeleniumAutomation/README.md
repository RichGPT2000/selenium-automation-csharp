# From repo root
$env:CHROME_BINARY    = (Resolve-Path .\SeleniumAutomation\tools\chrome-win64\chrome.exe)
$env:CHROMEDRIVER_DIR = (Resolve-Path .\SeleniumAutomation\tools\chromedriver-win64)

# (Highly recommended) neutralize stale system ChromeDriver:
$p = "C:\Program Files\Google\Chrome\Application\chromedriver.exe"
if (Test-Path $p) { Rename-Item $p "chromedriver-old.exe" -Force }

# SeleniumAutomation (C# / .NET 8)

Egyszerű Selenium példa: megnyitja a Google-t, rákeres a **"Selenium WebDriver"**-re, valamint tartalmaz xUnit teszteket.

## Követelmények
- .NET 8 SDK (futtatáshoz/tesztekhez)
- Windows (a mellékelt updater PowerShell szkripthez)
- Internetkapcsolat (ha a `tools/` mappában nincs Chrome + Chromedriver)

## Chrome + Chromedriver kezelése

A projekt **saját Chrome/Chromedriver példányt** tartalmaz a `SeleniumAutomation/tools/` mappában, így nincs szükség globális driver telepítésére.

### Frissítés
A `tools/Update-ChromeForTesting.ps1` script tölti le a megfelelő verziót.

Példa (repo gyökérből):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\SeleniumAutomation\tools\Update-ChromeForTesting.ps1 `
  -Version "139.0.7258.128" -Platform win64 -OutDir .\SeleniumAutomation\tools
```

A script csak akkor tölt le újra, ha szükséges, és mindig kiírja a telepített verziókat.

## Futtatás

```powershell
cd SeleniumAutomation
dotnet run
```

Várható kimenet:
```
Title: Google
```

## Tesztek futtatása

```powershell
cd SeleniumAutomation.Tests
dotnet test
```

A tesztek automatikusan a `tools/` mappából használják a Chrome/Chromedriver példányt, és a teszt kimenetben **HTML + screenshot** artefaktumokat is elmentenek:

```
bin\Debug\net8.0\TestResults\<timestamp>\*.png / *.html
```

## Git ignore

A nagy binárisokat ne tedd Git-be. A `.gitignore` már tartalmazza:

```gitignore
SeleniumAutomation/tools/
!SeleniumAutomation/tools/Update-ChromeForTesting.ps1
!SeleniumAutomation/tools/VERSION.txt
```

Így csak a script és a verziófájl marad a repo-ban.

---

✅ Ez a setup garantálja, hogy **alkalmazás + tesztek ugyanazt a Chrome/Chromedriver buildet** használják, és a régi rendszerbeli `chromedriver.exe` nem fog többé hibát okozni.  
