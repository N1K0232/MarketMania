CREATE TABLE [dbo].[Categories]
(
	[Id]                UNIQUEIDENTIFIER        NOT NULL,
    [Name]              NVARCHAR (256)          NOT NULL,
    [Description]       NVARCHAR (512)          NOT NULL,
    [CreatedAt]         DATETIME2               NOT NULL,
    [LastModifiedAt]    DATETIME2               NULL,
);

GO
ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [PK_Categories] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id]

GO
ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt]