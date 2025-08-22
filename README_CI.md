## CI (GitHub Actions)

A repo tartalmaz egy workflow-t (`.github/workflows/ci.yml`), amely:
- letölti a **Chrome for Testing** + **Chromedriver** párost (Linux/Windows),
- beállítja a `CHROME_BINARY` és `CHROMEDRIVER_DIR` környezeti változókat,
- futtatja a `SeleniumAutomation.Tests` projektet headless módban (Linuxon `xvfb-run`),
- feltölti az eredményeket artefaktként (TRX + screenshot/HTML).

Lokális parancsok:
```bash
dotnet restore
dotnet build --configuration Release
dotnet test SeleniumAutomation.Tests --no-build
```
