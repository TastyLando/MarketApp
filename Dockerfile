# Base SDK Image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy everything at once
COPY . .

# Show directory contents for debugging
RUN ls -la

# Try to restore without specific project file
RUN dotnet restore

# Build
RUN dotnet build --no-restore -c Release

# Publish
RUN dotnet publish --no-restore -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "Market.dll"]