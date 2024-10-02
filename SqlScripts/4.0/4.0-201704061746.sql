/****** Object:  StoredProcedure [dbo].[WAGES_ABSENT_REPORT_BYGROUP]    Script Date: 4/6/2017 5:45:47 PM ******/
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

ALTER PROCEDURE [dbo].[WAGES_ABSENT_REPORT_BYGROUP]
(
		@paramStartDate DATE
	,	@paramEndDate	DATE
	,	@paramLocationCode	VARCHAR(10)
	,	@paramUnitCode		VARCHAR(10) = NULL
	,	@paramProcessGroup	VARCHAR(10) = NULL
)
WITH RECOMPILE
AS
BEGIN
SET DATEFIRST 1
	DECLARE @startDate	DATE;
	DECLARE @endDate	DATE;
	DECLARE @LocationCode	VARCHAR(10);
	DECLARE @UnitCode		VARCHAR(10) = NULL;
	DECLARE @ProcessGroup	VARCHAR(10) = NULL;

	SET @startDate		= @paramStartDate;
	SET @endDate		= @paramEndDate;
	SET @LocationCode	= @paramLocationCode;
	SET @UnitCode		= @paramUnitCode;
	SET @ProcessGroup	= @paramProcessGroup;


	-- DECLARE temp table result
	DECLARE @tblTempResult TABLE 
	(
		id BIGINT
		, LocationCode		VARCHAR(20)
		, ProdUnit			VARCHAR(4)
		, BrandGroupCode	VARCHAR(20)
		, ProcessGroup		VARCHAR(10)
		, ProdGroup			VARCHAR(5)
		, ProductionDate	DATE
		, Terdaftar			FLOAT
		, Alpa				FLOAT
		, Ijin				FLOAT
		, Sakit				FLOAT
		, Cuti				FLOAT
		, CutiHamil			FLOAT
		, CutiTahunan		FLOAT
		, Skorsing			FLOAT
		, Keluar			FLOAT
		, Masuk				FLOAT
		, Turnover			FLOAT
		, TurnoverPersen	FLOAT
		, Kehadiran			FLOAT
		, Absensi			FLOAT
	)
	
	-- BUILD variable query insert
	DECLARE @queryInsert VARCHAR(MAX);

	SET @queryInsert = 
	'SELECT 
		id
		,	LocationCode
		,	ProdUnit
		,	BrandGroupCode
		,	ProcessGroup
		,	ProdGroup
		,	ProductionDate
		,	Terdaftar
		,	Alpa
		,	Ijin
		,	Sakit
		,	Cuti
		,	CutiHamil
		,	CutiTahunan
		,	Skorsing 
		,	(CASE WHEN TerdaftarAkhir < TerdaftarAwal THEN ABS(TerdaftarAkhir - TerdaftarAwal) ELSE 0 END) AS Keluar
		,	(CAST(Terdaftar AS FLOAT) - Alpa - Ijin - Sakit -  Cuti - CutiHamil - CutiTahunan - Skorsing) AS Masuk
		,	ABS(TerdaftarAkhir - TerdaftarAwal) AS Turnover
		,	(CASE WHEN TerdaftarAkhir > 0 THEN (ABS(TerdaftarAkhir  - TerdaftarAwal) / CAST(TerdaftarAkhir AS FLOAT) * 100) ELSE 0 END) AS TurnoverPersen
		,	COALESCE((CAST(Terdaftar AS FLOAT) - Alpa - Ijin - Sakit -  Cuti - CutiHamil - CutiTahunan - Skorsing) / NULLIF(CAST(Terdaftar AS FLOAT), 0) * 100, 0) AS Kehadiran
		,	COALESCE(((Alpa + Ijin + Sakit + Cuti + CutiHamil + CutiTahunan + Skorsing) / NULLIF(CAST(Terdaftar AS FLOAT), 0)), 0) * 100 AS Absensi
	FROM
	(
		SELECT ROW_NUMBER() OVER (ORDER BY LocationCode) AS id
			,	bg.LocationCode	
			,	bg.UnitCode AS ProdUnit
			,	bg.BrandGroupCode
			,	'''' AS ProcessGroup -- let empty
			,	bg.GroupCode AS ProdGroup
			,	GETDATE() AS ProductionDate -- let empty/default datenow
			,	CEILING(AVG(CAST(bg.Register AS FLOAT))) AS Terdaftar
			,	CEILING(AVG(CAST(ISNULL(bg.Absennce_A, 0) AS FLOAT))) AS Alpa
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_I, 0) AS FLOAT))) AS Ijin
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_S, 0) AS FLOAT))) AS Sakit
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_C, 0) AS FLOAT))) AS Cuti
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_CH, 0) AS FLOAT))) AS CutiHamil
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_CT, 0) AS FLOAT))) AS CutiTahunan
			,	CEILING(AVG(CAST(ISNULL(bg.Absence_ETC, 0) AS FLOAT))) AS Skorsing
			,	ISNULL((SELECT CEILING(AVG(CAST(Register AS FLOAT))) FROM ExeReportByGroups AS sub WHERE sub.LocationCode = MAX(bg.LocationCode) 
																									AND sub.UnitCode = MAX(bg.UnitCode)
																									AND sub.BrandGroupCode = MAX(bg.BrandGroupCode)
																									AND sub.GroupCode = MAX(bg.GroupCode)
																									AND sub.ProcessGroup = MAX(bg.ProcessGroup)
																									AND sub.ProductionDate =  ''' + CONVERT(VARCHAR(25), @endDate, 120) + '''), 0) AS TerdaftarAkhir
			,	ISNULL((SELECT CEILING(AVG(CAST(Register AS FLOAT))) FROM ExeReportByGroups AS sub WHERE sub.LocationCode = MAX(bg.LocationCode) 
																									AND sub.UnitCode = MAX(bg.UnitCode)
																									AND sub.BrandGroupCode = MAX(bg.BrandGroupCode)
																									AND sub.GroupCode = MAX(bg.GroupCode)
																									AND sub.ProcessGroup = MAX(bg.ProcessGroup)
																									AND sub.ProductionDate =  ''' + CONVERT(VARCHAR(25), @startDate, 120) + '''), 0) AS TerdaftarAwal
		FROM ExeReportByGroups bg
		WHERE bg.StatusEmp = ''Resmi''
			AND bg.UnitCode <>''PROD''
			AND bg.ProductionDate >= ''' + CONVERT(VARCHAR(25), @startDate, 120) + '''
			AND bg.ProductionDate <= ''' + CONVERT(VARCHAR(25), @endDate, 120) + '''';

	-- WHERE AND conditional
	IF (@LocationCode <> 'PLNT')
		SET @queryInsert = @queryInsert + ' AND bg.LocationCode = ''' + @LocationCode + '''';

	IF (@UnitCode IS NOT NULL)
		SET @queryInsert = @queryInsert + ' AND bg.UnitCode = ''' + @UnitCode + '''';

	IF (@ProcessGroup IS NOT NULL)
		SET @queryInsert = @queryInsert + ' AND bg.ProcessGroup = ''' + @ProcessGroup + '''';

	SET @queryInsert = @queryInsert + ' GROUP BY
		bg.BrandGroupCode
		,	bg.LocationCode
		,	bg.UnitCode
		,	bg.GroupCode
	) AS src';

	-- INSERT into temp table result
	INSERT INTO @tblTempResult
	EXEC(@queryInsert);


	---------------- CALCULATE Remark -----------------
	DECLARE @brandGroupCodeCSR VARCHAR(20);
	DECLARE @locationCodeCSR VARCHAR(8);
	DECLARE @unitCodeCSR VARCHAR(4);
	DECLARE @groupCodeCSR VARCHAR(4);
	DECLARE @employeeID VARCHAR(10);
	DECLARE @remark VARCHAR(200);
	DECLARE @brandCodeCSR VARCHAR(20);

	-- Cursor Loop ProductionCard
	DECLARE cursor_pc CURSOR LOCAL FOR
	SELECT BrandGroupCode, LocationCode, UnitCode, GroupCode, EmployeeID, Remark, BrandCode
	FROM ProductionCard WHERE Remark IS NOT NULL /** AND Remark LIKE '%J%' **/  AND Remark <> 'NULL'

	OPEN cursor_pc

	FETCH NEXT FROM cursor_pc   
	INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark, @brandCodeCSR

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
		
			---- count absent
			--DECLARE @countAbsent INT;
			--SELECT @countAbsent = COUNT(*) FROM ProductionCard 
			--WHERE EmployeeID = @employeeID AND ProductionDate = @prodDate

			
			DECLARE @prodCode VARCHAR(50);
			DECLARE @Year INT;
			DECLARE @Week INT;
			SELECT TOP 1 @Year = Year, @Week = Week FROM MstGenWeek WHERE @prodDate BETWEEN StartDate AND EndDate
			SET @prodCode = 'EBL/' + @locationCodeCSR 
			 + '/' + '1'
			 + '/' + @unitCodeCSR 
			 + '/' + @groupCodeCSR 
			 + '/' + @brandCodeCSR 
			 + '/' + CONVERT(varchar,@Year) 
			 + '/' + CONVERT(varchar,@Week)
			 + '/' + CONVERT(varchar,(select datepart(dw, @prodDate)));

			-- check alpha/ijin
			DECLARE @sktAbsentType VARCHAR(10);
			SELECT TOP 1 @sktAbsentType = AbsentCodeEblek FROM ExePlantProductionEntry 
			WHERE EmployeeID = @employeeID AND ProductionEntryCode = @prodCode --ProductionDate = @prodDate

			UPDATE @tblTempResult
			SET 
				 Alpa = CASE WHEN @sktAbsentType = 'A' THEN Alpa - 1 ELSE Alpa END 
				, Ijin = CASE WHEN @sktAbsentType = 'I' THEN Ijin - 1 ELSE Ijin END 
				, Sakit = CASE WHEN @sktAbsent = 'S' THEN Sakit + 1 ELSE Sakit END
				, Cuti = CASE WHEN @sktAbsent = 'C' THEN Cuti + 1 ELSE Cuti END
				, CutiHamil = CASE WHEN @sktAbsent = 'CH' THEN CutiHamil + 1 ELSE CutiHamil END
				, CutiTahunan = CASE WHEN @sktAbsent = 'CT' THEN CutiTahunan + 1 ELSE CutiTahunan END
			WHERE 
				LocationCode = @locationCodeCSR AND BrandGroupCode = @brandGroupCodeCSR AND ProdUnit = @unitCodeCSR AND ProdGroup = @groupCodeCSR
			AND ProductionDate = @prodDate
		
			FETCH NEXT FROM cursor_splitRemark   
			INTO @splitRemark
		END

		CLOSE cursor_splitRemark;  
		DEALLOCATE cursor_splitRemark;  

		FETCH NEXT FROM cursor_pc   
		INTO @brandGroupCodeCSR, @locationCodeCSR, @unitCodeCSR, @groupCodeCSR, @employeeID, @remark, @brandCodeCSR
	END  

	CLOSE cursor_pc;  
	DEALLOCATE cursor_pc;  

	-- Output Result
	SELECT * FROM @tblTempResult ORDER BY LocationCode, ProdUnit, ProdGroup
END