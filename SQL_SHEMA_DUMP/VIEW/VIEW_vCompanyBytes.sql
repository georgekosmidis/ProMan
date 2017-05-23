--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--VIEW: vCompanyBytes
--26/11/2014 8:10:05 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE VIEW dbo.vCompanyBytes
WITH SCHEMABINDING 
AS
SELECT      ROW_NUMBER() OVER(ORDER BY CONVERT(DATE, DT) DESC) AS PK, CONVERT(DATE, DT) AS CommitDate, SUM(Bytes) AS TotalBytes
FROM            dbo.BOC AS b
GROUP BY CONVERT(DATE, DT)
;

