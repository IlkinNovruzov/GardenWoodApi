# Build aşaması 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GardenWoodAPI/GardenWoodAPI.csproj", "GardenWoodAPI/"]
RUN dotnet restore "GardenWoodAPI/GardenWoodAPI.csproj"
COPY . .
WORKDIR "/src/GardenWoodAPI"
RUN dotnet publish "GardenWoodAPI.csproj" -c Release -o /app/publish

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GardenWoodAPI.dll"]
