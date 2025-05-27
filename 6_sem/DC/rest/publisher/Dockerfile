FROM eclipse-temurin:17-jdk as build
WORKDIR /app
COPY gradle gradle
COPY gradlew gradlew
COPY settings.gradle.kts settings.gradle.kts
COPY build.gradle.kts build.gradle.kts
COPY publisher/build.gradle.kts publisher/build.gradle.kts
COPY publisher/src publisher/src
RUN ./gradlew --no-daemon :publisher:build -x test

FROM eclipse-temurin:17-jre
WORKDIR /app
COPY --from=build /app/publisher/build/libs/publisher-0.0.1-SNAPSHOT.jar app.jar
CMD ["java", "-jar", "app.jar"]