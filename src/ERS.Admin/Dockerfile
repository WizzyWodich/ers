FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ERS.Admin/ERS.Admin.csproj src/ERS.Admin/
COPY src/ERS.Domain/ERS.Domain.csproj src/ERS.Domain/
COPY src/ERS.Application/ERS.Application.csproj src/ERS.Application/
COPY src/ERS.Infrastructure/ERS.Infrastructure.csproj src/ERS.Infrastructure/
COPY src/ERS.Localization/ERS.Localization.csproj src/ERS.Localization/
COPY ERS.slnx ./

RUN dotnet restore src/ERS.Admin/ERS.Admin.csproj

COPY src/ src/

RUN dotnet publish src/ERS.Admin/ERS.Admin.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" --uid 1000 appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ERS.Admin.dll"]
