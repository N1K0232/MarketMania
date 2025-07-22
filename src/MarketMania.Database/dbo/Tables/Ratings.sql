CREATE TABLE [dbo].[Ratings]
(
	[Id]                    UNIQUEIDENTIFIER            NOT NULL,
    [ProductId]             UNIQUEIDENTIFIER            NOT NULL,
    [UserId]                UNIQUEIDENTIFIER            NOT NULL,
    [Title]                 NVARCHAR (255)              NOT NULL,
    [Text]                  NVARCHAR (MAX)              NOT NULL,
    [Score]                 INTEGER                     NOT NULL,
    [SentimentScore]        FLOAT                       NULL,
    [CreatedAt]             DATETIME2                   NOT NULL,
    [LastModifiedAt]        DATETIME2                   NULL,
    [IsPublished]           BIT                         NOT NULL,
);

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [PK_Ratings] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [FK_Ratings_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [FK_Ratings_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [DF_Ratings_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [DF_Ratings_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];

GO
ALTER TABLE [dbo].[Ratings]
    ADD CONSTRAINT [DF_Ratings_IsPublished] DEFAULT ((1)) FOR [IsPublished];