FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.READER/FINN.READER.csproj", "FINN.READER/"]
RUN dotnet restore "FINN.READER/FINN.READER.csproj"
COPY . .
WORKDIR "/src/FINN.READER"
RUN dotnet build "FINN.READER.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.READER.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.READER.dll"]
