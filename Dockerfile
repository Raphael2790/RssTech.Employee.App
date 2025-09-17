# Use a imagem base do .NET 9 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restore dependencies
COPY *.sln .
COPY RssTech.Employee.Api/*.csproj ./RssTech.Employee.Api/
COPY RssTech.Employee.Application/*.csproj ./RssTech.Employee.Application/
COPY RssTech.Employee.Application.UnitTests/*.csproj ./RssTech.Employee.Application.UnitTests/
COPY RssTech.Employee.Common/*.csproj ./RssTech.Employee.Common/
COPY RssTech.Employee.Data/*.csproj ./RssTech.Employee.Data/
COPY RssTech.Employee.Domain/*.csproj ./RssTech.Employee.Domain/
COPY RssTech.Employee.Domain.UnitTests/*.csproj ./RssTech.Employee.Domain.UnitTests/
COPY RssTech.Employee.Ioc/*.csproj ./RssTech.Employee.Ioc/

RUN dotnet restore

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR /app/RssTech.Employee.Api
RUN dotnet publish -c Release -o out

# Use a imagem runtime do .NET 9 para execução
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar os arquivos publicados
COPY --from=build /app/RssTech.Employee.Api/out .

# Expor a porta 8080
EXPOSE 8080

# Definir variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando para executar a aplicação
ENTRYPOINT ["dotnet", "RssTech.Employee.Api.dll"]