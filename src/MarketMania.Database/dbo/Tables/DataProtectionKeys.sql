CREATE TABLE [dbo].[DataProtectionKeys]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
    [FriendlyName] NVARCHAR(MAX) NULL,
    [Xml] NVARCHAR(MAX) NULL,
);

GO
ALTER TABLE [dbo].[DataProtectionKeys]
    ADD CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY ([Id] ASC);