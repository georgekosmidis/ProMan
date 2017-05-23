--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--VIEW: vDevBytes
--26/11/2014 8:10:06 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE VIEW dbo.vDevBytes
AS
SELECT        MAX(b.ID) AS vID, d.ID AS DevID, d.Name AS DevName, CAST(b.DT AS Date) AS CommitDate, SUM(b.Bytes) AS TotalBytes
FROM            dbo.BOC AS b INNER JOIN
                         dbo.Developers AS d ON b.DeveloperID = d.ID
GROUP BY d.ID, d.Name, CONVERT(DATE, b.DT)
;

