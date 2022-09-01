FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#RUN sed -i s@/deb.debian.org/@/mirrors.aliyun.com/@g /etc/apt/sources.list \
#    && apt-get update \
#    && apt-get install -y libx11-dev libgdiplus libc6-dev \
#    && apt-get clean \
#    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FINN.DXF/FINN.DXF.csproj", "FINN.DXF/"]
RUN dotnet restore "FINN.DXF/FINN.DXF.csproj"
COPY . .
WORKDIR "/src/FINN.DXF"
RUN dotnet build "FINN.DXF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FINN.DXF.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FINN.DXF.dll"]
