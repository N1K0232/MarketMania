CREATE TABLE [dbo].[ShoppingCartItems]
(
	[Id]                UNIQUEIDENTIFIER            NOT NULL,
    [ShoppingCartId]    UNIQUEIDENTIFIER            NOT NULL,
    [ProductId]         UNIQUEIDENTIFIER            NOT NULL,
    [UnitPrice]         DECIMAL (18, 2)             NOT NULL,
    [Quantity]          INTEGER                     NOT NULL,
    [CreatedAt]         DATETIME2                   NOT NULL,
    [LastModifiedAt]    DATETIME2                   NULL
);

GO
ALTER TABLE [dbo].[ShoppingCartItems]
    ADD CONSTRAINT [PK_ShoppingCartItems] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[ShoppingCartItems]
    ADD CONSTRAINT [FK_ShoppingCartItems_ShoppingCart] FOREIGN KEY ([ShoppingCartId]) REFERENCES [dbo].[ShoppingCarts]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[ShoppingCartItems]
    ADD CONSTRAINT [FK_ShoppingCartItems_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[ShoppingCartItems]
    ADD CONSTRAINT [DF_ShoppingCartItems_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[ShoppingCartItems]
    ADD CONSTRAINT [DF_ShoppingCartItems_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ShoppingCart_Product]
    ON [dbo].[ShoppingCartItems]([ShoppingCartId] ASC, [ProductId] ASC);