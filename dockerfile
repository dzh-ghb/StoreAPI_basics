# Базовый образ .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копирование проекта и восстановление зависимостей
COPY "StoreApi.sln" "StoreApi.sln"
COPY "Api/Api.csproj" "Api/Api.csproj"

RUN dotnet restore "StoreApi.sln"

# Копирование и сборка приложения
COPY . .
WORKDIR /app
RUN dotnet publish -c Release -o out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:1337

ENTRYPOINT ["dotnet", "Api.dll"]