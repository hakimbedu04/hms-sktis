IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].SCHEDULER_INDIVIDUAL_CAPACITY'))
	DROP PROCEDURE [dbo].[SCHEDULER_INDIVIDUAL_CAPACITY]
GO
/****** Object:  StoredProcedure [dbo].[SCHEDULER_INDIVIDUAL_CAPACITY]    Script Date: 10/13/2016 9:38:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SCHEDULER_INDIVIDUAL_CAPACITY]
AS
BEGIN

BEGIN TRANSACTION trans1
BEGIN TRY

DECLARE @brandGroupCode VARCHAR(20);
DECLARE @employeeid VARCHAR(64);
DECLARE @groupCode VARCHAR(4);
DECLARE @processGroup VARCHAR(16);
DECLARE @hourCap3 DECIMAL(18,3);
DECLARE @hourCap5 DECIMAL(18,3);
DECLARE @hourCap6 DECIMAL(18,3);
DECLARE @hourCap7 DECIMAL(18,3);
DECLARE @hourCap8 DECIMAL(18,3);
DECLARE @hourCap9 DECIMAL(18,3);
DECLARE @hourCap10 DECIMAL(18,3);

DECLARE cursor_icap CURSOR LOCAL FOR
SELECT 
	BrandGroupCode, 
	EmployeeID, 
	ProcessGroup, 
	ISNULL([3], 0) as HoursCapacity3,
	ISNULL([5], 0) as HoursCapacity5,
	ISNULL([6], 0) as HoursCapacity6,
	ISNULL([7], 0) as HoursCapacity7,
	ISNULL([8], 0) as HoursCapacity8,
	ISNULL([9], 0) as HoursCapacity9,
	ISNULL([10], 0) as HoursCapacity10
FROM
(
	SELECT 
		pe.EmployeeID, 
		AVG(CONVERT(DECIMAL(18,3), ISNULL(pe.ProdActual, 0))) as AvgProdActual, 
		verif.WorkHour,
		b.BrandGroupCode,
		verif.ProcessGroup
	FROM ExePlantProductionEntryVerification AS verif 
	INNER JOIN ExePlantProductionEntry AS pe ON verif.ProductionEntryCode = pe.ProductionEntryCode 
	INNER JOIN MstGenBrand b ON verif.BrandCode = b.BrandCode 
	INNER JOIN
	(
		SELECT e.EmployeeID, MAX(v.ProductionDate) as MaxDate FROM ExePlantProductionEntry e
		INNER JOIN ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE (e.AbsentType IS NULL OR e.AbsentType = 'Multiskill Out') AND e.ProdActual IS NOT NULL AND e.ProdActual > 0 
		AND v.ProductionDate < CONVERT(DATE, GETDATE()) 
		AND v.WorkHour > 0 AND e.StatusEmp = 'Resmi'
		GROUP BY E.EmployeeID
	) maxDate on pe.EmployeeID = maxDate.EmployeeID
	WHERE (pe.AbsentType IS NULL OR pe.AbsentType = 'Multiskill Out') AND pe.ProdActual IS NOT NULL AND pe.ProdActual > 0
	AND verif.ProductionDate <= maxDate.MaxDate AND verif.ProductionDate >= DATEADD(DAY, -21, maxDate.MaxDate)
	AND verif.WorkHour > 0 AND pe.StatusEmp = 'Resmi'
	GROUP BY
		pe.EmployeeID, 
		verif.WorkHour,
		b.BrandGroupCode,
		verif.ProcessGroup
) icap
PIVOT
(
	SUM(AvgProdActual)
	FOR WorkHour IN ([3], [5], [6], [7], [8], [9], [10])
) AS piv

UNION ALL
--------------- Status Employee Multiskill
------------------------------------------

SELECT 
	BrandGroupCode, 
	EmployeeID,
	ProcessGroup, 
	ISNULL([3], 0) as HoursCapacity3,
	ISNULL([5], 0) as HoursCapacity5,
	ISNULL([6], 0) as HoursCapacity6,
	ISNULL([7], 0) as HoursCapacity7,
	ISNULL([8], 0) as HoursCapacity8,
	ISNULL([9], 0) as HoursCapacity9,
	ISNULL([10], 0) as HoursCapacity10
FROM
(
	SELECT 
		pe.EmployeeID, 
		AVG(CONVERT(DECIMAL(18,3), ISNULL(pe.ProdActual, 0))) as AvgProdActual, 
		verif.WorkHour,
		b.BrandGroupCode,
		verif.ProcessGroup
	FROM ExePlantProductionEntryVerification AS verif 
	INNER JOIN ExePlantProductionEntry AS pe ON verif.ProductionEntryCode = pe.ProductionEntryCode 
	INNER JOIN MstGenBrand b ON verif.BrandCode = b.BrandCode 
	INNER JOIN
	(
		SELECT e.EmployeeID, MAX(v.ProductionDate) as MaxDate FROM ExePlantProductionEntry e
		INNER JOIN ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		LEFT JOIN ExePlantWorkerAssignment ass on ass.EmployeeID = e.EmployeeID AND v.ProductionDate BETWEEN ass.StartDate AND ass.EndDate
		WHERE e.AbsentType IS NULL AND e.ProdActual IS NOT NULL AND e.ProdActual > 0 AND e.StatusEmp = 'Multiskill'
		AND v.ProductionDate < CONVERT(DATE, GETDATE()) 
		AND v.ProcessGroup = ass.DestinationProcessGroup AND v.BrandCode = ass.DestinationBrandCode
		GROUP BY E.EmployeeID
	) maxDate on pe.EmployeeID = maxDate.EmployeeID
	LEFT JOIN ExePlantWorkerAssignment ass on ass.EmployeeID = pe.EmployeeID AND verif.ProductionDate BETWEEN ass.StartDate AND ass.EndDate
	WHERE pe.AbsentType IS NULL AND pe.ProdActual IS NOT NULL AND pe.ProdActual > 0
	AND verif.ProductionDate <= maxDate.MaxDate AND verif.ProductionDate >= DATEADD(DAY, -21, maxDate.MaxDate)
	AND verif.WorkHour > 0 AND pe.StatusEmp = 'Multiskill' AND verif.ProcessGroup = ass.DestinationProcessGroup AND verif.BrandCode = ass.DestinationBrandCode
	GROUP BY
		pe.EmployeeID, 
		verif.WorkHour,
		b.BrandGroupCode,
		verif.GroupCode,
		verif.ProcessGroup
) icap
PIVOT
(
	SUM(AvgProdActual)
	FOR WorkHour IN ([3], [5], [6], [7], [8], [9], [10])
) AS piv

OPEN cursor_icap

FETCH NEXT FROM cursor_icap   
INTO @brandGroupCode, @employeeid, @processGroup, @hourCap3, @hourCap5, @hourCap6, @hourCap7, @hourCap8, @hourCap9, @hourCap10

WHILE @@FETCH_STATUS = 0  
BEGIN  
	DECLARE @LocationCode VARCHAR(8);
	DECLARE @unitCode VARCHAR(4);
	DECLARE @grouCode VARCHAR(4);

	SELECT @LocationCode = LocationCode, @unitCode = UnitCode, @grouCode = GroupCode 
	FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = @employeeid

	IF EXISTS
	(
		SELECT * FROM [dbo].[PlanPlantIndividualCapacityWorkHours] WHERE BrandGroupCode = @brandGroupCode AND EmployeeID = @employeeid
		AND GroupCode = @grouCode AND UnitCode = @unitCode AND LocationCode = @LocationCode
		AND ProcessGroup = @processGroup
	)
	BEGIN
		UPDATE [PlanPlantIndividualCapacityWorkHours]
		SET
			HoursCapacity3	= @hourCap3,
			HoursCapacity5	= @hourCap5,
			HoursCapacity6	= @hourCap6,
			HoursCapacity7	= @hourCap7,
			HoursCapacity8	= @hourCap8,
			HoursCapacity9	= @hourCap9,
			HoursCapacity10 = @hourCap10,
			UpdatedDate = GETDATE(),
			UpdatedBy = 'SYSTEM'
		WHERE BrandGroupCode = @brandGroupCode AND EmployeeID = @employeeid
		AND ProcessGroup = @processGroup
	END
	ELSE
		INSERT INTO [PlanPlantIndividualCapacityWorkHours]
			   ([BrandGroupCode]
			   ,[EmployeeID]
			   ,[GroupCode]
			   ,[UnitCode]
			   ,[LocationCode]
			   ,[ProcessGroup]
			   ,[HoursCapacity3]
			   ,[HoursCapacity5]
			   ,[HoursCapacity6]
			   ,[HoursCapacity7]
			   ,[HoursCapacity8]
			   ,[HoursCapacity9]
			   ,[HoursCapacity10]
			   ,[StatusActive]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		 VALUES
			   (@brandGroupCode
			   ,@employeeid
			   ,@grouCode
			   ,@unitCode
			   ,@LocationCode
			   ,@processGroup
			   ,@hourCap3
			   ,@hourCap5
			   ,@hourCap6
			   ,@hourCap7
			   ,@hourCap8
			   ,@hourCap9
			   ,@hourCap10
			   ,1
			   ,GETDATE()
			   ,'SYSTEM'
			   ,GETDATE()
			   ,'SYSTEM');
	
	FETCH NEXT FROM cursor_icap   
	INTO @brandGroupCode, @employeeid, @processGroup, @hourCap3, @hourCap5, @hourCap6, @hourCap7, @hourCap8, @hourCap9, @hourCap10
END

CLOSE cursor_icap
DEALLOCATE cursor_icap

COMMIT TRANSACTION trans1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION trans1
	IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
	DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
			@ErrorState     INT = ERROR_STATE(),
			@ErrorSeverity  INT = ERROR_SEVERITY();

	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

END