CREATE TABLE [dbo].[CacheStore]
(
	[Id] NVARCHAR(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
    [Value] VARBINARY(MAX) NOT NULL,
    [ExpiresAtTime] DATETIMEOFFSET(7) NOT NULL,
    [SlidingExpirationInSeconds] BIGINT NULL,
    [AbsoluteExpiration] DATETIMEOFFSET(7) NULL
);

GO
ALTER TABLE [dbo].[CacheStore]
ADD CONSTRAINT [PK_CacheStore_Id] PRIMARY KEY CLUSTERED ([Id] ASC);