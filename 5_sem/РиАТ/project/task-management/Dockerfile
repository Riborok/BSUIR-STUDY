# Установка базового образа
FROM node:18 as build

# Установка рабочей директории
WORKDIR /app

# Копирование файлов
COPY package*.json ./
RUN npm install

COPY . ./

# Сборка приложения
RUN npm run build

# Создание образа для Nginx
FROM nginx:1.23

# Копирование сборки в Nginx
COPY --from=build /app/build /usr/share/nginx/html

# Экспонирование порта
EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"]
