FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/adapters/driving/envolti.service.order.written.driving/envolti.service.order.written.driving.csproj", "src/adapters/driving/envolti.service.order.written.driving/"]
RUN dotnet restore "./src/adapters/driving/envolti.service.order.written.driving/envolti.service.order.written.driving.csproj"
COPY . .
WORKDIR "/src/src/adapters/driving/envolti.service.order.written.driving"
RUN dotnet build "./envolti.service.order.written.driving.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./envolti.service.order.written.driving.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "envolti.service.order.written.driving.dll"]