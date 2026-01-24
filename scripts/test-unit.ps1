Write-Host "--- Running Unit Tests ---" -ForegroundColor Cyan

$testProjects = Get-ChildItem -Recurse -Filter "*.Tests.csproj" -Path "./tests"

if ($null -eq $testProjects) {
    Write-Error "No test projects found in /tests folder!"
    exit 1
}

foreach ($project in $testProjects) {
    Write-Host "Testing: $($project.Name)" -ForegroundColor Blue
    dotnet test $project.FullName --configuration Release --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed in $($project.Name)" -ForegroundColor Red
        exit 1
    }
}

Write-Host "All tests passed successfully!" -ForegroundColor Green
