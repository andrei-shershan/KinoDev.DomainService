START TRANSACTION;

ALTER TABLE `Orders` MODIFY COLUMN `Id` char(36) COLLATE ascii_general_ci NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250327201746_OrderChangeIdToGuid', '8.0.2');

COMMIT;