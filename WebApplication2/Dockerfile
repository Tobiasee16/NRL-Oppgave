# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Konteksten er WebApplication2/, så csproj ligger her
COPY ["WebApplication2.csproj", "."]
RUN dotnet restore "WebApplication2.csproj"

# Kopier resten og publiser
COPY . .
RUN dotnet publish "WebApplication2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication2.dll"]
