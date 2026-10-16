FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ERS.slnx ./
COPY src/ERS.Domain/ERS.Domain.csproj src/ERS.Domain/
COPY src/ERS.Localization/ERS.Localization.csproj src/ERS.Localization/
COPY src/ERS.Application/ERS.Application.csproj src/ERS.Application/
COPY src/ERS.Infrastructure/ERS.Infrastructure.csproj src/ERS.Infrastructure/
COPY src/ERS.Admin/ERS.Admin.csproj src/ERS.Admin/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/ERS.Admin/ERS.Admin.csproj

COPY src/ERS.Domain/ src/ERS.Domain/
COPY src/ERS.Localization/ src/ERS.Localization/
COPY src/ERS.Application/ src/ERS.Application/
COPY src/ERS.Infrastructure/ src/ERS.Infrastructure/
COPY src/ERS.Admin/ src/ERS.Admin/

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/src/ERS.Domain/obj \
    --mount=type=cache,target=/src/src/ERS.Domain/bin \
    --mount=type=cache,target=/src/src/ERS.Localization/obj \
    --mount=type=cache,target=/src/src/ERS.Localization/bin \
    --mount=type=cache,target=/src/src/ERS.Application/obj \
    --mount=type=cache,target=/src/src/ERS.Application/bin \
    --mount=type=cache,target=/src/src/ERS.Infrastructure/obj \
    --mount=type=cache,target=/src/src/ERS.Infrastructure/bin \
    --mount=type=cache,target=/src/src/ERS.Admin/obj \
    --mount=type=cache,target=/src/src/ERS.Admin/bin \
    dotnet publish src/ERS.Admin/ERS.Admin.csproj \
        -c Release \
        -o /app/publish \
        /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build --chown=app:app /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

USER app

ENTRYPOINT ["dotnet", "ERS.Admin.dll"]
