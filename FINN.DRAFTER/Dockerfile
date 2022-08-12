FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.DRAFTER/FINN.DRAFTER.csproj", "FINN.DRAFTER/"]
RUN dotnet restore "FINN.DRAFTER/FINN.DRAFTER.csproj"
COPY . .
WORKDIR "/src/FINN.DRAFTER"
RUN dotnet build "FINN.DRAFTER.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.DRAFTER.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.DRAFTER.dll"]
