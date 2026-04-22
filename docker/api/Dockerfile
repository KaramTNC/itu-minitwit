#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM dhi.io/aspnetcore:8 AS base


WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM dhi.io/dotnet:8-sdk-alpine3.23 AS build

# Create non-root user here since the final image has no shell
RUN adduser --disabled-password --gecos "" appuser

# Copy the code into the container
WORKDIR /src
COPY src/Core/Core.csproj Core/
COPY src/Web/Web.csproj Web/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
COPY src/Org.OpenAPITools/Org.OpenAPITools.csproj Org.OpenAPITools/
# NuGet restore
RUN dotnet restore "Org.OpenAPITools/Org.OpenAPITools.csproj"

COPY /src .

# Build the API
WORKDIR "/src/Org.OpenAPITools"
RUN dotnet build "Org.OpenAPITools.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "Org.OpenAPITools.csproj" -c Release -o /app/publish

# Make the final image for publishing
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure the runtime healthcheck command is available in the final image.
RUN if command -v apk >/dev/null 2>&1; then \
      apk add --no-cache curl; \
    elif command -v apt-get >/dev/null 2>&1; then \
      apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*; \
    else \
      echo "No supported package manager found to install curl" >&2; exit 1; \
    fi

# CKV_DOCKER_3: Copy user info from build stage and switch to non-root user
COPY --from=build /etc/passwd /etc/passwd
COPY --from=build /etc/group /etc/group
USER appuser

# CKV_DOCKER_2: Add healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "Org.OpenAPITools.dll"]
