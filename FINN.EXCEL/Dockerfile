FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.EXCEL/FINN.EXCEL.csproj", "FINN.EXCEL/"]
RUN dotnet restore "FINN.EXCEL/FINN.EXCEL.csproj"
COPY . .
WORKDIR "/src/FINN.EXCEL"
RUN dotnet build "FINN.EXCEL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.EXCEL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.EXCEL.dll"]
