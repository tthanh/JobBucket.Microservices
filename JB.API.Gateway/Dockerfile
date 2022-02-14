#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["JB.API.Gateway/JB.API.Gateway.csproj", "JB.API.Gateway/"]
COPY ["JB.API.Infrastructure/JB.API.Infrastructure.csproj", "JB.API.Infrastructure/"]
RUN dotnet restore "JB.API.Gateway/JB.API.Gateway.csproj"
COPY . .
WORKDIR "/src/JB.API.Gateway"
RUN dotnet build "JB.API.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JB.API.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JB.API.Gateway.dll"]