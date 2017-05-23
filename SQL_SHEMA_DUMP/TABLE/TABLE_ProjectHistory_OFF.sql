--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: ProjectHistory_OFF
--26/11/2014 8:10:04 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [ProjectHistory_OFF](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectRepositoryID] [int] NOT NULL,
	[DeveloperID] [int] NOT NULL,
	[Bytes] [bigint] NOT NULL,
	[LastDT] [datetime] NOT NULL,
	[LastRevision] [bigint] NOT NULL
) ON [PRIMARY]
;

ALTER TABLE [ProjectHistory_OFF] ADD  CONSTRAINT [PK_ProjectHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

CREATE UNIQUE NONCLUSTERED INDEX [IX_ProjectHistory] ON [ProjectHistory_OFF]
(
	[ProjectRepositoryID] ASC,
	[DeveloperID] ASC,
	[LastRevision] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectHistory_OFF] ADD  CONSTRAINT [PK_ProjectHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectHistory_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectHistory_Developers] FOREIGN KEY([DeveloperID])
REFERENCES [Developers] ([ID]);

ALTER TABLE [ProjectHistory_OFF] CHECK CONSTRAINT [FK_ProjectHistory_Developers];

ALTER TABLE [ProjectHistory_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectHistory_ProjectRepositories] FOREIGN KEY([ProjectRepositoryID])
REFERENCES [ProjectRepositories] ([ID]);

ALTER TABLE [ProjectHistory_OFF] CHECK CONSTRAINT [FK_ProjectHistory_ProjectRepositories];

