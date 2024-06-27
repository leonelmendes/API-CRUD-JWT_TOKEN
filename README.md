# User Authentication API

## Sumário

- [Introduction](#introduction)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Setting up the database](#setting-up-the-database)
  - [Running the API](#running-the-api)
- [Configuration](#configuration)
- [Creation of JWT Tokens](#jwt-token-creation)
- [Usage Example](#usage-example)

## Introduction

This is a user authentication API built with ASP.NET Core and PostgreSQL. The API uses JWT (JSON Web Tokens) for user authentication and authorization.

## Installation

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setting Up the Database

1. Install PostgreSQL and create a new database
2. Update the connection string in the `Connection` folder in the `Connection.cs` file of the API project to use environment variables:
   ```csharp
   private static string ServidorBD = "Serivdor-Database";
   private static string PortaBD = "Port-Database";
   private static string BD = "Database";
   private static string UsuarioBD = "User-Database";
   private static string SenhaBD = "Password-Database";

   private static string Stringconexao = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", ServidorBD, PortaBD, UsuarioBD, SenhaBD, BD);

### Running the API
1. **Navigate to the API project directory**:
     ```bash
    git clone https://github.com/code-hub-dev/API-CRUD-JWT_TOKEN.git
    cd Api

2. **Restore API dependencies**:
    ```bash
    dotnet restore

3. **Run the API**:
     ```bash
     dotnet watch


3. Configure your Jwt in the `appsettings.json` file.

## Configuration

### `appsettings.json`

Add the following settings to your `appsettings.json` file:

```json
  "Jwt": {
    "Key": "your_secret_key_here",
    "Issuer": "your_issuer_here",
    "Audience": "your_audience_here"
  }
```

### Authentication Middleware Configuration

Configure authentication middleware in Program.cs:

```csharp
        using Microsoft.AspNetCore.Authentication.JwtBearer;
      using Microsoft.IdentityModel.Tokens;
      using System.Text;
      
      var builder = WebApplication.CreateBuilder(args);
      var services = builder.Services;
      var configuration = builder.Configuration;
      
      services.AddControllers();
      
      var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
      
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidAudience = configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(key)
              };
          });
      
      var app = builder.Build();
      
      app.UseAuthentication();
      app.UseAuthorization();
      
      app.MapControllers();
      
      app.Run();
```
## Creation of JWT Tokens

Generating the Token

```csharp
        using System;
        using System.IdentityModel.Tokens.Jwt;
        using System.Security.Claims;
        using System.Text;
        using Microsoft.IdentityModel.Tokens;
        
        public string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "your_issuer_here",
                Audience = "your_audience_here",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
```
Token Verification
Configure token verification in Program.cs as shown above in the Authentication Middleware Configuration section.

## Usage Example
HTTP Client
Here is an example of how to send an HTTP request to the API by testing the token using HttpClient in C#.

```csharp
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  
  public async Task GetUserTokenAsync(string token)
  {
      using (var httpClient = new HttpClient())
      {
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
  
          var response = await httpClient.GetAsync("https://yourapiurl.com/api/Get_User_Token");
  
          if (response.IsSuccessStatusCode)
          {
              var responseData = await response.Content.ReadAsStringAsync();
              Console.WriteLine("Usuário autenticado com sucesso.");
              Console.WriteLine(responseData);
          }
          else
          {
              Console.WriteLine($"Erro: {response.StatusCode}");
          }
      }
  }
```

