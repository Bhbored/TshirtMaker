# Use .NET 8.0 (more stable with Blazor Server)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TshirtMaker.csproj", "./"]
RUN dotnet restore "TshirtMaker.csproj"

COPY . .
RUN dotnet publish "TshirtMaker.csproj" -c Release -o /app/publish

# Use ASP.NET 8.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway automatically sets PORT environment variable
ENV ASPNETCORE_URLS="http://+:$PORT"
ENV ASPNETCORE_ENVIRONMENT="Production"

EXPOSE $PORT

ENTRYPOINT ["dotnet", "TshirtMaker.dll"]
