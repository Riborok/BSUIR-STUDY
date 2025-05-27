# 1. Создать представление, позволяющее получать список читателей с количеством
# находящихся у каждого читателя на руках книг, но отображающее только таких
# читателей, по которым имеются задолженности, т.е. на руках у читателя есть
# хотя бы одна книга, которую он должен был вернуть до наступления текущей даты.
CREATE OR REPLACE VIEW `overdue_readers` AS
SELECT `s_id`,
       `s_name`,
       COUNT(`sb_id`) AS `books_on_hand`
FROM `subscribers`
         JOIN `subscriptions` ON `s_id` = `sb_subscriber`
WHERE `sb_is_active` = 'Y'
GROUP BY `s_id`,
         `s_name`
HAVING COUNT(CASE
                 WHEN `sb_finish` < CURDATE() THEN 1
    END) > 0;

# 6. Создать представление, извлекающее информацию о книгах, переводя весь
# текст в верхний регистр и при этом допускающее модификацию списка книг.
CREATE OR REPLACE VIEW `v_books` AS
SELECT `b_id`,
       `b_name`,
       UPPER(`b_name`) AS `b_name_upper`,
       `b_year`,
       `b_quantity`
FROM `books`;

# 13. Создать триггер, не позволяющий добавить в базу данных информацию
# о выдаче книги, если выполняется хотя бы одно из условий:
#   •	дата выдачи или возврата приходится на воскресенье;
#   •	читатель брал за последние полгода более 100 книг;
#   •	промежуток времени между датами выдачи и возврата менее трёх дней.
DROP TRIGGER IF EXISTS `subscriptions_control_ins`;

DELIMITER $$
CREATE TRIGGER `subscriptions_control_ins`
    BEFORE INSERT ON `subscriptions`
    FOR EACH ROW
BEGIN
    DECLARE `v_count` INT;

    IF DAYOFWEEK(NEW.`sb_start`) = 1 OR DAYOFWEEK(NEW.`sb_finish`) = 1 THEN
        SET @msg = 'Sunday not allowed';
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1001;
    END IF;

    IF DATEDIFF(NEW.`sb_finish`, NEW.`sb_start`) < 3 THEN
        SET @msg = 'Period < 3 days';
        SIGNAL SQLSTATE '45002'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1002;
    END IF;

    SELECT COUNT(*) INTO `v_count`
    FROM `subscriptions`
    WHERE `sb_subscriber` = NEW.`sb_subscriber`
      AND `sb_start` >= DATE_SUB(CURDATE(), INTERVAL 6 MONTH);

    IF `v_count` >= 100 THEN
        SET @msg = 'Over 100 books in 6 months';
        SIGNAL SQLSTATE '45003'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1003;
    END IF;
END;
$$
DELIMITER ;


DROP TRIGGER IF EXISTS `subscriptions_control_upd`;

DELIMITER $$
CREATE TRIGGER `subscriptions_control_upd`
    BEFORE UPDATE ON `subscriptions`
    FOR EACH ROW
BEGIN
    DECLARE `v_count` INT;

    IF DAYOFWEEK(NEW.`sb_start`) = 1 OR DAYOFWEEK(NEW.`sb_finish`) = 1 THEN
        SET @msg = 'Sunday not allowed';
        SIGNAL SQLSTATE '45001'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1001;
    END IF;

    IF DATEDIFF(NEW.`sb_finish`, NEW.`sb_start`) < 3 THEN
        SET @msg = 'Period < 3 days';
        SIGNAL SQLSTATE '45002'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1002;
    END IF;

    SELECT COUNT(*) INTO `v_count`
    FROM `subscriptions`
    WHERE `sb_subscriber` = NEW.`sb_subscriber`
      AND `sb_start` >= DATE_SUB(CURDATE(), INTERVAL 6 MONTH);

    IF `v_count` >= 100 THEN
        SET @msg = 'Over 100 books in 6 months';
        SIGNAL SQLSTATE '45003'
            SET MESSAGE_TEXT = @msg, MYSQL_ERRNO = 1003;
    END IF;
END;
$$
DELIMITER ;

# 15. Создать триггер, допускающий регистрацию в библиотеке только таких авторов,
# имя которых не содержит никаких символов кроме букв, цифр, знаков - (минус),
# ' (апостроф) и пробелов (не допускается два и более идущих подряд пробела).
DROP TRIGGER IF EXISTS `authors_control_ins`;

DELIMITER $$
CREATE TRIGGER `authors_control_ins`
    BEFORE INSERT ON `authors`
    FOR EACH ROW
BEGIN
    IF NEW.`a_name` NOT REGEXP '^[A-Za-z0-9''\\- ]+$' THEN
        SIGNAL SQLSTATE '45001'
            SET MESSAGE_TEXT = 'Invalid characters', MYSQL_ERRNO = 2001;
    END IF;

    IF NEW.`a_name` REGEXP '  +' THEN
        SIGNAL SQLSTATE '45002'
            SET MESSAGE_TEXT = 'More than 2 spaces', MYSQL_ERRNO = 2002;
    END IF;
END;
$$
DELIMITER ;


DROP TRIGGER IF EXISTS `authors_control_upd`;

DELIMITER $$
CREATE TRIGGER `authors_control_upd`
    BEFORE UPDATE ON `authors`
    FOR EACH ROW
BEGIN
    IF NEW.`a_name` NOT REGEXP '^[A-Za-z0-9''\\- ]+$' THEN
        SIGNAL SQLSTATE '45001'
            SET MESSAGE_TEXT = 'Invalid characters', MYSQL_ERRNO = 2001;
    END IF;

    IF NEW.`a_name` REGEXP '  +' THEN
        SIGNAL SQLSTATE '45002'
            SET MESSAGE_TEXT = 'More than 2 spaces', MYSQL_ERRNO = 2002;
    END IF;
END;
$$
DELIMITER ;

# 17. Создать триггер, меняющий дату выдачи книги на текущую, если указанная в
# INSERT- или UPDATE-запросе дата выдачи книги меньше текущей на полгода и более.
DROP TRIGGER IF EXISTS `subscriptions_control_ins`;

DELIMITER $$
CREATE TRIGGER `subscriptions_control_ins`
    BEFORE INSERT ON `subscriptions`
    FOR EACH ROW
BEGIN
    IF NEW.`sb_start` < DATE_SUB(CURDATE(), INTERVAL 6 MONTH) THEN
        SET NEW.`sb_start` = CURDATE();
    END IF;
END;
$$
DELIMITER ;


DROP TRIGGER IF EXISTS `subscriptions_control_upd`;

DELIMITER $$
CREATE TRIGGER `subscriptions_control_upd`
    BEFORE UPDATE ON `subscriptions`
    FOR EACH ROW
BEGIN
    IF NEW.`sb_start` < DATE_SUB(CURDATE(), INTERVAL 6 MONTH) THEN
        SET NEW.`sb_start` = CURDATE();
    END IF;
END;
$$
DELIMITER ;