FROM eclipse-temurin:17-jdk as build
WORKDIR /app
COPY gradle gradle
COPY gradlew gradlew
COPY settings.gradle.kts settings.gradle.kts
COPY build.gradle.kts build.gradle.kts
COPY discussion/build.gradle.kts discussion/build.gradle.kts
COPY discussion/src discussion/src
RUN ./gradlew --no-daemon :discussion:build -x test

FROM eclipse-temurin:17-jre
WORKDIR /app
COPY --from=build /app/discussion/build/libs/discussion-0.0.1-SNAPSHOT.jar app.jar
CMD ["java", "-jar", "app.jar"]