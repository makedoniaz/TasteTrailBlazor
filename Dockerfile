# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet publish TasteTrailBlazor/src/TasteTrailBlazor.csproj -c Release -o /build/dist

# Добавляем команду для проверки файлов после публикации
RUN ls -l /build/dist

# Этап 2: Использование Nginx для отдачи статических файлов
FROM nginx:alpine

# Удаляем дефолтные файлы Nginx (например, index.html)
RUN rm /usr/share/nginx/html/*

# Копируем файлы Blazor прямо в папку, обслуживаемую Nginx
COPY --from=build /build/dist/wwwroot /usr/share/nginx/html

# Nginx автоматически запустится и начнет отдавать файлы
