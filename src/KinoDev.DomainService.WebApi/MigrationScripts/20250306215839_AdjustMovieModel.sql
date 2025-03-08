START TRANSACTION;

ALTER TABLE `Movies` MODIFY COLUMN `Description` varchar(1500) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `Movies` ADD `Url` longtext CHARACTER SET utf8mb4 NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250306215839_AdjustMovieModel', '8.0.2');

COMMIT;