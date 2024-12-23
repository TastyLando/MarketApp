FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Debug: List directory contents
RUN ls -la

# Copy csproj first and restore dependencies
COPY Market.csproj ./
RUN ls -la
RUN dotnet restore "Market.csproj" --verbosity detailed

# Copy everything else
COPY . ./
RUN ls -la

# Build and publish
RUN dotnet publish "Market.csproj" -c Release -o /app
RUN ls -la /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy published files from build image
COPY --from=build /app ./

# Verify files copied correctly
RUN ls -la

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "Market.dll"]