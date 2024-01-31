#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV DEBIAN_FRONTEND=noninteractive
ENV TZ=America/Vancouver
ENV VERSION=1.0.0-DEADBEEF
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && apt-get install -y tzdata
RUN apt-get autoclean
RUN apt-get autoremove
RUN apt-get clean

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WLEDAnimated.API/WLEDAnimated.API.csproj", "WLEDAnimated.API/"]
RUN dotnet restore "./WLEDAnimated.API/./WLEDAnimated.API.csproj"
COPY . .
WORKDIR "/src/WLEDAnimated.API"
RUN echo $VERSION
RUN dotnet build "./WLEDAnimated.API.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:Version=$VERSION  /p:AssemblyInformationalVersion=$VERSION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WLEDAnimated.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "WLEDAnimated.API.dll"]