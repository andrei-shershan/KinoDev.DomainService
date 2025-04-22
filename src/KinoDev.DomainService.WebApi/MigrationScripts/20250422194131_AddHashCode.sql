START TRANSACTION;

ALTER TABLE `Orders` ADD `Email` varchar(320) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Orders` ADD `HashCode` varchar(64) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Orders` ADD `UserId` char(36) COLLATE ascii_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250422194131_AddHashCode', '8.0.2');

COMMIT;
