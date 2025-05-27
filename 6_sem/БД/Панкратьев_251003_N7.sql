# 1. Создать хранимую процедуру, которая:
# a. добавляет каждой книге два случайных жанра;
# b. отменяет совершённые действия, если в процессе работы хотя бы
# одна операция вставки завершилась ошибкой в силу дублирования
# значения первичного ключа таблицы «m2m_books_genres» (т.е. у такой
# книги уже был такой жанр).
DELIMITER $$
CREATE PROCEDURE AssignRandomGenres()
BEGIN
    DECLARE done_books INT DEFAULT FALSE;
    DECLARE book_id INT;
    DECLARE genre_id_1 INT;
    DECLARE genre_id_2 INT;
    DECLARE total_genres INT;

    DECLARE cur_books CURSOR FOR
        SELECT b_id FROM books;

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
        BEGIN
            ROLLBACK;
        END;

    SELECT COUNT(*) INTO total_genres FROM genres;

    START TRANSACTION;

    OPEN cur_books;

    read_loop:
    LOOP
        FETCH cur_books INTO book_id;
        IF done_books THEN
            LEAVE read_loop;
        END IF;

        SET genre_id_1 = FLOOR(1 + RAND() * total_genres);
        SET genre_id_2 = FLOOR(1 + RAND() * total_genres);

        WHILE genre_id_2 = genre_id_1
            DO
                SET genre_id_2 = FLOOR(1 + RAND() * total_genres);
            END WHILE;

        INSERT INTO m2m_books_genres (b_id, g_id) VALUES (book_id, genre_id_1);
        INSERT INTO m2m_books_genres (b_id, g_id) VALUES (book_id, genre_id_2);
    END LOOP;

    CLOSE cur_books;
    COMMIT;
END$$
DELIMITER ;

# 2. Создать хранимую процедуру, которая:
# a. увеличивает значение поля «b_quantity» для всех книг в два раза;
# b. отменяет совершённое действие, если по итогу выполнения операции
# среднее количество экземпляров книг превысит значение 50.
DELIMITER $$
CREATE PROCEDURE DoubleBookQuantities()
BEGIN
    DECLARE avg_quantity DECIMAL(10, 2);

    START TRANSACTION;

    UPDATE books
    SET b_quantity = b_quantity * 2;

    SELECT AVG(b_quantity)
    INTO avg_quantity
    FROM books;

    IF avg_quantity > 50 THEN
        ROLLBACK;
    ELSE
        COMMIT;
    END IF;
END$$
DELIMITER ;

# 3. Написать запросы, которые, будучи выполненными параллельно,
# обеспечивали бы следующий эффект:
# a. первый запрос должен считать количество выданных на руки и
# возвращён-ных в библиотеку книг и не зависеть от запросов на
# обновление таблицы «subscriptions» (не ждать их завершения);
# b. второй запрос должен инвертировать значения поля
# «sb_is_active» таблицы subscriptions с «Y» на «N» и наоборот и
# не зависеть от первого запроса (не ждать его завершения).
SET SESSION TRANSACTION ISOLATION LEVEL READ COMMITTED;
START TRANSACTION READ ONLY;
SELECT SUM(IF(sb_is_active = 'Y', 1, 0)) AS active_books,
       SUM(IF(sb_is_active = 'N', 1, 0)) AS returned_books
FROM subscriptions;
COMMIT;

START TRANSACTION;
UPDATE subscriptions
SET sb_is_active = CASE
                       WHEN sb_is_active = 'Y' THEN 'N'
                       WHEN sb_is_active = 'N' THEN 'Y'
                       ELSE sb_is_active
    END;
COMMIT;

# 5. Написать код, в котором запрос, инвертирующий значения поля «sb_is_active»
# таблицы «subscriptions» с «Y» на «N» и наоборот, будет иметь максимальные шансы
# на успешное завершение в случае возникновения ситуации взаимной блокировки с
# другими транзакциями.
START TRANSACTION;
UPDATE subscriptions
SET sb_is_active = CASE
                       WHEN sb_is_active = 'Y' THEN 'N'
                       WHEN sb_is_active = 'N' THEN 'Y'
                       ELSE sb_is_active
    END
ORDER BY sb_id;
COMMIT;

# 6. Создать на таблице «subscriptions» триггер, определяющий уровень изолированности
# транзакции, в котором сейчас проходит операция обновления, и отменяющий операцию,
# если уровень изолированности транзакции отличен от REPEATABLE READ.
DELIMITER $$
CREATE TRIGGER subscriptions_control_upd
    BEFORE UPDATE
    ON subscriptions
    FOR EACH ROW
BEGIN
    DECLARE isolation_level VARCHAR(64);

    SELECT @@transaction_isolation INTO isolation_level;

    IF isolation_level != 'REPEATABLE-READ' THEN
        SET @msg = 'Update not allowed';
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1001;
    END IF;
END$$
DELIMITER ;
