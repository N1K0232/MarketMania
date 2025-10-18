CREATE TABLE [dbo].[ShoppingCarts]
(
	[Id] 				UNIQUEIDENTIFIER 			NOT NULL,
	[UserId]			UNIQUEIDENTIFIER			NOT NULL,
	[CreatedAt]			DATETIME2					NOT NULL,
	[LastModifiedAt] 	DATETIME2					NULL
);

GO
ALTER TABLE [dbo].[ShoppingCarts]
	ADD CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[ShoppingCarts]
	ADD CONSTRAINT [FK_ShoppingCarts_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
    ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[ShoppingCarts]
	ADD CONSTRAINT [DF_ShoppingCarts_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[ShoppingCarts]
	ADD CONSTRAINT [DF_ShoppingCarts_CreatedAt] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAt];