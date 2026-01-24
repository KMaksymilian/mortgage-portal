param(
    [string]$ConnectionString
)

Write-Host "=== RUNNING EF CORE MIGRATIONS ==="

dotnet tool install --global dotnet-ef --version 9.0.0 -q

$env:ConnectionStrings__DefaultConnection = $ConnectionString

dotnet ef database update `
    --project ./src/LoanHub/LoanHub.csproj `
    --startup-project ./src/LoanHub/LoanHub.csproj `
    --connection $ConnectionString

Write-Host "=== MIGRATIONS DONE ==="
