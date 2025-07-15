CREATE TABLE [dbo].[Brands]
(
	[Id]                UNIQUEIDENTIFIER        NOT NULL,
    [Name]              NVARCHAR (255)          NOT NULL,
    [City]              NVARCHAR (50)           NOT NULL,
    [CreatedAt]         DATETIME2               NOT NULL,
    [LastModifiedAt]    DATETIME2               NULL
);

GO
ALTER TABLE [dbo].[Brands]
    ADD CONSTRAINT [PK_Brands] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Brands]
    ADD CONSTRAINT [DF_Brands_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Brands]
    ADD CONSTRAINT [DF_Brands_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];