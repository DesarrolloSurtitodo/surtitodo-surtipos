IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'sap')
BEGIN
    EXEC('CREATE SCHEMA sap');
END;

GO

-- Tabla cabecera del documento agrupado
CREATE TABLE sap.DocumentAgroup(
    Id BIGINT PRIMARY KEY NOT NULL IDENTITY(1,1),
    AgroupDate DATE NOT NULL,
    AgroupHour TIME NOT NULL,
    AgroupDateTime DATETIME2 NOT NULL,
    DocumentAgroupType VARCHAR(10) NOT NULL
        CONSTRAINT CK_DocumentAgroup_DocumentAgroupType CHECK (DocumentAgroupType IN ('VTI_AG', 'VTD_AG')),
    WarehouseCode VARCHAR(10) NOT NULL,
    CashBoxCode VARCHAR(50) NOT NULL,
    DocDate DATE NOT NULL,
    CardCode VARCHAR(150) NOT NULL,
    NumAtCard VARCHAR(150) NOT NULL,
    IntegrationStatus VARCHAR(1) NOT NULL
        CONSTRAINT DF_DocumentAgroup_IntegrationStatus DEFAULT ('P')
        CONSTRAINT CK_DocumentAgroup_IntegrationStatus CHECK (IntegrationStatus IN ('P', 'N', 'E', 'T', 'C')),
    IntegrationDate DATE NULL,
    IntegrationHour TIME NULL,
    IntegrationDateTime DATETIME2 NULL,
    IntegrationCode INT NULL,
    IntegrationMessage VARCHAR(200) NULL,
    IntegrationHttpCode INT NULL,
    IntegrationHttpMessage VARCHAR(100) NULL,
    IntegrationJsonRequestFile VARCHAR(100) NULL,
    IntegrationJsonResponseFile VARCHAR(100) NULL,
    DocNumSap BIGINT NULL,
    DocEntrySap BIGINT NULL
);

GO

-- Tabla detalle del documento agrupado
CREATE TABLE sap.DocumentAgroupLines(
    Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    DocumentAgroupId BIGINT NOT NULL,
    CONSTRAINT FK_DocumentAgroup_DocumentAgroupLines
        FOREIGN KEY (DocumentAgroupId)
        REFERENCES sap.DocumentAgroup(Id)
        ON DELETE CASCADE,
    WarehouseCode VARCHAR(10) NOT NULL,
    ItemCode VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    DiscountPercent INT NOT NULL,
    TaxCode VARCHAR(50) NOT NULL
);

GO

CREATE TABLE DocumentAgroupTrace (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    DocumentAgroupId BIGINT NOT NULL,
    BOCODI          VARCHAR(50) NOT NULL,
    CACODI          VARCHAR(50) NOT NULL,
    TIPDOC          VARCHAR(10) NOT NULL,
    TICODI          INT NOT NULL,
    TracedAt        DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_Trace UNIQUE (BOCODI, CACODI, TIPDOC, TICODI),
    CONSTRAINT FK_Trace_Agroup FOREIGN KEY (DocumentAgroupId)
        REFERENCES DocumentAgroup(Id) ON DELETE CASCADE
);

GO

-- 1. Cola de integraci�n (PRINCIPAL)
CREATE INDEX IX_DocumentAgroup_IntegrationStatus_DateTime
ON sap.DocumentAgroup (IntegrationStatus, AgroupDateTime);

GO

-- 2. Monitoreo de errores
CREATE INDEX IX_DocumentAgroup_IntegrationStatus_Error
ON sap.DocumentAgroup (IntegrationStatus)
INCLUDE (IntegrationMessage, IntegrationCode, IntegrationHttpCode);

GO

-- 3. Control de documentos SAP (evitar duplicados)
CREATE INDEX IX_DocumentAgroup_DocEntrySap
ON sap.DocumentAgroup (DocEntrySap)
WHERE DocEntrySap IS NOT NULL;

GO

-- 4. FK principal (OBLIGATORIO)
CREATE INDEX IX_DocumentAgroupLines_DocumentAgroupId
ON sap.DocumentAgroupLines (DocumentAgroupId);

-- 5. Indice por dia de documento agrupado
CREATE INDEX IX_DocumentAgroup_DocDate
ON sap.DocumentAgroup (DocDate);
