# Use official .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project and restore dependencies
COPY backend/ ./backend/
RUN dotnet restore ./backend/WebApi/WebApi.csproj

# Publish
RUN dotnet publish ./backend/WebApi/WebApi.csproj -c Release -o /out

# Use the official ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Set environment variable for ASP.NET Core to listen on port 5246
ENV ASPNETCORE_URLS=http://+:5246
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false \
    DOTNET_USE_POLLING_FILE_WATCHER=true

# Expose the correct port
EXPOSE 5246

# Set the entrypoint to run the app.
ENTRYPOINT ["dotnet", "WebApi.dll"]
