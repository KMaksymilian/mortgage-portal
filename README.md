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