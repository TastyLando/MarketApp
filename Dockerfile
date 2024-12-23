# Base SDK Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Debug: Show current directory
RUN pwd && ls -la

# Copy csproj and restore dependencies
COPY ["Market.csproj", "./"]
RUN dotnet restore

# Copy everything else
COPY . ./

# Build and publish
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "Market.dll"]