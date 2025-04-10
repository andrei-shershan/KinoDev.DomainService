
START TRANSACTION;

ALTER TABLE `Tickets` MODIFY COLUMN `Id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT (UUID());

ALTER TABLE `Orders` MODIFY COLUMN `Id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT (UUID());

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250327210443_GuidValueGeneratedOnAdd', '8.0.2');

COMMIT;