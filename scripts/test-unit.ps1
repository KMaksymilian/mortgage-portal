Write-Host "--- Running Unit Tests ---" -ForegroundColor Cyan

$testProjects = "BasicTests/"
cd $testProjects

if ($null -eq $testProjects) {
    Write-Error "No test projects found in /tests folder!"
    exit 1
}

foreach ($project in $testProjects) {
    Write-Host "Testing: $($project.Name)" -ForegroundColor Blue
    dotnet test 
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed in $($project.Name)" -ForegroundColor Red
        exit 1
    }
}

Write-Host "All tests passed successfully!" -ForegroundColor Green
