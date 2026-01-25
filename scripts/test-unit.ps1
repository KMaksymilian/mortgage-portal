Write-Host "--- Running Unit Tests ---" -ForegroundColor Cyan

$testProjects = "BasicTests/"
cd $testProjects




    dotnet test 
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed in $($project.Name)" -ForegroundColor Red
        exit 1
    }

Write-Host "All tests passed successfully!" -ForegroundColor Green
