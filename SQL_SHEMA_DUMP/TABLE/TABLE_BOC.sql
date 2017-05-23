--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: BOC
--26/11/2014 8:10:02 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [BOC](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[ProjectRepositoryID] [int] NOT NULL,
	[DeveloperID] [int] NOT NULL,
	[FileTypeID] [int] NOT NULL,
	[FileID] [int] NOT NULL,
	[Bytes] [bigint] NOT NULL,
	[Type] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RevisionNumber] [bigint] NOT NULL,
	[DT] [datetime] NOT NULL,
	[Change_OFF] [decimal](18, 3) NOT NULL
) ON [PRIMARY]
;

ALTER TABLE [BOC] ADD  CONSTRAINT [PK_Lines] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

CREATE NONCLUSTERED INDEX [IX_BOC] ON [BOC]
(
	[ProjectID] ASC,
	[FileID] ASC,
	[DeveloperID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

CREATE UNIQUE NONCLUSTERED INDEX [IX_BOC_1] ON [BOC]
(
	[ProjectID] ASC,
	[ProjectRepositoryID] ASC,
	[DeveloperID] ASC,
	[FileID] ASC,
	[RevisionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [BOC] ADD  CONSTRAINT [PK_Lines] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [BOC]  WITH CHECK ADD  CONSTRAINT [FK_BOC_Files] FOREIGN KEY([FileID])
REFERENCES [Files] ([ID]);

ALTER TABLE [BOC] CHECK CONSTRAINT [FK_BOC_Files];

ALTER TABLE [BOC]  WITH CHECK ADD  CONSTRAINT [FK_BOC_ProjectRepositories] FOREIGN KEY([ProjectRepositoryID])
REFERENCES [ProjectRepositories] ([ID]);

ALTER TABLE [BOC] CHECK CONSTRAINT [FK_BOC_ProjectRepositories];

ALTER TABLE [BOC]  WITH CHECK ADD  CONSTRAINT [FK_Lines_Developers] FOREIGN KEY([DeveloperID])
REFERENCES [Developers] ([ID]);

ALTER TABLE [BOC] CHECK CONSTRAINT [FK_Lines_Developers];

ALTER TABLE [BOC]  WITH CHECK ADD  CONSTRAINT [FK_Lines_FileTypes] FOREIGN KEY([FileTypeID])
REFERENCES [FileTypes] ([ID]);

ALTER TABLE [BOC] CHECK CONSTRAINT [FK_Lines_FileTypes];

ALTER TABLE [BOC]  WITH CHECK ADD  CONSTRAINT [FK_Lines_Projects] FOREIGN KEY([ProjectID])
REFERENCES [Projects] ([ID]);

ALTER TABLE [BOC] CHECK CONSTRAINT [FK_Lines_Projects];
