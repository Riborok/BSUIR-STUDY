# 6. Отметить как невозвращённые все выдачи, полученные
# читателем с идентификатором 2.
UPDATE `subscriptions`
SET `sb_is_active` = 'Y'
WHERE `sb_subscriber` = 2;

# 9. Удалить информацию обо всех выдачах книг, произведённых
# после 20-го числа любого месяца любого года.
DELETE
FROM `subscriptions`
WHERE DAY(`sb_start`) > 20;

# 10. Добавить в базу данных жанры
# «Политика», «Психология», «История».
INSERT
    IGNORE INTO `genres` (`g_name`)
VALUES ('Политика'),
       ('Психология'),
       ('История');

# 12. Добавить в базу данных читателей с именами «Сидоров С.С.»,
# «Иванов И.И.», «Орлов О.О.»; если читатель с таким именем уже
# существует, добавить в конец имени нового читателя порядковый
# номер в квадратных скобках (например, если при добавлении
# читателя «Сидоров С.С.» выяснится, что в базе данных уже есть
# четыре таких читателя, имя добавляемого должно превратиться
# в «Сидоров С.С. [5]»).
INSERT INTO `subscribers` (`s_name`)
SELECT CONCAT('Сидоров С.С.', IF(`name_count` > 0, CONCAT(' [', `name_count` + 1, ']'), ''))
FROM
    (SELECT COUNT(`s_name`) AS `name_count`
     FROM `subscribers`
     WHERE `s_name` = 'Сидоров С.С.'
        OR `s_name` LIKE 'Сидоров С.С. [%]') AS `t`;


INSERT INTO `subscribers` (`s_name`)
SELECT CONCAT('Иванов И.И.', IF(`name_count` > 0, CONCAT(' [', `name_count` + 1, ']'), ''))
FROM
    (SELECT COUNT(`s_name`) AS `name_count`
     FROM `subscribers`
     WHERE `s_name` = 'Иванов И.И.'
        OR `s_name` LIKE 'Иванов И.И. [%]') AS `t`;


INSERT INTO `subscribers` (`s_name`)
SELECT CONCAT('Орлов О.О.', IF(`name_count` > 0, CONCAT(' [', `name_count` + 1, ']'), ''))
FROM
    (SELECT COUNT(`s_name`) AS `name_count`
     FROM `subscribers`
     WHERE `s_name` = 'Орлов О.О.'
        OR `s_name` LIKE 'Орлов О.О. [%]') AS `t`;

# 13. Обновить все имена авторов, добавив в конец имени « [+]»,
# если в библиотеке есть более трёх книг этого автора, или добавив
# в конец имени « [-]» в противном случае.
UPDATE `authors` AS `a`
    JOIN
    (SELECT `a`.`a_id`,
            COUNT(`m`.`b_id`) AS `book_count`
     FROM `authors` AS `a`
              LEFT JOIN `m2m_books_authors` AS `m` ON `a`.`a_id` = `m`.`a_id`
     GROUP BY `a`.`a_id`) AS `t` ON `a`.`a_id` = `t`.`a_id`
SET `a`.`a_name` = CONCAT(`a`.`a_name`, IF(`t`.`book_count` > 3, ' [+]', ' [-]'));