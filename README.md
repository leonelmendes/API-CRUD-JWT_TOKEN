# User Authentication API

## Sumário

- [Introdução](#introdução)
- [Instalação](#installation)
  - [Pré-requisitos](#pré-requisitos)
  - [Configurando o banco de dados](#setting-up-the-database)
  - [Executando a API](#running-the-api)
- [Configuração](#configuração)
- [Criação de Tokens JWT](#criação-de-tokens-jwt)
- [Exemplo de Uso](#exemplo-de-uso)

## Introdução

Esta é uma API de autenticação de usuário construída com ASP.NET Core e PostgreSQL. A API usa JWT (JSON Web Tokens) para autenticação e autorização de usuários.

## Instalação

### Pré-requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setting Up the Database

1. Instale o PostgreSQL e crie um novo banco de dados
2. Atualize a string de conexão na pasta `Conexao` no arquivo `Connection.cs` do projeto API para usar variáveis ​​de ambiente:
   ```csharp
   private static string ServidorBD = "Serivdor-Database";
   private static string PortaBD = "Port-Database";
   private static string BD = "Database";
   private static string UsuarioBD = "User-Database";
   private static string SenhaBD = "Password-Database";

   private static string Stringconexao = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", ServidorBD, PortaBD, UsuarioBD, SenhaBD, BD);

### Executando a API
1. **Navegue até o diretório do projeto API**:
     ```bash
    git clone https://github.com/code-hub-dev/API-CRUD-JWT_TOKEN.git
    cd Api

2. **Restaure as dependências da API**:
    ```bash
    dotnet restore

3. **Execute a API**:
     ```bash
     dotnet watch


3. Configure o seu Jwt no arquivo `appsettings.json`.

## Configuração

### `appsettings.json`

Adicione as seguintes configurações ao seu arquivo `appsettings.json`:

```json
  "Jwt": {
    "Key": "your_secret_key_here",
    "Issuer": "your_issuer_here",
    "Audience": "your_audience_here"
  }
```

### Configuração do Middleware de Autenticação

Configure o middleware de autenticação no Program.cs:

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
## Criação de Tokens JWT

Gerando o Token

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
Verificação do Token
Configure a verificação do token no Program.cs conforme mostrado acima na seção Configuração do Middleware de Autenticação.

## Exemplo de Uso
Cliente HTTP
Aqui está um exemplo de como enviar uma solicitação HTTP para a API testando o token usando HttpClient em C#.

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

