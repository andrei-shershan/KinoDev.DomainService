START TRANSACTION;

ALTER TABLE `Orders` DROP COLUMN `HashCode`;

ALTER TABLE `Orders` ADD `EmailSent` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250511141345_AdjustOrderModel', '8.0.2');

COMMIT;