#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Copy the code into the container
WORKDIR /src
COPY ["src/Org.OpenAPITools/Org.OpenAPITools.csproj", "Org.OpenAPITools/"]

# NuGet restore
RUN dotnet restore "Org.OpenAPITools/Org.OpenAPITools.csproj"
COPY ["src/Org.OpenAPITools/", "Org.OpenAPITools/"]

# Build the API
WORKDIR /src/Org.OpenAPITools
RUN dotnet build "Org.OpenAPITools.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
WORKDIR /src/Org.OpenAPITools
RUN dotnet publish "Org.OpenAPITools.csproj" -c Release -o /app/publish

# Make the final image for publishing
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run as non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser && \
    chown -R appuser:appgroup /app
USER appuser

HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD ["dotnet", "--info"]

ENTRYPOINT ["dotnet", "Org.OpenAPITools.dll"]