# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

# Set working directory
WORKDIR /source

# Copy only necessary files for restore
COPY ["Market.csproj", "./"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the files
COPY . .

# Clean and rebuild
RUN dotnet clean
RUN dotnet build --no-restore
RUN dotnet publish --no-restore -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app from builder
COPY --from=builder /app . 

# Set environment variable for dynamic port
ENV ASPNETCORE_URLS=http://+:${PORT}

# Expose default port
EXPOSE 443

# Entry point
ENTRYPOINT ["dotnet", "Market.dll"]
