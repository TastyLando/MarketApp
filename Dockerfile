# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy everything
COPY . ./

# Restore and build
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Market.dll"]
