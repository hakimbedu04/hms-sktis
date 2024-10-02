/****** Object:  StoredProcedure [dbo].[WAGES_ABSENT_REPORT_BYEMPLOYEE]    Script Date: 1/10/2017 9:44:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[WAGES_ABSENT_REPORT_BYEMPLOYEE]
(	
		@paramStartDate DATE
	,	@paramEndDate	DATE
	,	@paramLocationCode	VARCHAR(10)
	,	@paramUnitCode		VARCHAR(10) = NULL
	,	@paramGroupCode		VARCHAR(50) = NULL
	,	@paramProcessGroup	VARCHAR(20) = NULL
)
WITH RECOMPILE
AS
BEGIN 

	DECLARE @startDate	DATE;
	DECLARE @endDate	DATE;
	DECLARE @LocationCode	VARCHAR(10);
	DECLARE @UnitCode		VARCHAR(10) = NULL;
	DECLARE @ProcessGroup	VARCHAR(10) = NULL;
	DECLARE @GroupCode		VARCHAR(10) = NULL;

	SET @startDate		= @paramStartDate;
	SET @endDate		= @paramEndDate;
	SET @LocationCode	= @paramLocationCode;
	SET @UnitCode		= @paramUnitCode;
	SET @ProcessGroup	= @paramProcessGroup;
	SET @GroupCode		= @paramGroupCode;

	-- DECLARE temp table result
	DECLARE @tblTempResult TABLE
	(
		id BIGINT
		, LocationCode		VARCHAR(20)
		, ProdUnit			VARCHAR(4)
		, EmployeeID		VARCHAR(20)
		, EmployeeName		VARCHAR(100)
		, Alpa				INT
	    , Ijin				INT
	    , Sakit				INT
	    , Cuti				INT
	    , CutiHamil			INT
	    , CutiTahunan		INT
	    , Skorsing			INT
		, HKNTotal			INT
		, AbsentTotal		INT
		, ProductionTotal	FLOAT
		, JKTotal			INT
		, Productivity		FLOAT
	)

	-- BUILD variable query insert
	DECLARE @queryInsert VARCHAR(MAX);
		
	SET @queryInsert = 
	'SELECT ROW_NUMBER() OVER (
                          ORDER BY MAX(acv.LocationCode)) AS id
		, MAX(acv.LocationCode) AS LocationCode
		, MAX(acv.UnitCode) AS ProdUnit
		, prodCard.EmployeeID
		, MAX(acv.EmployeeNumber) + '' - '' + MAX(acv.EmployeeName) AS EmployeeName
		, SUM(Alpa) AS Alpa
		, SUM(Ijin) AS Ijin
		, SUM(Sakit) AS Sakit
		, SUM(Cuti) AS Cuti
		, SUM(CutiHamil) AS CutiHamil
		, SUM(CutiTahunan) AS CutiTahunan
		, SUM(Skorsing) AS Skorsing
		, SUM(HKNTotal) AS HKNTotal
		, (SUM(Alpa) + SUM(Ijin) + SUM(Sakit) + SUM(Cuti) + SUM(CutiHamil) + SUM(CutiTahunan) + SUM(Skorsing)) AS AbsentTotal
		, SUM((ProductionTotal * psalv.UOMEblek)) AS ProductionTotal
		, SUM(JKTotal) AS JKTotal
		, ROUND((SUM((CASE WHEN JKTotal != 0 THEN (ProductionTotal / JKTotal) ELSE 0 END) * psalv.UOMEblek)), 2) AS Productivity
	FROM
	(
		SELECT 
			pc.BrandGroupCode
			, pc.EmployeeID
			, SUM(CASE WHEN pc.EblekAbsentType = ''A'' AND (pc.Remark IS NULL OR pc.Remark = ''NULL'') THEN 1 ELSE 0 END) AS Alpa
			, SUM(CASE WHEN pc.EblekAbsentType = ''I'' AND (pc.Remark IS NULL OR pc.Remark = ''NULL'') THEN 1 ELSE 0 END) AS Ijin
			, SUM(CASE WHEN pc.EblekAbsentType = ''S'' THEN 1 ELSE 0 END) AS Sakit
			, SUM(CASE WHEN pc.EblekAbsentType = ''C'' THEN 1 ELSE 0 END) AS Cuti
			, SUM(CASE WHEN pc.EblekAbsentType = ''CH'' THEN 1 ELSE 0 END) AS CutiHamil
			, SUM(CASE WHEN pc.EblekAbsentType = ''CT'' THEN 1 ELSE 0 END) AS CutiTahunan
			, SUM(CASE WHEN pc.EblekAbsentType = ''SKR'' THEN 1 ELSE 0 END) AS Skorsing
			, SUM(CASE WHEN (pc.Production + pc.UpahLain > 0)  THEN 1 ELSE 0 END) AS HKNTotal
			, SUM(pc.WorkHours) AS JKTotal
			, SUM(pc.Production) AS ProductionTotal
		FROM ProductionCard AS pc
		WHERE pc.ProductionDate >= ''' + CONVERT(VARCHAR(25), @startDate, 120)  + '''
				AND pc.ProductionDate <= ''' + CONVERT(VARCHAR(25), @endDate, 120) + '''' ;

	IF (@LocationCode <> 'PLNT')
		SET @queryInsert = @queryInsert + ' AND pc.LocationCode = ''' + @LocationCode + '''';
	
	IF (@UnitCode IS NOT NULL)
		SET @queryInsert = @queryInsert + ' AND pc.UnitCode = ''' + @UnitCode + '''';
	
	IF (@GroupCode IS NOT NULL)
		SET @queryInsert = @queryInsert + ' AND pc.GroupCode = ''' + @GroupCode + '''';
	
	IF (@ProcessGroup IS NOT NULL)
		SET @queryInsert = @queryInsert + ' AND pc.ProcessGroup = ''' + @ProcessGroup + '''';
	
	SET @queryInsert = @queryInsert + ' GROUP BY
			pc.EmployeeID
			, pc.BrandGroupCode
	) AS prodCard
	INNER JOIN MstPlantEmpJobsDataAcv AS acv on acv.EmployeeID = prodCard.EmployeeID
	INNER JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = acv.LocationCode 
													AND psalv.BrandGroupCode = prodCard.BrandGroupCode 
													AND psalv.ProcessGroup = acv.ProcessSettingsCode
	GROUP BY
		prodCard.EmployeeID';

	INSERT INTO @tblTempResult
	EXEC(@queryInsert);

	---------------- CALCULATE Remark -----------------
	DECLARE @brandGroupCodeCSR VARCHAR(20);
	DECLARE @locationCodeCSR VARCHAR(8);
	DECLARE @unitCodeCSR VARCHAR(4);
	DECLARE @groupCodeCSR VARCHAR(4);
	DECLARE @employeeID VARCHAR(10);
	DECLARE @remark VARCHAR(200);

	-- Cursor Loop ProductionCard
	DECLARE cursor_pc CURSOR LOCAL FOR
	SELECT BrandGroupCode, LocationCode, UnitCode, GroupCode, EmployeeID, Remark 
	FROM ProductionCard WHERE Remark IS NOT NULL AND Remark LIKE '%J%'  AND Remark <> 'NULL' AND EmployeeID IN (SELECT DISTINCT EmployeeID FROM @tblTempResult)

	OPEN cursor_pc

	FETCH NEXT FROM cursor_pc   
	INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark

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
				SELECT @sktAbsent = SktAbsentCode FROM MstPlantAbsentType WHERE SUBSTRING(AlphaReplace, 6, 2) = SUBSTRING(@splitRemark, 6, 2);
				FETCH NEXT FROM cursor_splitRemark   
				INTO @splitRemark
				CONTINUE;
			END
		
			DECLARE @prodDate DATE;
			IF (SUBSTRING(@splitRemark, 1, 1) <> 'J')
			BEGIN
				SET @prodDate = CONVERT(DATE, @splitRemark, 103);
			END
		
			-- count absent
			DECLARE @countAbsent INT;
			SELECT @countAbsent = COUNT(*) FROM ProductionCard 
			WHERE EmployeeID = @employeeID AND ProductionDate = @prodDate

			-- check alpha/ijin
			DECLARE @sktAbsentType VARCHAR(10);
			SELECT @sktAbsentType = EblekAbsentType FROM ProductionCard 
			WHERE EmployeeID = @employeeID AND ProductionDate = @prodDate

			UPDATE @tblTempResult
			SET 
				Sakit = CASE WHEN @sktAbsent = 'S' THEN Sakit + @countAbsent ELSE Sakit END
				, Cuti = CASE WHEN @sktAbsent = 'C' THEN Cuti + @countAbsent ELSE Cuti END
				, CutiHamil = CASE WHEN @sktAbsent = 'CH' THEN CutiHamil + @countAbsent ELSE CutiHamil END
				, CutiTahunan = CASE WHEN @sktAbsent = 'CT' THEN CutiTahunan + @countAbsent ELSE CutiTahunan END
				, AbsentTotal = Alpa + Ijin + Sakit + Cuti + CutiHamil + CutiTahunan + Skorsing 
			WHERE 
				LocationCode = @locationCodeCSR AND ProdUnit = @unitCodeCSR AND EmployeeID = @employeeID
		
			FETCH NEXT FROM cursor_splitRemark   
			INTO @splitRemark
		END

		CLOSE cursor_splitRemark;  
		DEALLOCATE cursor_splitRemark;  

		FETCH NEXT FROM cursor_pc   
		INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark
	END  

	CLOSE cursor_pc;  
	DEALLOCATE cursor_pc;  

SELECT * FROM @tblTempResult ORDER BY LocationCode, ProdUnit, EmployeeName
END