#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UrlShortener.Generator/UrlShortener.Generator.csproj", "UrlShortener.Generator/"]
RUN dotnet restore "UrlShortener.Generator/UrlShortener.Generator.csproj"
COPY . .
WORKDIR "/src/UrlShortener.Generator"
RUN dotnet build "UrlShortener.Generator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlShortener.Generator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Generator.dll"]