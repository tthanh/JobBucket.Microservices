#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["JB.API.Authentication/JB.API.Authentication.csproj", "JB.API.Authentication/"]
COPY ["JB.API.Infrastructure/JB.API.Infrastructure.csproj", "JB.API.Infrastructure/"]
RUN dotnet restore "JB.API.Authentication/JB.API.Authentication.csproj"
COPY . .
WORKDIR "/src/JB.API.Authentication"
RUN dotnet build "JB.API.Authentication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JB.API.Authentication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JB.Authentication.dll"]

