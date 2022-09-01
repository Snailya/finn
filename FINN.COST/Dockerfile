FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.COST/FINN.COST.csproj", "FINN.COST/"]
RUN dotnet restore "FINN.COST/FINN.COST.csproj"
COPY . .
WORKDIR "/src/FINN.COST"
RUN dotnet build "FINN.COST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.COST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.COST.dll"]
