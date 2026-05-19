-- ============================================================
-- AireIndustrial - Script de creación de base de datos MySQL
-- ============================================================

CREATE DATABASE IF NOT EXISTS aire_industrial
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE aire_industrial;

-- ----------------------------------------
-- Tablas de ASP.NET Core Identity
-- ----------------------------------------

CREATE TABLE IF NOT EXISTS `AspNetRoles` (
    `Id`               VARCHAR(255) NOT NULL,
    `Name`             VARCHAR(256) NULL,
    `NormalizedName`   VARCHAR(256) NULL,
    `ConcurrencyStamp` LONGTEXT     NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
);

CREATE TABLE IF NOT EXISTS `AspNetUsers` (
    `Id`                   VARCHAR(255) NOT NULL,
    `FirstName`            VARCHAR(100) NOT NULL DEFAULT '',
    `LastName`             VARCHAR(100) NOT NULL DEFAULT '',
    `Tel`                  VARCHAR(20)  NOT NULL DEFAULT '',
    `UserName`             VARCHAR(256) NULL,
    `NormalizedUserName`   VARCHAR(256) NULL,
    `Email`                VARCHAR(256) NULL,
    `NormalizedEmail`      VARCHAR(256) NULL,
    `EmailConfirmed`       TINYINT(1)   NOT NULL DEFAULT 0,
    `PasswordHash`         LONGTEXT     NULL,
    `SecurityStamp`        LONGTEXT     NULL,
    `ConcurrencyStamp`     LONGTEXT     NULL,
    `PhoneNumber`          LONGTEXT     NULL,
    `PhoneNumberConfirmed` TINYINT(1)   NOT NULL DEFAULT 0,
    `TwoFactorEnabled`     TINYINT(1)   NOT NULL DEFAULT 0,
    `LockoutEnd`           DATETIME(6)  NULL,
    `LockoutEnabled`       TINYINT(1)   NOT NULL DEFAULT 1,
    `AccessFailedCount`    INT          NOT NULL DEFAULT 0,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
);

CREATE TABLE IF NOT EXISTS `AspNetUserRoles` (
    `UserId` VARCHAR(255) NOT NULL,
    `RoleId` VARCHAR(255) NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_Roles` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetRoleClaims` (
    `Id`         INT          NOT NULL AUTO_INCREMENT,
    `RoleId`     VARCHAR(255) NOT NULL,
    `ClaimType`  LONGTEXT     NULL,
    `ClaimValue` LONGTEXT     NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_Roles` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserClaims` (
    `Id`         INT          NOT NULL AUTO_INCREMENT,
    `UserId`     VARCHAR(255) NOT NULL,
    `ClaimType`  LONGTEXT     NULL,
    `ClaimValue` LONGTEXT     NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserLogins` (
    `LoginProvider`       VARCHAR(255) NOT NULL,
    `ProviderKey`         VARCHAR(255) NOT NULL,
    `ProviderDisplayName` LONGTEXT     NULL,
    `UserId`              VARCHAR(255) NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserTokens` (
    `UserId`        VARCHAR(255) NOT NULL,
    `LoginProvider` VARCHAR(255) NOT NULL,
    `Name`          VARCHAR(255) NOT NULL,
    `Value`         LONGTEXT     NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

-- ----------------------------------------
-- Tablas de dominio AireIndustrial
-- ----------------------------------------

CREATE TABLE IF NOT EXISTS `SensoresCalidadAire` (
    `Id`        CHAR(36)     NOT NULL,
    `Ubicacion` VARCHAR(200) NOT NULL,
    `TipoGas`   VARCHAR(100) NOT NULL,
    `Estado`    VARCHAR(50)  NOT NULL,
    `CreatedAt` DATETIME(6)  NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `IsDeleted` TINYINT(1)   NOT NULL DEFAULT 0,
    CONSTRAINT `PK_SensoresCalidadAire` PRIMARY KEY (`Id`)
);

CREATE TABLE IF NOT EXISTS `LecturasAire` (
    `Id`        CHAR(36)   NOT NULL,
    `SensorId`  CHAR(36)   NOT NULL,
    `PM2_5`     DOUBLE     NOT NULL,
    `PM10`      DOUBLE     NOT NULL,
    `CO2`       DOUBLE     NOT NULL,
    `FechaHora` DATETIME(6) NOT NULL,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `IsDeleted` TINYINT(1)  NOT NULL DEFAULT 0,
    CONSTRAINT `PK_LecturasAire` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LecturasAire_SensoresCalidadAire`
        FOREIGN KEY (`SensorId`) REFERENCES `SensoresCalidadAire` (`Id`)
        ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS `AlertasAire` (
    `Id`        CHAR(36)     NOT NULL,
    `SensorId`  CHAR(36)     NOT NULL,
    `Nivel`     VARCHAR(50)  NOT NULL,
    `Mensaje`   VARCHAR(500) NOT NULL,
    `FechaHora` DATETIME(6)  NOT NULL,
    `CreatedAt` DATETIME(6)  NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `IsDeleted` TINYINT(1)   NOT NULL DEFAULT 0,
    CONSTRAINT `PK_AlertasAire` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AlertasAire_SensoresCalidadAire`
        FOREIGN KEY (`SensorId`) REFERENCES `SensoresCalidadAire` (`Id`)
        ON DELETE RESTRICT
);

-- ----------------------------------------
-- Índices para rendimiento
-- ----------------------------------------

CREATE INDEX `IX_LecturasAire_SensorId`      ON `LecturasAire` (`SensorId`);
CREATE INDEX `IX_LecturasAire_FechaHora`     ON `LecturasAire` (`FechaHora`);
CREATE INDEX `IX_AlertasAire_SensorId`       ON `AlertasAire`  (`SensorId`);
CREATE INDEX `IX_AlertasAire_FechaHora`      ON `AlertasAire`  (`FechaHora`);
CREATE INDEX `IX_AspNetUsers_NormalizedEmail`ON `AspNetUsers`  (`NormalizedEmail`);
CREATE INDEX `IX_AspNetRoles_NormalizedName` ON `AspNetRoles`  (`NormalizedName`);
