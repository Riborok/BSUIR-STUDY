Эндпоинты
Эндпоинты задач
Получить все задачи пользователя
GET /api/tasks

Получить список всех задач, связанных с текущим пользователем.

Требуется аутентификация

Заголовки:

Authorization: Bearer <JWT-TOKEN>

Пример запроса:

http

Копировать код

GET /api/tasks HTTP/1.1

Host: example.com

Authorization: Bearer <JWT-TOKEN>

Пример ответа:

json

[
{
"id": 1,
"title": "Первая задача",
"description": "Описание задачи",
"status": "Pending",
"createdBy": "user@example.com",
"participants": ["user1@example.com", "user2@example.com"]
}
]

Получить историю изменений задачи

GET /api/tasks/{taskId}/history

Возвращает историю изменений указанной задачи.

Требуется аутентификация

Параметры:

taskId: ID задачи (в пути).

Заголовки:

Authorization: Bearer <JWT-TOKEN>

Пример запроса:

http
Копировать код
GET /api/tasks/1/history HTTP/1.1
Host: example.com
Authorization: Bearer <JWT-TOKEN>
Пример ответа:
json
Копировать код
[
{
"action": "STATUS_CHANGE",
"previousValue": "Pending",
"newValue": "Completed",
"changedBy": "user@example.com",
"changedAt": "2024-11-26T12:34:56"
}
]
Создать задачу
POST /api/tasks

Создает новую задачу.

Требуется аутентификация

Заголовки:
Authorization: Bearer <JWT-TOKEN>
Тело запроса:
json
Копировать код
{
"title": "Новая задача",
"description": "Описание задачи"
}
Пример запроса:
http
Копировать код
POST /api/tasks HTTP/1.1
Host: example.com
Authorization: Bearer <JWT-TOKEN>
Content-Type: application/json

{
"title": "Новая задача",
"description": "Описание задачи"
}
Пример ответа:
json
Копировать код
{
"id": 2,
"title": "Новая задача",
"description": "Описание задачи",
"status": "Pending",
"createdBy": "user@example.com",
"participants": []
}
Удалить задачу
DELETE /api/tasks/{taskId}

Удаляет указанную задачу.

Требуется аутентификация

Параметры:
taskId: ID задачи (в пути).
Заголовки:
Authorization: Bearer <JWT-TOKEN>
Пример запроса:
http
Копировать код
DELETE /api/tasks/2 HTTP/1.1
Host: example.com
Authorization: Bearer <JWT-TOKEN>
Пример ответа:
http
Копировать код
HTTP/1.1 204 No Content
Обновить статус задачи
PUT /api/tasks/{taskId}/status

Обновляет статус указанной задачи.

Требуется аутентификация

Параметры:
taskId: ID задачи (в пути).
Заголовки:
Authorization: Bearer <JWT-TOKEN>
Тело запроса:
json
Копировать код
{
"newStatus": "Completed"
}
Пример запроса:
http
Копировать код
PUT /api/tasks/1/status HTTP/1.1
Host: example.com
Authorization: Bearer <JWT-TOKEN>
Content-Type: application/json

{
"newStatus": "Completed"
}
Пример ответа:
http
Копировать код
HTTP/1.1 204 No Content
Эндпоинты аутентификации
Войти в систему
POST /api/auth/signin

Аутентифицирует пользователя и возвращает JWT-токен.

Тело запроса:
json
Копировать код
{
"email": "user@example.com",
"password": "password123"
}
Пример запроса:
http
Копировать код
POST /api/auth/signin HTTP/1.1
Host: example.com
Content-Type: application/json

{
"email": "user@example.com",
"password": "password123"
}
Пример ответа:
json
Копировать код
{
"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
Зарегистрироваться
POST /api/auth/signup

Регистрирует нового пользователя и возвращает JWT-токен.

Тело запроса:
json
Копировать код
{
"email": "newuser@example.com",
"password": "password123",
"firstName": "John",
"lastName": "Doe"
}
Пример запроса:
http
Копировать код
POST /api/auth/signup HTTP/1.1
Host: example.com
Content-Type: application/json

{
"email": "newuser@example.com",
"password": "password123",
"firstName": "John",
"lastName": "Doe"
}
Пример ответа:
json
Копировать код
{
"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
