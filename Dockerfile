FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

COPY src /src

# Run as non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser && \
    chown -R appuser:appgroup /src
USER appuser

HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD ["dotnet", "--info"]

CMD ["dotnet", "run", "--project", "Web/Web.csproj"]