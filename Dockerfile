# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["TshirtMaker.sln", "."]
COPY ["TshirtMaker/*.csproj", "./TshirtMaker/"]
RUN dotnet restore "TshirtMaker/TshirtMaker.csproj"

# Copy everything else
COPY . .

# Publish the app
RUN dotnet publish "TshirtMaker/TshirtMaker.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variable for port (Render provides $PORT)
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the port
EXPOSE $PORT

# Start the app
ENTRYPOINT ["dotnet", "TshirtMaker.dll"]