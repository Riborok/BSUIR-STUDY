# 1. Создать хранимую функцию, получающую на вход идентификатор читателя и возвращающую
# список идентификаторов книг, которые он уже прочитал и вернул в библиотеку.
DELIMITER $$
CREATE FUNCTION GetReturnedBookIds(subscriber_id INT)
    RETURNS TEXT
    DETERMINISTIC
BEGIN
    DECLARE result TEXT;

    SELECT GROUP_CONCAT(sb_book ORDER BY sb_book SEPARATOR ', ')
    INTO result
    FROM subscriptions
    WHERE sb_subscriber = subscriber_id
      AND sb_is_active = 'N';

    RETURN IFNULL(result, '');
END$$
DELIMITER ;

SELECT GetReturnedBookIds(4);

# 3. Создать хранимую функцию, получающую на вход идентификатор читателя и возвращающую 1,
# если у читателя на руках сейчас менее десяти книг, и 0 в противном случае.
DELIMITER $$
CREATE FUNCTION IsLessTenBooks(subscriber_id INT UNSIGNED)
    RETURNS INT
    DETERMINISTIC
BEGIN
    DECLARE active_books INT;

    SELECT COUNT(*)
    INTO active_books
    FROM subscriptions
    WHERE sb_subscriber = subscriber_id
      AND sb_is_active = 'Y';

    RETURN IF(active_books < 10, 1, 0);
END$$
DELIMITER ;

SELECT IsLessTenBooks(2);

# 5. Создать хранимую процедуру, обновляющую все поля типа DATE (если такие есть)
# всех записей указанной таблицы на значение текущей даты.
DELIMITER $$
CREATE PROCEDURE UpdateDates(IN target_table VARCHAR(64))
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE col_name VARCHAR(64);
    DECLARE sql_query TEXT DEFAULT '';
    DECLARE is_first BOOLEAN DEFAULT TRUE;

    DECLARE cur CURSOR FOR
        SELECT COLUMN_NAME
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = target_table
          AND DATA_TYPE = 'date';

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    OPEN cur;

    read_loop:
    LOOP
        FETCH cur INTO col_name;
        IF done THEN
            LEAVE read_loop;
        END IF;

        IF is_first THEN
            SET sql_query = CONCAT('`', col_name, '` = CURDATE()');
            SET is_first = FALSE;
        ELSE
            SET sql_query = CONCAT(sql_query, ', `', col_name, '` = CURDATE()');
        END IF;
    END LOOP;

    CLOSE cur;

    IF sql_query IS NOT NULL AND sql_query != '' THEN
        SET @full_sql_query = CONCAT('UPDATE `', target_table, '` SET ', sql_query);
        PREPARE stmt FROM @full_sql_query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$
DELIMITER ;

CALL UpdateDates('subscriptions');

# 6. Создать хранимую процедуру, формирующую список таблиц и их внешних ключей,
# зависящих от указанной в параметре функции таблицы.
DELIMITER $$
CREATE PROCEDURE GetForeignKeyDependents(IN target_table VARCHAR(64))
BEGIN
    SELECT TABLE_NAME,
           CONSTRAINT_NAME,
           COLUMN_NAME
    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE REFERENCED_TABLE_NAME = CONVERT(target_table USING utf8mb3) COLLATE utf8mb3_general_ci
      AND TABLE_SCHEMA = DATABASE();
END$$
DELIMITER ;

CALL GetForeignKeyDependents('books');

# 11. Создать хранимую процедуру, удаляющую все представления, для которых SELECT COUNT(1) FROM
# представление возвращает значение меньше десяти.
DELIMITER $$
CREATE PROCEDURE DropViews()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE view_name VARCHAR(64);
    DECLARE row_count INT;

    DECLARE cur CURSOR FOR
        SELECT TABLE_NAME
        FROM INFORMATION_SCHEMA.VIEWS
        WHERE TABLE_SCHEMA = DATABASE();

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    OPEN cur;

    read_loop:
    LOOP
        FETCH cur INTO view_name;
        IF done THEN
            LEAVE read_loop;
        END IF;

        SET @sql_query = CONCAT('SELECT COUNT(1) INTO @res FROM `', view_name, '`');
        PREPARE stmt FROM @sql_query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;

        SELECT @res INTO row_count;

        IF row_count < 10 THEN
            SET @sql_query = CONCAT('DROP VIEW `', view_name, '`');
            PREPARE stmt FROM @sql_query;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
        END IF;
    END LOOP;

    CLOSE cur;
END$$
DELIMITER ;

CALL DropViews();
