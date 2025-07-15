CREATE TABLE [dbo].[Suppliers]
(
	[Id]                UNIQUEIDENTIFIER        NOT NULL,
    [Name]              NVARCHAR (255)          NOT NULL,
    [City]              NVARCHAR (50)           NOT NULL,
    [CreatedAt]         DATETIME2               NOT NULL,
    [LastModifiedAt]    DATETIME2               NULL
);

GO
ALTER TABLE [dbo].[Suppliers]
    ADD CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Suppliers]
    ADD CONSTRAINT [DF_Suppliers_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Suppliers]
    ADD CONSTRAINT [DF_Suppliers_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];