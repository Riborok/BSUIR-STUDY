# 18. Показать самый читаемый жанр, т.е. жанр (или жанры, если их несколько), относящиеся к которому книги читатели брали чаще всего.
WITH `genre_counts` AS
         (SELECT `g`.`g_id`,
                 COUNT(`sb`.`sb_id`) AS `total_borrows`
          FROM `genres` AS `g`
                   LEFT JOIN `m2m_books_genres` AS `mbg` ON `g`.`g_id` = `mbg`.`g_id`
                   LEFT JOIN `subscriptions` AS `sb` ON `mbg`.`b_id` = `sb`.`sb_book`
          GROUP BY `g`.`g_id`)
SELECT `g`.`g_id`,
       `g`.`g_name`,
       `gc`.`total_borrows`
FROM `genre_counts` AS `gc`
         JOIN `genres` AS `g` ON `gc`.`g_id` = `g`.`g_id`
WHERE `gc`.`total_borrows` =
      (SELECT MAX(`total_borrows`)
       FROM `genre_counts`);

# 21. Показать читателей, бравших самые разножанровые книги (т.е. книги, одновременно относящиеся к максимальному количеству жанров).
SELECT DISTINCT `s`.`s_id`,
                `s`.`s_name`
FROM `subscribers` AS `s`
         JOIN `subscriptions` AS `sb` ON `s`.`s_id` = `sb`.`sb_subscriber`
WHERE `sb`.`sb_book` IN
      (SELECT `b_id`
       FROM `m2m_books_genres`
       GROUP BY `b_id`
       HAVING COUNT(`g_id`) =
              (SELECT MAX(`genre_count`)
               FROM
                   (SELECT COUNT(`g_id`) AS `genre_count`
                    FROM `m2m_books_genres`
                    GROUP BY `b_id`) AS `counts`));

# 24. Показать читателя (или читателей, если их окажется несколько), дольше всего держащего у себя книгу (учитывать только случаи, когда книга не возвращена).
SELECT `s`.`s_id`,
       `s`.`s_name`,
       DATEDIFF(CURDATE(), `sb`.`sb_start`) AS `holding_days`
FROM `subscribers` AS `s`
         JOIN `subscriptions` AS `sb` ON `s`.`s_id` = `sb`.`sb_subscriber`
WHERE `sb`.`sb_is_active` = 'Y'
  AND DATEDIFF(CURDATE(), `sb`.`sb_start`) =
      (SELECT MAX(DATEDIFF(CURDATE(), `sb_start`))
       FROM `subscriptions`
       WHERE `sb_is_active` = 'Y');

# 25. Показать, какую книгу (или книги, если их несколько) каждый читатель взял в свой последний визит в библиотеку.
SELECT `s`.`s_id`,
       `s`.`s_name`,
       `last_visit`.`last_visit`,
       GROUP_CONCAT(`b`.`b_name`
                    ORDER BY `b`.`b_name` SEPARATOR ', ') AS `books_taken`
FROM `subscribers` AS `s`
         JOIN `subscriptions` AS `sb` ON `s`.`s_id` = `sb`.`sb_subscriber`
         JOIN `books` AS `b` ON `b`.`b_id` = `sb`.`sb_book`
         JOIN
     (SELECT `sb_subscriber`,
             MAX(`sb_start`) AS `last_visit`
      FROM `subscriptions`
      GROUP BY `sb_subscriber`) AS `last_visit` ON `sb`.`sb_subscriber` = `last_visit`.`sb_subscriber`
         AND `sb`.`sb_start` = `last_visit`.`last_visit`
GROUP BY `s`.`s_id`;

# 28. Показать информацию о том, какие книги (при условии, что он их ещё не брал) каждый из читателей может взять в библиотеке.
SELECT `s`.`s_id`,
       `s`.`s_name`,
       `b`.`b_name`
FROM `subscribers` AS `s`
         CROSS JOIN `books` AS `b`
         LEFT JOIN `subscriptions` AS `sb` ON `sb`.`sb_subscriber` = `s`.`s_id`
    AND `sb`.`sb_book` = `b`.`b_id`
WHERE `b`.`b_quantity` > 0
  AND `sb`.`sb_subscriber` IS NULL
ORDER BY `s`.`s_id`;
