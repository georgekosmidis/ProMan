--****************************************************
--MS SQL schema dump v1 Beta
--Latest Version on GitHub: http://github.com/georgekosmidis/mssql-schema-dump
--George Kosmidis <www.georgekosmidis.com>
-------------------------------------------------------
--DB: ProMan
--VIEW: vTmp
--26/11/2014 8:10:06 μμ
--****************************************************


SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

CREATE VIEW dbo.vTmp
AS
SELECT        d.Name, SUM(b.Bytes) AS Expr1
FROM            dbo.BOC AS b INNER JOIN
                         dbo.Developers AS d ON b.DeveloperID = d.ID
WHERE        (b.DT BETWEEN GETDATE() - 60 AND GETDATE() - 30)
GROUP BY d.Name
;

