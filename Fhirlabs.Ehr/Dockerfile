#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FhirLabs.Ehr/Fhirlabs.Ehr.csproj", "FhirLabs.Ehr/"]
COPY ["FhirLabs.Ehr.Client/Fhirlabs.Ehr.Client.csproj", "FhirLabs.Ehr.Client/"]
COPY ["Fhirlabs.Ehr.Shared/Fhirlabs.Ehr.Rcl.csproj", "Fhirlabs.Ehr.Shared/"]
RUN dotnet restore "./FhirLabs.Ehr/Fhirlabs.Ehr.csproj"
COPY . .
WORKDIR "/src/FhirLabs.Ehr"
RUN dotnet build "./Fhirlabs.Ehr.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fhirlabs.Ehr.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fhirlabs.Ehr.dll"]