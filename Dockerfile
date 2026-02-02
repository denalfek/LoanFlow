FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/LoanFlow.Domain/LoanFlow.Domain.csproj src/LoanFlow.Domain/
COPY src/LoanFlow.Configuration/LoanFlow.Configuration.csproj src/LoanFlow.Configuration/
COPY src/LoanFlow.Infrastructure/LoanFlow.Infrastructure.csproj src/LoanFlow.Infrastructure/
COPY src/LoanFlow.Api/LoanFlow.Api.csproj src/LoanFlow.Api/
RUN dotnet restore src/LoanFlow.Api/LoanFlow.Api.csproj

# Copy everything and build
COPY src/ src/
RUN dotnet build src/LoanFlow.Api/LoanFlow.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/LoanFlow.Api/LoanFlow.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoanFlow.Api.dll"]
