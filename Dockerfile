FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "DockerRaspr/DockerRaspr.csproj"
WORKDIR "/src/DockerRaspr"
RUN dotnet build "DockerRaspr.csproj" -c Release -o /app

FROM build AS publish
WORKDIR "/src/DockerRaspr"
RUN dotnet publish "DockerRaspr.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DockerRaspr.dll", "--server.urls", "http://127.0.0.1:5000"]