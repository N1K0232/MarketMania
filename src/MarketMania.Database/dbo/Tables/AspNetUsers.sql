CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                         UNIQUEIDENTIFIER   NOT NULL,
    [UserName]                   NVARCHAR (256)     NULL,
    [NormalizedUserName]         NVARCHAR (256)     NULL,
    [FirstName]                  NVARCHAR (256)     NOT NULL,
    [LastName]                   NVARCHAR (256)     NULL,
    [Email]                      NVARCHAR (256)     NULL,
    [NormalizedEmail]            NVARCHAR (256)     NULL,
    [EmailConfirmed]             BIT                NOT NULL,
    [PasswordHash]               NVARCHAR (MAX)     NULL,
    [SecurityStamp]              NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]           NVARCHAR (MAX)     NULL,
    [PhoneNumber]                NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed]       BIT                NOT NULL,
    [TwoFactorEnabled]           BIT                NOT NULL,
    [LockoutEnd]                 DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]             BIT                NOT NULL,
    [AccessFailedCount]          INT                NOT NULL,

    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [DF_AspNetUsers_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [DF_AspNetUsers_EmailConfirmed] DEFAULT ((0)) FOR [EmailConfirmed];

GO
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [DF_AspNetUsers_PhoneNumberConfirmed] DEFAULT ((0)) FOR [PhoneNumberConfirmed];

GO
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [DF_AspNetUsers_TwoFactorEnabled] DEFAULT ((0)) FOR [TwoFactorEnabled];

GO
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [DF_AspNetUsers_LockoutEnabled] DEFAULT ((1)) FOR [LockoutEnabled];

GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);