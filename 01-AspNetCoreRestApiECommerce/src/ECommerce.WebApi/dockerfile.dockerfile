# ---------- Stage 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first (better layer caching)
COPY ["ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["ECommerce.Application/ECommerce.Application.csproj", "ECommerce.Application/"]
COPY ["ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]
COPY ["ECommerce.WebApi/ECommerce.WebApi.csproj", "ECommerce.WebApi/"]

# Restore only the WebApi project (and its dependencies)
RUN dotnet restore "ECommerce.WebApi/ECommerce.WebApi.csproj"

# Copy everything else
COPY . .

# Build in Release mode
WORKDIR "/src/ECommerce.WebApi"
RUN dotnet build "ECommerce.WebApi.csproj" -c Release -o /app/build

# ---------- Stage 2: Publish ----------
FROM build AS publish
RUN dotnet publish "ECommerce.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- Stage 3: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ECommerce.WebApi.dll"]