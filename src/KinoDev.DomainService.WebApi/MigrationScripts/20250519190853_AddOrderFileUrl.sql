START TRANSACTION;

ALTER TABLE `Orders` ADD `FileUrl` longtext CHARACTER SET utf8mb4 NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250519190853_AddOrderFileUrl', '8.0.2');

COMMIT;