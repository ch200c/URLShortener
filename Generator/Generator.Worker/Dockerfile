#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Generator/Generator.Worker/Generator.Worker.csproj", "Generator.Worker/"]
RUN dotnet restore "Generator.Worker/Generator.Worker.csproj"
COPY . .
WORKDIR "/src/Generator/Generator.Worker"
RUN dotnet build "Generator.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Generator.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Generator.Worker.dll"]