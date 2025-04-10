START TRANSACTION;

ALTER TABLE `Tickets` DROP FOREIGN KEY `FK_Tickets_Orders_OrderId`;

ALTER TABLE `Tickets` DROP INDEX `IX_Tickets_OrderId`;

ALTER TABLE `Tickets` DROP COLUMN `OrderId`;

ALTER TABLE `Tickets` MODIFY COLUMN `Id` char(36) COLLATE ascii_general_ci NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250327201535_TicketChangeIdToGuid', '8.0.2');

COMMIT;