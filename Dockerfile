# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first
COPY ["Market.csproj", "./"]

# Restore dependencies
RUN dotnet restore "./Market.csproj"

# Copy everything else
COPY . .

# Build
RUN dotnet build "Market.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "Market.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Market.dll"]
