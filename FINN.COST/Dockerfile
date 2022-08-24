FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.COSTER/FINN.COSTER.csproj", "FINN.COSTER/"]
RUN dotnet restore "FINN.COSTER/FINN.COSTER.csproj"
COPY . .
WORKDIR "/src/FINN.COSTER"
RUN dotnet build "FINN.COSTER.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.COSTER.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.COSTER.dll"]
