FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WrappedUSDTCollector/WrappedUSDTCollector.csproj", "WrappedUSDTCollector/"]
RUN dotnet restore "WrappedUSDTCollector/WrappedUSDTCollector.csproj"
COPY . .
WORKDIR "/src/WrappedUSDTCollector"
RUN dotnet build "./WrappedUSDTCollector.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WrappedUSDTCollector.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WrappedUSDTCollector.dll"]
