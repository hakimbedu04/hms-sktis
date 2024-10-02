-- =============================================
-- Description: Create Exereport by process view
-- Ticket: 
-- Author: Indra - 2016/06/24
-- =============================================

CREATE VIEW [dbo].[ExeReportByProcessView]
AS
SELECT g.*, e.BrandGroupCode
FROM            
	dbo.ExeReportByProcess AS g INNER JOIN
    dbo.MstGenBrand AS e ON e.BrandCode= g.BrandCode

GO


