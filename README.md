# Introduction 
Mortgage Comparer is a web application designed to help user find the most attractive mortgage taking into consideration required factors such as earnings or loan term. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
In order to run the app 

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Technologies
Backend : C# 12, .NET 9 (ASP .NET Core MVC)
Frontend : React.js
Data base : PostgreSQL, Entity Framework
Testing : xUnit

# Naming conventions
Throughout the project we use listed below naming conventions:
1) Class/struct/enum names - PascalCase : 
	`class ProductService`
2) Interfaces - I + PascalCase : 
	`interface IProductServce`
3) Methods - PascalCase : 
	`public void CalculateTotal()`
4) Properties - PascalCase : 
	`public string UserName {get; set;}`
5) Consts - PascalCase : 
	`public const int MaxSize = 100`
6) Local variables - camelCase :  
	`int counter = 0`
7) Method parameters - camelCase : 
	`void SetName(string newName)`
8) Private class fields - _ + camelCase : 
	`private string _connectionString`
9) Asynchronous methods - PascalCase + Async : 
	`async Task GetDataAsync()`
10) Braces statements - Each statement must have braces, braces are always in new lines after statement/code to execute
	```csharp 
	if(statement)
	{
		// code to execute
	}

# Branching Conventions

In our project, we follow the Git branching conventions below to keep code organized and facilitate team collaboration:

## 1. `main`
- The main branch of the project.
- Always contains stable, production-ready code.
- Only merged via pull requests after review.

## 2. `develop`
- Integration branch for new features.
- All feature branches are merged here before moving to `main`.
- Code should be stable but may contain unfinished functionalities.

## 3. `feature/*`
- Branches for new features or major changes.
- Naming: `feature/feature-name`.
- Created from `develop`.
- Once done, merged back into `develop`.

## 4. `bugfix/*`
- Branches for fixing bugs during development.
- Naming: `bugfix/short-bug-description`.
- Created from `develop` and merged back into `develop` after the fix.

## 5. `release/*` (Staging)
- Branches for preparing a new release.
- Naming: `release/vX.Y.Z`.
- Created from `develop` when a release is near.
- Used for final testing, bug fixes, and version updates.
- After testing, merged into both `main` (production) and `develop` (to keep hotfixes).

## 6. `hotfix/*`
- Branches for critical fixes on `main`.
- Naming: `hotfix/short-description`.
- Created from `main` and merged back into both `main` and `develop`.

## Summary
- **Create feature/bugfix/release/hotfix branches from the appropriate base branch.**
- **Merge only via pull requests and after testing.**
- This ensures every team member knows where to implement changes and where to find stable code.
