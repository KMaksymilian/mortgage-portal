param(
    [string]$Url
)

Write-Host "=== SMOKE TEST ==="
$response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30

if ($response.StatusCode -eq 200) {
    Write-Host "Smoke test OK"
    exit 0
}
else {
    Write-Error "Smoke test FAILED with status code $($response.StatusCode)"
    exit 1
}
