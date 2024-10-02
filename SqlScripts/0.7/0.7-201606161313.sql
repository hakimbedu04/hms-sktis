/****** Object:  UserDefinedFunction [dbo].[GetPlanPlantIndividualCapacityByReference]    Script Date: 6/16/2016 9:46:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Abud
-- Create date: 2016-04-27
-- Description:	http://tp.voxteneo.co.id/entity/3747
-- =============================================

-- =============================================
-- Description: I AM NOT CHANGE ANYTHING JUST RE-RUN THE SCRIPT :)
-- Ticket: http://tp.voxteneo.co.id/entity/7097, http://tp.voxteneo.co.id/entity/7096
-- Author: AZKA
-- Date: 2016/06/16
-- =============================================

ALTER FUNCTION [dbo].[GetPlanPlantIndividualCapacityByReference]
(	
	@LocationCode varchar(20),
	@UnitCode varchar(20),
	@BrandGroupCode varchar(30),
	@ProcessGroup varchar(30),
	@GroupCode varchar(20),
	@Workhour int,
	@StartDate DATE,
	@EndDate DATE
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT pev.ProductionDate,
       pev.GroupCode,
       pev.UnitCode,
       pev.LocationCode,
       pev.ProcessGroup,
       pev.BrandCode,
       brand.BrandGroupCode,
       pev.WorkHour,
       ISNULL(MIN(ProdActual) OVER (PARTITION BY emp.EmployeeID), 0) AS MinimumValue,
       ISNULL(MAX(ProdActual) OVER (PARTITION BY emp.EmployeeID), 0) AS MaximumValue,
       ISNULL(AVG(ProdActual) OVER (PARTITION BY emp.EmployeeID), 0) AS AverageValue,
       ROUND(ISNULL(
         (SELECT AVG(1.0 * ProdActual) AS Median
          FROM
            (SELECT eepe.ProdActual, rn = ROW_NUMBER() OVER (
                                                             ORDER BY eepe.ProdActual), c.c
             FROM dbo.ExePlantProductionEntryVerification AS epev
             INNER JOIN ExePlantProductionEntry AS eepe ON epev.ProductionEntryCode = eepe.ProductionEntryCode
             AND eepe.AbsentType IS NULL
             AND eepe.ProdActual >0
             CROSS JOIN
               (SELECT c = COUNT(eepe.ProdActual)
                FROM dbo.ExePlantProductionEntryVerification AS epev
                INNER JOIN ExePlantProductionEntry AS eepe ON epev.ProductionEntryCode = eepe.ProductionEntryCode
                AND eepe.AbsentType IS NULL
                AND eepe.ProdActual > 0
                WHERE eepe.EmployeeID = emp.EmployeeID
                  AND epev.ProductionDate >= @StartDate
                  AND epev.ProductionDate <= @EndDate
                  AND epev.LocationCode = pev.LocationCode
                  AND epev.GroupCode = pev.GroupCode
                  AND epev.UnitCode = pev.UnitCode
                  AND epev.ProcessGroup = pev.ProcessGroup) AS c
             WHERE eepe.EmployeeID = emp.EmployeeID
               AND epev.ProductionDate >= @StartDate
               AND epev.ProductionDate <= @EndDate
               AND epev.LocationCode = pev.LocationCode
               AND epev.GroupCode = pev.GroupCode
               AND epev.UnitCode = pev.UnitCode
               AND epev.ProcessGroup = pev.ProcessGroup) AS x
          WHERE rn IN ((c + 1)/2, (c + 2)/2)),0),0) AS MedianValue,
       ISNULL(ProdActual, 0) AS LatestValue,
       emp.EmployeeID AS EmployeeID,
       emp.EmployeeNumber AS EmployeeNumber,
       cwh.HoursCapacity3,
       cwh.HoursCapacity5,
       cwh.HoursCapacity6,
       cwh.HoursCapacity7,
       cwh.HoursCapacity8,
       cwh.HoursCapacity9,
       cwh.HoursCapacity10
	FROM dbo.ExePlantProductionEntryVerification AS pev
	INNER JOIN dbo.ExePlantProductionEntry AS pe ON pev.ProductionEntryCode = pe.ProductionEntryCode
	AND pe.AbsentType IS NULL
	AND (pe.ProdActual > 0
		 AND pe.ProdActual IS NOT NULL)
	INNER JOIN dbo.MstPlantEmpJobsDataAcv AS emp ON emp.EmployeeID = pe.EmployeeID
	INNER JOIN dbo.PlanPlantIndividualCapacityWorkHours AS cwh ON cwh.EmployeeID = pe.EmployeeID
	AND cwh.GroupCode = pev.GroupCode
	AND cwh.UnitCode = pev.UnitCode
	AND cwh.LocationCode = pev.LocationCode
	AND cwh.ProcessGroup = pev.ProcessGroup
	INNER JOIN dbo.MstGenBrand AS brand ON brand.BrandCode = pev.BrandCode

	WHERE
		pev.LocationCode = @LocationCode 
		AND pev.UnitCode = @UnitCode 
		AND brand.BrandGroupCode = @BrandGroupCode
		AND pev.ProcessGroup = @ProcessGroup
		AND pev.GroupCode = @GroupCode
		AND pev.WorkHour = @Workhour 
		AND pev.ProductionDate >= @StartDate
		AND pev.ProductionDate <= @EndDate
)
