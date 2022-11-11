FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FINN.SLIDE/FINN.SLIDE.csproj", "FINN.SLIDE/"]
RUN dotnet restore "FINN.SLIDE/FINN.SLIDE.csproj"
COPY . .
WORKDIR "/src/FINN.SLIDE"
RUN dotnet build "FINN.SLIDE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.SLIDE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.SLIDE.dll"]
