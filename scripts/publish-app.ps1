param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "./published-app"
)

Write-Host "=== CLEAN OUTPUT ==="
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath | Out-Null

# 1) Build API (LoanHubApi)
Write-Host "=== BUILD LoanHubApi ==="
dotnet publish ./LoanHubApi/LoanHubApi.csproj `
    -c $Configuration `
    -o "$OutputPath/api" `
    /p:EnvironmentName=Production

Write-Host "=== BUILD LoanHub ==="
dotnet publish ./LoanHub/LoanHub.csproj `
    -c $Configuration `
    -o "$OutputPath" `
    /p:EnvironmentName=Production


# 2) Build React frontend
Write-Host "=== BUILD FRONTEND (React) ==="
Push-Location ./frontend
npm install
npm run build
Pop-Location

# 3) Copy frontend build to API wwwroot
Write-Host "=== COPY FRONTEND TO API wwwroot ==="
$frontendBuildPath = "./frontend/build"
$wwwrootPath = "$OutputPath/api/wwwroot"

Copy-Item $frontendBuildPath/* $wwwrootPath -Recurse -Force

Write-Host "=== PUBLISH DONE ==="
Write-Host "OUTPUT PATH: $OutputPath"
