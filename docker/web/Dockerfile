FROM dhi.io/aspnetcore:8 AS base
WORKDIR /app

FROM dhi.io/dotnet:8-sdk-alpine3.23 AS build

WORKDIR /src

COPY /src .

RUN dotnet restore "Web/Web.csproj"

COPY /src .

WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# CKV_DOCKER_3: Create and use a non-root user
# RUN adduser --disabled-password --gecos "" appuser
# USER appuser

# CKV_DOCKER_2: Add healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "Web.dll"]