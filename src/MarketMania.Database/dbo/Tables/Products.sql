CREATE TABLE [dbo].[Products]
(
	[Id]                    UNIQUEIDENTIFIER        NOT NULL,
    [BrandId]               UNIQUEIDENTIFIER        NOT NULL,
    [CategoryId]            UNIQUEIDENTIFIER        NOT NULL,
    [SupplierId]            UNIQUEIDENTIFIER        NOT NULL,
    [PromotionId]           UNIQUEIDENTIFIER        NULL,
    [Name]                  NVARCHAR (256)          NOT NULL,
    [Description]           NVARCHAR (4000)         NOT NULL,
    [Quantity]              INTEGER                 NOT NULL,
    [IsAvailable]           BIT                     NOT NULL,
    [Price]                 DECIMAL (18, 2)         NOT NULL,
    [ShippingCost]          DECIMAL (5, 2)          NULL,
    [DiscountPercentage]    FLOAT                   NULL,
    [Taxes]                 FLOAT                   NULL,
    [TotalPrice]            DECIMAL (18, 2)         NOT NULL,
    [RatingsAverage]        FLOAT                   NULL,
    [ImageUrl]              NVARCHAR (2048)         NULL,
    [Tags]                  NVARCHAR (MAX)          NOT NULL,
    [SeoTitle]              NVARCHAR (255)          NULL,
    [SeoDescription]        NVARCHAR (4000)         NULL,
    [IsFeatured]            BIT                     NOT NULL,
    [Barcode]               NVARCHAR (100)          NOT NULL,
    [SKU]                   INTEGER                 NULL,
    [Weight]                FLOAT                   NOT NULL,         
    [Width]                 FLOAT                   NOT NULL,
    [Height]                FLOAT                   NOT NULL,
    [Length]                FLOAT                   NOT NULL,
    [SerialNumber]          VARCHAR (50)            NOT NULL,
    [WarehouseLocation]     NVARCHAR (100)          NOT NULL,
    [MinStockAlert]         INTEGER                 NOT NULL,
    [IsBackorderable]       BIT                     NOT NULL,
    [RestockDate]           DATETIME2               NULL,
    [ViewCount]             INTEGER                 NOT NULL,
    [PurchaseCount]         INTEGER                 NOT NULL,
    [LastPurchasedAt]       DATETIME2               NULL, 
    [CreatedAt]             DATETIME2               NOT NULL,
    [LastModifiedAt]        DATETIME2               NULL,
    [IsPublished]           BIT                     NOT NULL,
    [PublishedAt]           DATETIME2               NULL,
);

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [PK_Products] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [FK_Products_Brands] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[Brands]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [FK_Products_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [FK_Products_Promotions] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[Promotions]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [DF_Products_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [DF_Products_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [DF_Products_IsPublished] DEFAULT ((0)) FOR [IsPublished];

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [DF_Products_IsFeatured] DEFAULT ((0)) FOR [IsFeatured];

GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [DF_Products_IsAvailable] DEFAULT ((1)) FOR [IsAvailable];

GO
CREATE NONCLUSTERED INDEX [IX_Products_Name]
ON [dbo].[Products]([Name]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_UniqueName]
ON [dbo].[Products]([BrandId] ASC, [CategoryId] ASC, [SupplierId] ASC, [Name] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_UniqueSerialNumber]
ON [dbo].[Products]([SerialNumber] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Products_PromotionId]
ON [dbo].[Products]([PromotionId]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_Barcode]
ON [dbo].[Products]([Barcode]);