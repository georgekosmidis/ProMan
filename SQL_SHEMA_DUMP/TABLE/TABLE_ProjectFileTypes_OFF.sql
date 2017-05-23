--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: ProjectFileTypes_OFF
--26/11/2014 8:10:04 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [ProjectFileTypes_OFF](
	[ProjectID] [int] NOT NULL,
	[FileTypeID] [int] NOT NULL
) ON [PRIMARY]
;

ALTER TABLE [ProjectFileTypes_OFF] ADD  CONSTRAINT [PK_ProjectFileTypes] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC,
	[FileTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectFileTypes_OFF] ADD  CONSTRAINT [PK_ProjectFileTypes] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC,
	[FileTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectFileTypes_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFileTypes_FileTypes] FOREIGN KEY([FileTypeID])
REFERENCES [FileTypes] ([ID]);

ALTER TABLE [ProjectFileTypes_OFF] CHECK CONSTRAINT [FK_ProjectFileTypes_FileTypes];

ALTER TABLE [ProjectFileTypes_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFileTypes_Projects] FOREIGN KEY([ProjectID])
REFERENCES [Projects] ([ID]);

ALTER TABLE [ProjectFileTypes_OFF] CHECK CONSTRAINT [FK_ProjectFileTypes_Projects];

