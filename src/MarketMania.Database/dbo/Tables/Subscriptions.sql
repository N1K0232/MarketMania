CREATE TABLE [dbo].[Subscriptions]
(
	[Id]                    UNIQUEIDENTIFIER   NOT NULL,
    [UserName]              NVARCHAR (255)     NOT NULL,
    [ApiKey]                NVARCHAR (512)     NOT NULL,
    [ValidFrom]             DATETIMEOFFSET(7)  NOT NULL,
    [ValidTo]               DATETIMEOFFSET(7)  NOT NULL,
    [RequestsPerWindow]     INTEGER            NOT NULL,
    [WindowMinutes]         INTEGER            NOT NULL,
);

GO
ALTER TABLE [dbo].[Subscriptions]
    ADD CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Subscriptions]
    ADD CONSTRAINT [DF_Subscriptions_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Subscriptions_UserName]
    ON [dbo].[Subscriptions]([UserName] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Subscriptions_ApiKey]
    ON [dbo].[Subscriptions]([ApiKey] ASC);