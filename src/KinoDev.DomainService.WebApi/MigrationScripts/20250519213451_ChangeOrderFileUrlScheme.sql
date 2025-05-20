START TRANSACTION;

ALTER TABLE `Orders` MODIFY COLUMN `FileUrl` varchar(1500) CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250519213451_ChangeOrderFileUrlScheme', '8.0.2');

COMMIT;