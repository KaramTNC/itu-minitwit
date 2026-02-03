FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

COPY src /src

CMD dotnet run --project Web/Web.csproj