CREATE TABLE [acme].[QueuedEmails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sender] [varchar](max) NOT NULL,
	[Recipient] [varchar](max) NOT NULL,
	[ReplyTo] [varchar](max) NULL,
	[Title] [varchar](max) NOT NULL,
	[Body] [varchar](max) NOT NULL,
	[AdminCopy] [bit] NOT NULL,
	[Attachments] [varchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Sent] [datetime] NULL,
	[Message] [varchar](max) NULL,
 CONSTRAINT [PK_Mails] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [acme].[QueuedEmails] ADD  CONSTRAINT [DF_QueuedEmails_Created]  DEFAULT (getdate()) FOR [Created]
GO

