FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.API/FINN.API.csproj", "FINN.API/"]
RUN dotnet restore "FINN.API/FINN.API.csproj"
COPY . .
WORKDIR "/src/FINN.API"
RUN dotnet build "FINN.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.API.dll"]
