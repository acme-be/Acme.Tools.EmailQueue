CREATE TABLE [acme].[QueuedEmails]
(
    [Id]            [uniqueidentifier] NOT NULL,
    [Email]        [varchar](max)     NOT NULL,
    [Created]       [datetime]         NOT NULL,
    [Sent]          [datetime]         NULL,
    [ResultMessage] [varchar](max)     NULL,
    CONSTRAINT [PK_Mails] PRIMARY KEY NONCLUSTERED
        (
         [Id] ASC
            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [acme].[QueuedEmails]
    ADD CONSTRAINT [DF_QueuedEmails_Created] DEFAULT (getdate()) FOR [Created]
GO

CREATE CLUSTERED INDEX [IX_QueuedEmails_Created] ON [acme].[QueuedEmails]
    (
     [Created] ASC,
     [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO