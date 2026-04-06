Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$TunnelName = "cinelog"
$ApiPort    = 5000
$MinioPort  = 9000
$BucketName = "cinelog"
$RepoRoot   = Split-Path $PSScriptRoot -Parent
$InfraDir   = $PSScriptRoot

$tunnelExists = $false
try {
    devtunnel show $TunnelName 2>$null | Out-Null
    $tunnelExists = ($LASTEXITCODE -eq 0)
} catch { }

if (-not $tunnelExists) {
    Write-Host "Creating tunnel '$TunnelName'..."
    devtunnel create $TunnelName --allow-anonymous | Out-Null
    devtunnel port create $TunnelName -p $ApiPort   --protocol http | Out-Null
    devtunnel port create $TunnelName -p $MinioPort --protocol http | Out-Null
    Write-Host "Tunnel created."
} else {
    Write-Host "Tunnel '$TunnelName' found."
}

$logFile = [System.IO.Path]::GetTempFileName()
Write-Host "Starting tunnel (waiting for URLs)..."

$proc = Start-Process devtunnel `
    -ArgumentList "host", $TunnelName `
    -RedirectStandardOutput $logFile `
    -NoNewWindow -PassThru

$apiUrl   = $null
$minioUrl = $null
$deadline = [DateTime]::Now.AddSeconds(30)

while ([DateTime]::Now -lt $deadline) {
    Start-Sleep -Milliseconds 500

    $lines = Get-Content $logFile -ErrorAction SilentlyContinue
    foreach ($line in $lines) {
        if (-not $apiUrl   -and $line -match "https://\S+-$ApiPort\.\S*devtunnels\.ms") {
            $apiUrl = $Matches[0].TrimEnd('/')
        }
        if (-not $minioUrl -and $line -match "https://\S+-$MinioPort\.\S*devtunnels\.ms") {
            $minioUrl = $Matches[0].TrimEnd('/')
        }
    }

    if ($apiUrl -and $minioUrl) { break }
}

if (-not $apiUrl -or -not $minioUrl) {
    Write-Error "Timed out waiting for tunnel URLs. Check devtunnel output at: $logFile"
    $proc | Stop-Process
    exit 1
}

$minioPublicUrl = "$minioUrl/$BucketName"

Write-Host ""
Write-Host "Tunnel URLs:"
Write-Host "  API   : $apiUrl"
Write-Host "  MinIO : $minioUrl"
Write-Host ""

$mobilePath = Join-Path $RepoRoot "src/mobile/CineLog.Mobile/appsettings.Development.json"
@{ ApiBaseUrl = $apiUrl } | ConvertTo-Json | Set-Content -Path $mobilePath -Encoding UTF8
Write-Host "Updated $mobilePath"

$envPath = Join-Path $InfraDir ".env"
$lines = if (Test-Path $envPath) { Get-Content $envPath } else { @() }

$found = $false
$lines = $lines | ForEach-Object {
    if ($_ -match "^MINIO_PUBLIC_URL=") { $found = $true; "MINIO_PUBLIC_URL=$minioPublicUrl" }
    else { $_ }
}
if (-not $found) { $lines += "MINIO_PUBLIC_URL=$minioPublicUrl" }

$lines | Set-Content -Path $envPath -Encoding UTF8
Write-Host "Updated $envPath"

Write-Host ""
Write-Host "Configs updated. If the backend is not running yet, start it in another terminal:"
Write-Host "  docker compose up --build -d"
Write-Host ""

Write-Host ""
Write-Host "Tunnel is running. Press Ctrl+C to stop."
Write-Host ""

Get-Content $logFile
$proc | Wait-Process