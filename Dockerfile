# Use the official .NET 10.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file and project file(s) first for better caching
# Adjust the path if your .csproj file has a different name or is in a subdirectory
COPY ["TshirtMaker/TshirtMaker.csproj", "TshirtMaker/"]
RUN dotnet restore "TshirtMaker/TshirtMaker.csproj"

# Copy the rest of the source code
COPY . .

# Publish the application to the 'app/publish' directory
RUN dotnet publish "TshirtMaker/TshirtMaker.csproj" -c Release -o /app/publish --no-restore

# Use the official .NET 10.0 ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy the published output from the 'build' stage
COPY --from=build /app/publish .

# Set the environment variable for ASP.NET Core to listen on the port Render provides
# Render automatically sets the $PORT environment variable
ENV ASPNETCORE_URLS=http://+:$PORT

# Inform Render that the app listens on port 80 (will be mapped to $PORT internally)
EXPOSE $PORT

# Define the command to run your application
ENTRYPOINT ["dotnet", "TshirtMaker.dll"]
