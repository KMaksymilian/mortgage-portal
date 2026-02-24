# Introduction 
Mortgage Comparer is a web application designed to help users find the most attractive mortgage, taking into consideration required factors such as earnings or loan term. The system consists of a frontend application, a main backend service, and an internal banking API simulator, all communicating securely.

# Technologies
* Backend: C# 12, .NET 9 (ASP.NET Core Web API / MVC)
* Frontend: React.js
* Database: PostgreSQL, Entity Framework Core
* Testing: xUnit
* Infrastructure: Docker & Docker Compose

# Getting Started

## 1. Clone the repository
git clone https://github.com/KMaksymilian/mortgage-portal.git
cd mortgage-portal

## 2. Set up Environment Variables (.env)
For security reasons, sensitive data like database passwords and JWT keys are not stored in the source code. 
Create a new file named `.env` in the root directory of the project (at the same level as `compose.yaml`) and populate it with the following structure. Make sure your JWT keys are at least 32 characters long.

DB_USER=postgres  
DB_PASSWORD=YourStrongPassword123!  
DB_NAME=TestDB  

JWT_KEY=YourSuperSecretJwtKeyForApp11111  
JWT_SECRET_KEY=YourAnotherSuperSecretJwtKeyForApp  

## 3. Build and Run the Application
Open your terminal in the root directory and run the following command to build the images and start the containers:

docker-compose up --build

## 4. Access the Services
Once the Docker containers are successfully built and running, you can access the application through your browser:

* Frontend : http://localhost:3000

To remove the containers run `docker-compose down` to cleanly remove the containers.
