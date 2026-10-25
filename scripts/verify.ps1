param(
    [ValidateSet("fast", "full")][string]$Mode = "full"
)

$PSScriptRootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $PSScriptRootPath

$candidates = @(
    "C:\Program Files\Git\bin\bash.exe",
    (Join-Path $env:ProgramFiles "Git\bin\bash.exe"),
    (Join-Path ${env:ProgramFiles(x86)} "Git\bin\bash.exe"),
    (Join-Path $env:LOCALAPPDATA "Programs\Git\bin\bash.exe")
) | Where-Object { $_ -and (Test-Path $_) }

$bash = $candidates | Select-Object -First 1

if (-not $bash) {
    $gitCmd = Get-Command git.exe -ErrorAction SilentlyContinue
    if ($gitCmd) {
        $parent = Split-Path -Parent (Split-Path -Parent $gitCmd.Source)
        $testPath = Join-Path $parent "bin\bash.exe"
        if (Test-Path $testPath) { $bash = $testPath }
    }
}

if (-not $bash) {
    Write-Error "Git Bash not found. Please ensure Git for Windows is installed."
    exit 1
}

$verifySh = (Join-Path $PSScriptRootPath "verify.sh").Replace("\", "/")
& $bash $verifySh $Mode
exit $LASTEXITCODE
