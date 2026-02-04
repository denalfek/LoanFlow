FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/LoanFlow.Domain/LoanFlow.Domain.csproj src/LoanFlow.Domain/
COPY src/LoanFlow.Configuration/LoanFlow.Configuration.csproj src/LoanFlow.Configuration/
COPY src/LoanFlow.Contracts/LoanFlow.Contracts.csproj src/LoanFlow.Contracts/
COPY src/LoanFlow.Infrastructure/LoanFlow.Infrastructure.csproj src/LoanFlow.Infrastructure/
COPY src/LoanFlow.Workers/LoanFlow.Workers.csproj src/LoanFlow.Workers/
RUN dotnet restore src/LoanFlow.Workers/LoanFlow.Workers.csproj

# Copy everything and build
COPY src/ src/
RUN dotnet build src/LoanFlow.Workers/LoanFlow.Workers.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/LoanFlow.Workers/LoanFlow.Workers.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoanFlow.Workers.dll"]
