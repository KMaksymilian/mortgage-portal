cd Backend

Write-Host "🔄 Restoring dependencies..."
dotnet restore

Write-Host "🏗️ Building project in Release mode..."
dotnet build --configuration Release

Write-Host "🧪 Running unit tests..."
# Run tests and store the exit code
dotnet test --configuration Release `
    --logger "trx;LogFileName=UnitTests.trx" `
    --results-directory "$env:AGENT_TEMPDIRECTORY\TestResults\Unit"

$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Host "❌ Some unit tests failed."
    exit $exitCode   # This will fail the pipeline
} else {
    Write-Host "✅ All unit tests passed!"
}
