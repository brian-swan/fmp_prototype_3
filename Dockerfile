FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FMP.Api/FMP.Api.csproj", "src/FMP.Api/"]
COPY ["src/FMP.Core/FMP.Core.csproj", "src/FMP.Core/"]
RUN dotnet restore "src/FMP.Api/FMP.Api.csproj"
COPY . .
WORKDIR "/src/src/FMP.Api"
RUN dotnet build "FMP.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FMP.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "FMP.Api.dll"]