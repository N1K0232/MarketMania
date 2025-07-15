CREATE TABLE [dbo].[Images]
(
	[Id]                UNIQUEIDENTIFIER    NOT NULL,
    [ProductId]         UNIQUEIDENTIFIER    NOT NULL,
    [Path]              NVARCHAR (255)      NOT NULL,
    [ContentType]       NVARCHAR (100)      NOT NULL,
    [Length]            BIGINT              NOT NULL,
    [CreatedAt]         DATETIME2           NOT NULL,
    [LastModifiedAt]    DATETIME2           NULL,
    [IsPublished]       BIT                 NOT NULL
);

GO
ALTER TABLE [dbo].[Images]
    ADD CONSTRAINT [PK_Images] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Images]
    ADD CONSTRAINT [FK_Images_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id]);

GO
ALTER TABLE [dbo].[Images]
    ADD CONSTRAINT [DF_Images_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Images]
    ADD CONSTRAINT [DF_Images_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];

GO
ALTER TABLE [dbo].[Images]
    ADD CONSTRAINT [DF_Images_IsPublished] DEFAULT ((1)) FOR [IsPublished];

GO
CREATE NONCLUSTERED INDEX [IX_Images_Path]
ON [dbo].[Images]([Path]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Images_UniquePath]
ON [dbo].[Images]([ProductId] ASC, [Path] ASC);