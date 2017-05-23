--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--VIEW: vProjectBytes
--26/11/2014 8:10:06 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE VIEW dbo.vProjectBytes
AS
SELECT        TOP (100) PERCENT MAX(b.ID) AS vID, p.ID AS ProjID, p.Name AS ProjName, CONVERT(datetime, b.DT, 110) AS CommitDate, SUM(b.Bytes) AS TotalBytes
FROM            dbo.BOC AS b INNER JOIN
                         dbo.Projects AS p ON b.ProjectID = p.ID
GROUP BY p.ID, p.Name, CONVERT(datetime, b.DT, 110)
ORDER BY CommitDate, ProjName DESC
;

