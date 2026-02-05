FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["LoanFlow.Domain/LoanFlow.Domain.csproj", "LoanFlow.Domain/"]
COPY ["LoanFlow.Configuration/LoanFlow.Configuration.csproj", "LoanFlow.Configuration/"]
COPY ["LoanFlow.Contracts/LoanFlow.Contracts.csproj", "LoanFlow.Contracts/"]
COPY ["LoanFlow.Infrastructure/LoanFlow.Infrastructure.csproj", "LoanFlow.Infrastructure/"]
COPY ["LoanFlow.Workers/LoanFlow.Workers.csproj", "LoanFlow.Workers/"]
RUN dotnet restore "LoanFlow.Workers/LoanFlow.Workers.csproj"

# Copy everything and build
COPY src/ src/
RUN dotnet build LoanFlow.Workers/LoanFlow.Workers.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish LoanFlow.Workers/LoanFlow.Workers.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoanFlow.Workers.dll"]
