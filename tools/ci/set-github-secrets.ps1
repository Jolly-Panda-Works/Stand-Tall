# Sets the GitHub Actions secrets used by .github/workflows/android-release.yml.
# Run it yourself from the project root (needs the GitHub CLI, logged in with `gh auth login`):
#   powershell -ExecutionPolicy Bypass -File tools/ci/set-github-secrets.ps1
# Passwords are typed into hidden prompts and sent straight to GitHub; nothing is written to disk.

param(
    [string]$Repo = "Jolly-Panda-Works/Stand-Tall",
    [string]$KeystorePath = "F:\Jolly Panda Studio\Mobile Games\com.jollypanda.standtall.keystore",
    [string]$LicensePath = "C:\ProgramData\Unity\Unity_lic.ulf"
)

$ErrorActionPreference = "Stop"

function Read-Secret([string]$prompt) {
    $secure = Read-Host -Prompt $prompt -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }
}

function Set-Secret([string]$name, [string]$value) {
    if ([string]::IsNullOrEmpty($value)) { throw "Value for $name is empty." }
    $value | gh secret set $name --repo $Repo
    Write-Host "  set $name"
}

if (-not (Test-Path $LicensePath)) { throw "Unity license not found: $LicensePath (open Unity Hub once and sign in)." }
if (-not (Test-Path $KeystorePath)) { throw "Keystore not found: $KeystorePath" }

Write-Host "Unity license"
Set-Secret "UNITY_LICENSE" (Get-Content $LicensePath -Raw)
Set-Secret "UNITY_EMAIL" (Read-Host -Prompt "Unity account e-mail")
Set-Secret "UNITY_PASSWORD" (Read-Secret "Unity account password")

Write-Host "Android keystore"
Set-Secret "ANDROID_KEYSTORE_BASE64" ([Convert]::ToBase64String([IO.File]::ReadAllBytes($KeystorePath)))
Set-Secret "ANDROID_KEYSTORE_PASS" (Read-Secret "Keystore password")
$alias = Read-Host -Prompt "Key alias name (Player Settings > Publishing Settings > Alias)"
Set-Secret "ANDROID_KEYALIAS_NAME" $alias
Set-Secret "ANDROID_KEYALIAS_PASS" (Read-Secret "Key alias password")

Write-Host "Done. Push a commit or run the workflow from the Actions tab."
