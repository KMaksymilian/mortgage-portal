param(
    [string]$ConnectionString
)

Write-Host "=== RUNNING EF CORE MIGRATIONS ==="

dotnet tool install --global dotnet-ef --version 9.0.0 -q

$env:ConnectionStrings__DefaultConnection = $ConnectionString

dotnet ef database update `
    --project ./MortgageComparer/MortgageComparer.csproj `
    --startup-project ./MortgageComparer/MortgageComparer.csproj `
    --connection $ConnectionString

Write-Host "=== MIGRATIONS DONE ==="
