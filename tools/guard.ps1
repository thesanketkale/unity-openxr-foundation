param(
    [switch]$CheckStagedOnly = $false
)

$ErrorActionPreference = "Stop"
$root = (git rev-parse --show-toplevel 2>$null)
if (-not $root) { $root = Get-Location }

$forbiddenPatterns = @(
    "VueXR",
    "Trium Designs",
    "Fudge3d",
    "AKIA[0-9A-Z]{16}",
    "-----BEGIN (RSA |EC |OPENSSH )?PRIVATE KEY-----",
    "ghp_[0-9a-zA-Z]{36}",
    "gho_[0-9a-zA-Z]{36}"
)

$filesToCheck = @()
if ($CheckStagedOnly) {
    $diffFiles = & git diff --cached --name-only --diff-filter=ACMR
    if ($diffFiles) {
        $filesToCheck = $diffFiles -split "`r?`n" | Where-Object { $_ -and (Test-Path (Join-Path $root $_)) }
    }
} else {
    $trackedFiles = & git ls-files
    if ($trackedFiles) {
        $filesToCheck = $trackedFiles -split "`r?`n" | Where-Object { $_ -and (Test-Path (Join-Path $root $_)) }
    }
}

$violations = @()

foreach ($relPath in $filesToCheck) {
    # Skip binary files or git/pipeline internals, or guard itself
    if ($relPath -match "\.(png|jpg|jpeg|ico|dll|so|dylib|bin|zip|tar|gz|aab|apk)$" -or $relPath -like ".git/*" -or $relPath -like ".pipeline/*" -or $relPath -like "tools/guard.*") {
        continue
    }

    $fullPath = Join-Path $root $relPath
    if (-not (Test-Path $fullPath -PathType Leaf)) { continue }

    $content = Get-Content -Path $fullPath -Raw -ErrorAction SilentlyContinue
    if (-not $content) { continue }

    foreach ($pattern in $forbiddenPatterns) {
        if ($content -match $pattern) {
            $violations += "File '$relPath' matched forbidden pattern '$pattern'"
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "GUARD FAILURE: Forbidden strings or credentials detected!" -ForegroundColor Red
    foreach ($v in $violations) {
        Write-Host " - $v" -ForegroundColor Red
    }
    exit 1
}

Write-Host "Guard check passed cleanly: 0 violations." -ForegroundColor Green
exit 0
