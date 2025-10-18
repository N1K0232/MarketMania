CREATE TABLE [dbo].[Promotions]
(
	[Id]                    UNIQUEIDENTIFIER        NOT NULL,
    [Name]                  NVARCHAR (255)          NOT NULL,
    [DiscountPercentage]    DECIMAL (5, 2)          NOT NULL,
    [StartDate]             DATETIME2               NOT NULL,
    [EndDate]               DATETIME2               NOT NULL,
    [IsActive]              BIT                     NOT NULL,
    [CreatedAt]             DATETIME2               NOT NULL,
    [LastModifiedAt]        DATETIME2               NULL,
);

GO
ALTER TABLE [dbo].[Promotions]
    ADD CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Promotions]
    ADD CONSTRAINT [DF_Promotions_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Promotions]
    ADD CONSTRAINT [DF_Promotions_CreatedAt] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAt];

GO
ALTER TABLE [dbo].[Promotions]
    ADD CONSTRAINT [DF_Promotions_IsActive] DEFAULT ((1)) FOR [IsActive];