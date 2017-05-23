--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: FileTypes
--26/11/2014 8:10:04 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [FileTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RemovesRegExp] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

ALTER TABLE [FileTypes] ADD  CONSTRAINT [PK_FileTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [FileTypes] ADD  CONSTRAINT [PK_FileTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

