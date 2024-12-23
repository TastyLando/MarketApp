FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file
COPY ["Market.csproj", "./"]

# Restore dependencies
RUN dotnet restore "Market.csproj"

# Copy everything else
COPY . .

# Build the project
RUN dotnet build "Market.csproj" -c Release -o /app/build

# Publish the project
RUN dotnet publish "Market.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "Market.dll"]