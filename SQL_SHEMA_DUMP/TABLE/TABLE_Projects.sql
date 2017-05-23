--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: Projects
--26/11/2014 8:10:04 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [Projects](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RepositoryID] [int] NOT NULL,
	[Name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Descr] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ExcludeRegExp_OFF] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

ALTER TABLE [Projects] ADD  CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

CREATE NONCLUSTERED INDEX [IX_Projects] ON [Projects]
(
	[RepositoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [Projects] ADD  CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_Repositories] FOREIGN KEY([RepositoryID])
REFERENCES [Repositories] ([ID]);

ALTER TABLE [Projects] CHECK CONSTRAINT [FK_Projects_Repositories];

