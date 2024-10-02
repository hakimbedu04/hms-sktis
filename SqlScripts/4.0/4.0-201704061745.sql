/****** Object:  StoredProcedure [dbo].[WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL]    Script Date: 4/6/2017 2:22:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: change logic calculation Remark SPL
-- Ticket: http://tp.voxteneo.co.id/entity/18537
-- Author: azka
-- Version: 1.1
-- =============================================

ALTER PROCEDURE [dbo].[WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL]
(	
		@paramStartDate DATE
	,	@paramEndDate	DATE
	,	@paramEmployeeID	VARCHAR(20)
)
WITH RECOMPILE
AS
BEGIN
	
	DECLARE @swapParamStartDate		DATE;
	DECLARE @swapParamEndDate		DATE;
	DECLARE @swapParamEmployeeID	VARCHAR(20);

	SET @swapParamStartDate		= @paramStartDate;
	SET @swapParamEndDate		= @paramEndDate;
	SET @swapParamEmployeeID	= @paramEmployeeID;

	DECLARE @tblTempResult TABLE 
	(
		EmployeeNumber		VARCHAR(20)
		, EmployeeID		VARCHAR(20)
		, ProductionDate	DATE
		, AbsentType		VARCHAR(200)
		, EmployeeName		VARCHAR(200)
	)

	INSERT INTO @tblTempResult
	SELECT DISTINCT
		ProdCard.EmployeeNumber
		,	ProdCard.EmployeeID
		,	ProdCard.ProductionDate
		,	exeAbs.AbsentType
		,	ProdCard.EmployeeName
	FROM
	(
	SELECT DISTINCT
		pc.EmployeeNumber
		,	pc.EmployeeID
		,	pc.ProductionDate
		,	pc.EblekAbsentType
		,	(SELECT EmployeeName FROM MstPlantEmpJobsDataAll WHERE EmployeeID = pc.EmployeeID) AS EmployeeName
	FROM productioncard pc
	WHERE 
		pc.EmployeeID = @swapParamEmployeeID
		AND pc.ProductionDate >= @swapParamStartDate
		AND pc.ProductionDate <= @swapParamEndDate
		AND pc.Remark IS NULL
		AND (pc.EblekAbsentType IS NOT NULL OR pc.EblekAbsentType <> 'NULL')
	) AS ProdCard
	INNER JOIN ExePlantWorkerAbsenteeism exeAbs on exeAbs.EmployeeID = ProdCard.EmployeeID 
													AND exeAbs.SktAbsentCode = ProdCard.EblekAbsentType
													AND ProdCard.ProductionDate BETWEEN exeAbs.StartDateAbsent AND exeAbs.EndDateAbsent
	ORDER BY
		ProdCard.ProductionDate

	--SELECT * FROM @tblTempResult

	---------------- CALCULATE Remark -----------------
		DECLARE @brandGroupCodeCSR VARCHAR(20);
		DECLARE @locationCodeCSR VARCHAR(8);
		DECLARE @unitCodeCSR VARCHAR(4);
		DECLARE @groupCodeCSR VARCHAR(4);
		DECLARE @employeeID VARCHAR(10);
		DECLARE @remark VARCHAR(200);
		DECLARE @employeeNumber VARCHAR(10);

		-- Cursor Loop ProductionCard
		DECLARE cursor_pc CURSOR LOCAL FOR
		SELECT BrandGroupCode, LocationCode, UnitCode, GroupCode, EmployeeID, Remark, EmployeeNumber
		FROM ProductionCard WHERE Remark IS NOT NULL AND Remark LIKE '%J%'  AND Remark <> 'NULL' AND EmployeeID = @swapParamEmployeeID

		OPEN cursor_pc

		FETCH NEXT FROM cursor_pc   
		INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark, @employeeNumber

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			DECLARE @splitRemark VARCHAR(10);
			-- Cursor loop remark date
			DECLARE cursor_splitRemark CURSOR LOCAL FOR
			SELECT splitdata FROM [dbo].[fnSplitString](@remark, ';')

			OPEN cursor_splitRemark

			FETCH NEXT FROM cursor_splitRemark   
			INTO @splitRemark

			DECLARE @sktAbsent VARCHAR(5);
			DECLARE @absentType VARCHAR(100);

			WHILE @@FETCH_STATUS = 0  
			BEGIN
				IF (@splitRemark = 'X') 
				BEGIN
					FETCH NEXT FROM cursor_splitRemark   
					INTO @splitRemark
					CONTINUE;
				END
		
				IF (SUBSTRING(@splitRemark, 1, 1) = 'J')
				BEGIN
					SELECT @sktAbsent = SktAbsentCode, @absentType = AbsentType FROM MstPlantAbsentType WHERE SUBSTRING(AlphaReplace, 6, 2) = SUBSTRING(@splitRemark, 6, 2);
					FETCH NEXT FROM cursor_splitRemark   
					INTO @splitRemark
					CONTINUE;
				END
		
				DECLARE @prodDate DATE;
				IF (SUBSTRING(@splitRemark, 1, 1) <> 'J')
				BEGIN
					SET @prodDate = CONVERT(DATE, @splitRemark, 103);
				END
		
				---- count absent
				--DECLARE @countAbsent INT;
				--SELECT @countAbsent = COUNT(*) FROM ProductionCard 
				--WHERE EmployeeID = @employeeID AND ProductionDate = @prodDate

				---- check alpha/ijin
				--DECLARE @sktAbsentType VARCHAR(10);
				--SELECT @sktAbsentType = EblekAbsentType FROM ProductionCard 
				--WHERE EmployeeID = @employeeID AND ProductionDate = @prodDate

				IF (@prodDate BETWEEN @swapParamStartDate AND @swapParamEndDate)
				BEGIN
					INSERT INTO @tblTempResult
						(	EmployeeNumber
							,	EmployeeID		
							,	ProductionDate	
							,	AbsentType	
							,	EmployeeName		
						)
					VALUES
						(
							@employeeNumber
							,	@employeeID
							,	@prodDate
							,	@absentType
							,	(SELECT TOP 1 EmployeeName FROM MstPlantEmpJobsDataAll WHERE EmployeeID = @employeeID)
						)
				END

			
				FETCH NEXT FROM cursor_splitRemark   
				INTO @splitRemark
			END

			CLOSE cursor_splitRemark;  
			DEALLOCATE cursor_splitRemark;  

			FETCH NEXT FROM cursor_pc   
			INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark, @employeeNumber
		END  

		CLOSE cursor_pc;  
		DEALLOCATE cursor_pc;  

	SELECT * FROM @tblTempResult WHERE ProductionDate BETWEEN @swapParamStartDate AND @swapParamEndDate
END