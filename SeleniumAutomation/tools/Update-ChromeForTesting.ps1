param(
    [string]$Version = "139.0.7258.128",
    [ValidateSet("win64", "linux64", "mac-x64", "mac-arm64")]
    [string]$Platform = "win64",
    [string]$OutDir = "",
    [switch]$Force
)

# Determine output directory robustly
if ([string]::IsNullOrWhiteSpace($OutDir)) {
    if ($PSCommandPath) { $OutDir = Split-Path -Parent $PSCommandPath }
    elseif ($PSScriptRoot) { $OutDir = $PSScriptRoot }
    else { $OutDir = (Get-Location).Path }
}

try { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 } catch {}
$prevProgress = $global:ProgressPreference
$global:ProgressPreference = 'SilentlyContinue'

$baseUrl = "https://storage.googleapis.com/chrome-for-testing-public"

$chromeFolder = Join-Path $OutDir "chrome-$Platform"
$driverFolder = Join-Path $OutDir "chromedriver-$Platform"
$versionFile = Join-Path $OutDir "VERSION.txt"

# Expected binary names
$chromeBin = if ($Platform -eq "win64") { Join-Path $chromeFolder "chrome.exe" } else { Join-Path $chromeFolder "chrome" }
$driverBin = if ($Platform -eq "win64") { Join-Path $driverFolder "chromedriver.exe" } else { Join-Path $driverFolder "chromedriver" }

# Current state
$haveChrome = Test-Path $chromeBin
$haveDriver = Test-Path $driverBin
$currentVersion = if (Test-Path $versionFile) { (Get-Content $versionFile -Raw).Trim() } else { "" }

Write-Host "==> Target: $OutDir  |  Requested: $Version ($Platform)" -ForegroundColor Cyan
if (-not $Force -and $haveChrome -and $haveDriver -and ($currentVersion -eq $Version)) {
    Write-Host "Already up-to-date ✅" -ForegroundColor Green
}
else {
    # Clean if needed
    if ($Force -or ($currentVersion -ne $Version) -or -not ($haveChrome -and $haveDriver)) {
        if (Test-Path $chromeFolder) { Write-Host "Cleaning: $chromeFolder"; Remove-Item $chromeFolder -Recurse -Force }
        if (Test-Path $driverFolder) { Write-Host "Cleaning: $driverFolder"; Remove-Item $driverFolder -Recurse -Force }
    }

    # Remove leftovers
    $chromeZip = Join-Path $OutDir "chrome-$Platform.zip"
    $driverZip = Join-Path $OutDir "chromedriver-$Platform.zip"
    if (Test-Path $chromeZip) { Remove-Item $chromeZip -Force }
    if (Test-Path $driverZip) { Remove-Item $driverZip -Force }

    # URLs
    $chromeUri = "$baseUrl/$Version/$Platform/chrome-$Platform.zip"
    $driverUri = "$baseUrl/$Version/$Platform/chromedriver-$Platform.zip"

    # Download Chrome
    if (-not (Test-Path $chromeFolder)) {
        Write-Host "Download: $chromeUri"
        Invoke-WebRequest -Uri $chromeUri -OutFile $chromeZip
        Expand-Archive -Path $chromeZip -DestinationPath $OutDir -Force
        Remove-Item $chromeZip -Force
    }
    # Download Chromedriver
    if (-not (Test-Path $driverFolder)) {
        Write-Host "Download: $driverUri"
        Invoke-WebRequest -Uri $driverUri -OutFile $driverZip
        Expand-Archive -Path $driverZip -DestinationPath $OutDir -Force
        Remove-Item $driverZip -Force
    }

    # Write version file
    Set-Content -Path $versionFile -Value $Version
}

# Verify binaries
if (-not (Test-Path $chromeBin)) { throw "Missing Chrome binary: $chromeBin" }
if (-not (Test-Path $driverBin)) { throw "Missing Chromedriver binary: $driverBin" }

# Show versions
Write-Host "==> Installed binaries:" -ForegroundColor Yellow
Write-Host "Chrome binary:      $chromeBin"
& $chromeBin --version
Write-Host "Chromedriver binary: $driverBin"
& $driverBin --version

# Restore progress preference
$global:ProgressPreference = $prevProgress
