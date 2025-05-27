# 10. Показать книги, количество экземпляров которых меньше среднего
# по библиотеке.
SELECT `b_name`
FROM `books`
WHERE `b_quantity` < (SELECT AVG(`b_quantity`)
                      FROM `books`);

# 11. Показать идентификаторы и даты выдачи книг за первый год работы
# библиотеки (первым годом работы библиотеки считать все даты с первой
# выдачи книги по 31-е декабря (включительно) того года, когда библиотека
# начала работать).
SELECT `sb_book`, `sb_start`
FROM `subscriptions`
WHERE YEAR(`sb_start`) = YEAR((SELECT MIN(`sb_start`) FROM `subscriptions`));

# 15. Показать, сколько в среднем экземпляров книг есть в библиотеке.
SELECT AVG(`b_quantity`) AS `avg_books_quantity`
FROM `books`;

# 16. Показать в днях, сколько в среднем времени читатели уже
# зарегистрированы в библиотеке (временем регистрации считать
# диапазон от первой даты получения читателем книги до текущей даты).
SELECT AVG(DATEDIFF(CURDATE(), `first_receipt_date`)) AS `avg_registration_days`
FROM (SELECT MIN(`sb_start`) AS `first_receipt_date`
      FROM `subscriptions`
      GROUP BY `sb_subscriber`) AS `first_receipt_dates`;

# 17. Показать, сколько книг было возвращено и не возвращено в
# библиотеку (СУБД должна оперировать исходными значениями поля
# sb_is_active (т.е. «Y» и «N»), а после подсчёта значения «Y» и
# «N» должны быть преобразованы в «Returned» и «Not returned»).
SELECT IF(`sb_is_active` = 'N', 'Returned', 'Not returned') AS `status`,
       COUNT(`sb_id`)                                       AS `b_quantity`
FROM `subscriptions`
GROUP BY `status`