FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR /src
COPY ["src/RoomLocator/RoomLocator.Api/RoomLocator.Api.csproj", "RoomLocator.Api/"]
COPY ["src/RoomLocator/RoomLocator.Business/RoomLocator.Business.csproj", "RoomLocator.Business/"]
COPY ["src/RoomLocator/RoomLocator.Persistence/RoomLocator.Persistence.csproj", "RoomLocator.Persistence/"]
RUN dotnet restore "RoomLocator.Api/RoomLocator.Api.csproj"
COPY . .
WORKDIR "src/RoomLocator/RoomLocator.Api"
RUN dotnet build "RoomLocator.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomLocator.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5164
EXPOSE 7137

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomLocator.Api.dll"]
