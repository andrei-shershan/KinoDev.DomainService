FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY ["KinoDev.DomainService.sln", "./"]
COPY ["src/KinoDev.DomainService.WebApi/KinoDev.DomainService.WebApi.csproj", "src/KinoDev.DomainService.WebApi/"]
COPY ["src/KinoDev.DomainService.Infrastructure/KinoDev.DomainService.Infrastructure.csproj", "src/KinoDev.DomainService.Infrastructure/"]
COPY ["src/KinoDev.DomainService.Domain/KinoDev.DomainService.Domain.csproj", "src/KinoDev.DomainService.Domain/"]
COPY ["tests/KinoDev.DomainService.Domain.UnitTets/KinoDev.DomainService.Domain.UnitTets.csproj", "tests/KinoDev.DomainService.Domain.UnitTets/"]
COPY ["tests/KinoDev.DomainService.Infrastructure.UnitTests/KinoDev.DomainService.Infrastructure.UnitTests.csproj", "tests/KinoDev.DomainService.Infrastructure.UnitTests/"]
COPY ["tests/KinoDev.DomainService.WebApi.UnitTests/KinoDev.DomainService.WebApi.UnitTests.csproj", "tests/KinoDev.DomainService.WebApi.UnitTests/"]
RUN dotnet restore "src/KinoDev.DomainService.WebApi/KinoDev.DomainService.WebApi.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/src/KinoDev.DomainService.WebApi"
RUN dotnet build "KinoDev.DomainService.WebApi.csproj" -c Release -o /app/build

# Add a stage for running tests
# FROM build AS testrunner
# WORKDIR /src/tests/FoodExpress.Api.UnitTests
# RUN dotnet test "FoodExpress.Api.UnitTests.csproj" --logger:trx

FROM build AS publish
RUN dotnet publish "KinoDev.DomainService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KinoDev.DomainService.WebApi.dll"]
