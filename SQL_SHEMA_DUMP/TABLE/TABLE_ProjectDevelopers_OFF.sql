--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--TABLE: ProjectDevelopers_OFF
--26/11/2014 8:10:04 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE TABLE [ProjectDevelopers_OFF](
	[DeveloperID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL
) ON [PRIMARY]
;

ALTER TABLE [ProjectDevelopers_OFF] ADD  CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[DeveloperID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectDevelopers_OFF] ADD  CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[DeveloperID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

ALTER TABLE [ProjectDevelopers_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectDevelopers_Developers] FOREIGN KEY([DeveloperID])
REFERENCES [Developers] ([ID]);

ALTER TABLE [ProjectDevelopers_OFF] CHECK CONSTRAINT [FK_ProjectDevelopers_Developers];

ALTER TABLE [ProjectDevelopers_OFF]  WITH CHECK ADD  CONSTRAINT [FK_ProjectDevelopers_Projects] FOREIGN KEY([ProjectID])
REFERENCES [Projects] ([ID]);

ALTER TABLE [ProjectDevelopers_OFF] CHECK CONSTRAINT [FK_ProjectDevelopers_Projects];

