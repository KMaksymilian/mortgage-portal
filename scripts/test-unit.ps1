cd Backend
Write-Host "🔄 Restoring dependencies..."
dotnet restore
Write-Host "🏗️ Building project in Release mode..."
dotnet build --configuration Release
Write-Host "🧪 Running unit tests..."
dotnet test --configuration Release --logger trx --results-directory "$env:AGENT_TEMPDIRECTORY\TestResults\Unit"
