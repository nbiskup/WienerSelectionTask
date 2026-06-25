IF OBJECT_ID(N'dbo.Partners', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Partners
    (
        PartnerId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Partners PRIMARY KEY,
        FirstName NVARCHAR(255) NOT NULL,
        LastName NVARCHAR(255) NOT NULL,
        Address NVARCHAR(500) NULL,
        PartnerNumber CHAR(20) NOT NULL,
        CroatianPIN CHAR(11) NULL,
        PartnerTypeId INT NOT NULL,
        CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Partners_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CreatedByUser NVARCHAR(255) NOT NULL,
        IsForeign BIT NOT NULL,
        ExternalCode NVARCHAR(20) NOT NULL,
        Gender CHAR(1) NOT NULL,
        CONSTRAINT CK_Partners_FirstName_Length CHECK (LEN(FirstName) BETWEEN 2 AND 255),
        CONSTRAINT CK_Partners_LastName_Length CHECK (LEN(LastName) BETWEEN 2 AND 255),
        CONSTRAINT CK_Partners_PartnerNumber CHECK (PartnerNumber NOT LIKE '%[^0-9]%' AND LEN(PartnerNumber) = 20),
        CONSTRAINT CK_Partners_CroatianPIN CHECK (CroatianPIN IS NULL OR (CroatianPIN NOT LIKE '%[^0-9]%' AND LEN(CroatianPIN) = 11)),
        CONSTRAINT CK_Partners_PartnerTypeId CHECK (PartnerTypeId IN (1, 2)),
        CONSTRAINT CK_Partners_ExternalCode_Length CHECK (LEN(ExternalCode) BETWEEN 10 AND 20),
        CONSTRAINT CK_Partners_Gender CHECK (Gender IN ('M', 'F', 'N')),
        CONSTRAINT UQ_Partners_ExternalCode UNIQUE (ExternalCode)
    );
END;

IF OBJECT_ID(N'dbo.Policies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Policies
    (
        PolicyId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Policies PRIMARY KEY,
        PartnerId INT NOT NULL,
        PolicyNumber NVARCHAR(15) NOT NULL,
        PolicyAmount DECIMAL(18,2) NOT NULL,
        CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Policies_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Policies_Partners FOREIGN KEY (PartnerId) REFERENCES dbo.Partners(PartnerId) ON DELETE CASCADE,
        CONSTRAINT CK_Policies_PolicyNumber_Length CHECK (LEN(PolicyNumber) BETWEEN 10 AND 15),
        CONSTRAINT CK_Policies_PolicyAmount CHECK (PolicyAmount > 0)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Partners_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.Partners'))
BEGIN
    CREATE INDEX IX_Partners_CreatedAtUtc ON dbo.Partners (CreatedAtUtc DESC);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Policies_PartnerId' AND object_id = OBJECT_ID(N'dbo.Policies'))
BEGIN
    CREATE INDEX IX_Policies_PartnerId ON dbo.Policies (PartnerId);
END;