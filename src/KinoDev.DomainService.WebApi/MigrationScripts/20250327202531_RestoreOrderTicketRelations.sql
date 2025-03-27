START TRANSACTION;

ALTER TABLE `Tickets` ADD `OrderId` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

CREATE INDEX `IX_Tickets_OrderId` ON `Tickets` (`OrderId`);

ALTER TABLE `Tickets` ADD CONSTRAINT `FK_Tickets_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250327202531_RestoreOrderTicketRelations', '8.0.2');

COMMIT;