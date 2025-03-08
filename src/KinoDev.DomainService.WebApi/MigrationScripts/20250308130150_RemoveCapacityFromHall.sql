START TRANSACTION;

ALTER TABLE `Halls` DROP COLUMN `Capacity`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250308130150_RemoveCapacityFromHall', '8.0.2');

COMMIT;