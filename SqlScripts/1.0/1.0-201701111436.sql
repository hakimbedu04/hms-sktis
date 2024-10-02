-- Description: Get Production Card Approval List Detail Group(PIVOT), add SL4
-- Ticket: http://tp.voxteneo.co.id/entity/3727
-- Author: Indra Permana


-- Description: add revision type
-- Ticket: http://tp.voxteneo.co.id/entity/12904
-- Author: Hakim
ALTER VIEW [dbo].[WagesProductionCardApprovalDetailViewGroup]
AS
SELECT        
	pc.RevisionType,
	cast(null as varchar) as ProductionCardCode, 
	LocationCode, 
	UnitCode, 
	ProductionDate, 
	BrandGroupCode, 
	ProcessGroup, 
	GroupCode, 
	COUNT(distinct employeeID) AS Worker, 
	CAST(SUM(Production) AS REAL) AS Production, 
	CAST(SUM(UpahLain) AS REAL) AS UpahLain,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'A' and GroupCode = pc.GroupCode) AS A, 
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'C' and GroupCode = pc.GroupCode) AS C,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'CH' and GroupCode = pc.GroupCode) AS CH,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'CT' and GroupCode = pc.GroupCode) AS CT,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'I' and GroupCode = pc.GroupCode) AS I,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'LL' and GroupCode = pc.GroupCode) AS LL,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'LO' and GroupCode = pc.GroupCode) AS LO,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'LP' and GroupCode = pc.GroupCode) AS LP,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'MO' and GroupCode = pc.GroupCode) AS MO,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'PG' and GroupCode = pc.GroupCode) AS PG,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'S' and GroupCode = pc.GroupCode) AS S,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'SB' and GroupCode = pc.GroupCode) AS SB,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'SKR' and GroupCode = pc.GroupCode) AS SKR,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'SL4' and GroupCode = pc.GroupCode) AS SL4,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'SLP' and GroupCode = pc.GroupCode) AS SLP,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'SLS' and GroupCode = pc.GroupCode) AS SLS,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'T' and GroupCode = pc.GroupCode) AS T,
	(select count(distinct EmployeeID) from dbo.ProductionCard where LocationCode = pc.LocationCode and UnitCode = pc.UnitCode and ProductionDate = pc.ProductionDate and EblekAbsentType = 'TL' and GroupCode = pc.GroupCode) AS TL
 --   SUM(CASE EblekAbsentType WHEN 'A' THEN 1 ELSE 0 END) AS A, 
	--SUM(CASE EblekAbsentType WHEN 'C' THEN 1 ELSE 0 END) AS C, 
	--SUM(CASE EblekAbsentType WHEN 'CH' THEN 1 ELSE 0 END) AS CH, 
 --   SUM(CASE EblekAbsentType WHEN 'CT' THEN 1 ELSE 0 END) AS CT, 
	--SUM(CASE EblekAbsentType WHEN 'I' THEN 1 ELSE 0 END) AS I, 
	--SUM(CASE EblekAbsentType WHEN 'LL' THEN 1 ELSE 0 END) AS LL, 
 --   SUM(CASE EblekAbsentType WHEN 'LO' THEN 1 ELSE 0 END) AS LO, 
	--SUM(CASE EblekAbsentType WHEN 'LP' THEN 1 ELSE 0 END) AS LP, 
	--SUM(CASE EblekAbsentType WHEN 'MO' THEN 1 ELSE 0 END) AS MO, 
 --   SUM(CASE EblekAbsentType WHEN 'PG' THEN 1 ELSE 0 END) AS PG, 
	--SUM(CASE EblekAbsentType WHEN 'S' THEN 1 ELSE 0 END) AS S, 
	--SUM(CASE EblekAbsentType WHEN 'SB' THEN 1 ELSE 0 END) AS SB, 
 --   SUM(CASE EblekAbsentType WHEN 'SKR' THEN 1 ELSE 0 END) AS SKR,
 --   SUM(CASE EblekAbsentType WHEN 'SL4' THEN 1 ELSE 0 END) AS SL4, 
	--SUM(CASE EblekAbsentType WHEN 'SLP' THEN 1 ELSE 0 END) AS SLP, 
	--SUM(CASE EblekAbsentType WHEN 'SLS' THEN 1 ELSE 0 END) AS SLS, 
	--SUM(CASE EblekAbsentType WHEN 'T' THEN 1 ELSE 0 END) AS T, 
	--SUM(CASE EblekAbsentType WHEN 'TL' THEN 1 ELSE 0 END) AS TL
FROM            
	dbo.ProductionCard AS pc
GROUP BY 
	LocationCode, 
	UnitCode, 
	ProductionDate, 
	BrandGroupCode, 
	ProcessGroup, 
	GroupCode,RevisionType

GO


