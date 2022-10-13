#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["URLShortener.Generator/URLShortener.Generator.csproj", "URLShortener.Generator/"]
RUN dotnet restore "URLShortener.Generator/URLShortener.Generator.csproj"
COPY . .
WORKDIR "/src/URLShortener.Generator"
RUN dotnet build "URLShortener.Generator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortener.Generator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortener.Generator.dll"]