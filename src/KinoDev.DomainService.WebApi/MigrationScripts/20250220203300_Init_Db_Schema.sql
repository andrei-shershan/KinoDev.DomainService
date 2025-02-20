CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Halls` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Capacity` int NOT NULL,
    CONSTRAINT `PK_Halls` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Movies` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ReleaseDate` date NOT NULL,
    `Duration` int NOT NULL,
    CONSTRAINT `PK_Movies` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Cost` decimal(18,2) NOT NULL,
    `State` int NOT NULL,
    `CreatedAt` datetime(0) NOT NULL,
    `CompletedAt` datetime(0) NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Seats` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `HallId` int NOT NULL,
    `Row` int NOT NULL,
    `Number` int NOT NULL,
    CONSTRAINT `PK_Seats` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Seats_Halls_HallId` FOREIGN KEY (`HallId`) REFERENCES `Halls` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ShowTimes` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `MovieId` int NOT NULL,
    `HallId` int NOT NULL,
    `Time` datetime(0) NOT NULL,
    `Price` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_ShowTimes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShowTimes_Halls_HallId` FOREIGN KEY (`HallId`) REFERENCES `Halls` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShowTimes_Movies_MovieId` FOREIGN KEY (`MovieId`) REFERENCES `Movies` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Tickets` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ShowTimeId` int NOT NULL,
    `SeatId` int NOT NULL,
    `OrderId` int NOT NULL,
    CONSTRAINT `PK_Tickets` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Tickets_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tickets_Seats_SeatId` FOREIGN KEY (`SeatId`) REFERENCES `Seats` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tickets_ShowTimes_ShowTimeId` FOREIGN KEY (`ShowTimeId`) REFERENCES `ShowTimes` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Seats_HallId` ON `Seats` (`HallId`);

CREATE INDEX `IX_ShowTimes_HallId` ON `ShowTimes` (`HallId`);

CREATE INDEX `IX_ShowTimes_MovieId` ON `ShowTimes` (`MovieId`);

CREATE INDEX `IX_Tickets_OrderId` ON `Tickets` (`OrderId`);

CREATE INDEX `IX_Tickets_SeatId` ON `Tickets` (`SeatId`);

CREATE INDEX `IX_Tickets_ShowTimeId` ON `Tickets` (`ShowTimeId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250220203300_Init_Db_Schema', '8.0.2');

COMMIT;