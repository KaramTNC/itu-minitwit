FROM dhi.io/aspnetcore:8 AS base


WORKDIR /app

FROM dhi.io/dotnet:8-sdk-alpine3.23 AS build

# Create non-root user here since the final image has no shell
RUN adduser --disabled-password --gecos "" appuser

WORKDIR /src

COPY /src .
RUN dotnet restore "Web/Web.csproj"

COPY /src .

WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish
RUN mkdir -p /app/publish/DataProtection-Keys && chown appuser /app/publish/DataProtection-Keys

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# CKV_DOCKER_3: Copy user info from build stage and switch to non-root user
COPY --from=build /etc/passwd /etc/passwd
COPY --from=build /etc/group /etc/group
USER appuser

# CKV_DOCKER_2: Add healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:5001/health/live || exit 1

ENTRYPOINT ["dotnet", "Web.dll"]
