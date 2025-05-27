# Используем базовый образ OpenJDK
FROM openjdk:17-jdk-slim

# Указываем рабочую директорию
WORKDIR /app

# Копируем jar-файл
COPY build/libs/*.jar app.jar

EXPOSE 3012

# Запуск Spring Boot приложения
ENTRYPOINT ["java", "-jar", "app.jar"]
