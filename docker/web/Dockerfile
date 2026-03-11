FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

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
ENTRYPOINT ["dotnet", "Web.dll"]