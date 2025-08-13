CREATE TABLE [dbo].[Specifications]
(
	[Id]                UNIQUEIDENTIFIER    NOT NULL,
    [ProductId]         UNIQUEIDENTIFIER    NOT NULL,
    [Name]              NVARCHAR (256)      NOT NULL,
    [Description]       NVARCHAR (MAX)      NOT NULL,
    [CreatedAt]         DATETIME2           NOT NULL,
    [LastModifiedAt]    DATETIME2           NULL
);

GO
ALTER TABLE [dbo].[Specifications]
    ADD CONSTRAINT [PK_Specifications] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Specifications]
    ADD CONSTRAINT [FK_Specifications_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Specifications]
    ADD CONSTRAINT [DF_Specifications_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Specifications]
    ADD CONSTRAINT [DF_Specifications_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];

GO
CREATE NONCLUSTERED INDEX [IX_Specifications_Name]
    ON [dbo].[Specifications]([Name]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Specifications_UniqueName]
    ON [dbo].[Specifications]([ProductId] ASC, [Name] ASC);