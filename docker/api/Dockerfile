#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM dhi.io/aspnetcore:8 AS base


WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM dhi.io/dotnet:8-sdk-alpine3.23 AS build

# Copy the code into the container
WORKDIR /src
COPY src/Core/Core.csproj Core/
COPY src/Web/Web.csproj Web/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
COPY src/Org.OpenAPITools/Org.OpenAPITools.csproj Org.OpenAPITools/
RUN dotnet add package Microsoft.AspNetCore --version 2.3.6
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

# CKV_DOCKER_3: Create and use a non-root user
# RUN adduser --disabled-password --gecos "" appuser
# USER appuser

# CKV_DOCKER_2: Add healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "Org.OpenAPITools.dll"]