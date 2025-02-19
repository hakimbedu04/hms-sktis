/****** Object:  StoredProcedure [dbo].[GENERATE_BY_PROCESS]    Script Date: 10/21/2016 2:46:10 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GENERATE_BY_PROCESS]'))
	DROP PROCEDURE [dbo].[GENERATE_BY_PROCESS]
GO

/****** Object:  StoredProcedure [dbo].[GENERATE_BY_PROCESS]    Script Date: 10/21/2016 2:46:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GENERATE_BY_PROCESS]
(
	@paramLocationCode		VARCHAR(8),
	@paramUnitCode			VARCHAR(4),
	@paramBrandCode			VARCHAR(11),
	@paramProductionDate	DATETIME,
	@paramProductionDateTo	DATETIME
)
AS
BEGIN
	DECLARE @dateCurr	DATETIME =  @paramProductionDate;
	--DECLARE @dateTo		DATETIME = CONVERT(DATE,'2016-10-16'); --CONVERT(DATE, GETDATE());
	DECLARE @dateTo		DATETIME = @paramProductionDateTo; --CONVERT(DATE, GETDATE());

	DECLARE @brandGroupCode VARCHAR(20);
	SELECT @brandGroupCode = BrandGroupCode FROM MstGenBrand WHERE BrandCode = @paramBrandCode;

	WHILE(@dateCurr <= @dateTo)
	BEGIN
		DECLARE @kpsWeek INT, @kpsYear INT;
		SELECT TOP 1 @kpsWeek = [Week], @kpsYear = [YEar] FROM MstGenWeek WHERE @dateCurr BETWEEN StartDate AND EndDate

		-- Delete first
		DELETE ExeReportByProcess WHERE LocationCode = @paramLocationCode AND UnitCode = @paramUnitCode AND BrandCode = @paramBrandCode AND ProductionDate = @dateCurr
		
		EXEC [dbo].[InsertDefaultExeReportByProcess] @paramLocationCode, @paramBrandCode, @paramUnitCode, @kpsYear, @kpsWeek, @dateCurr, 'SYSTEM', 'SYSTEM' 
		
		DECLARE @processGroup VARCHAR(16);
		DECLARE @processOrder INT;

		IF((SELECT dbo.GetParentLocationLastChild(@paramLocationCode)) = 'PLANT')
		BEGIN
			
			DECLARE cursor_processPlant CURSOR LOCAl FOR
			SELECT DISTINCT ProcessGroup, ProcessOrder FROM ExePlantProductionEntryVerification 
			WHERE LocationCode = @paramLocationCode AND UnitCode = @paramUnitCode AND BrandCode = @paramBrandCode
			AND ProductionDate = @dateCurr AND KPSWeek = @kpsWeek AND KPSYear = @kpsYear
			ORDER BY ProcessOrder desc
			
			OPEN cursor_processPlant

			FETCH NEXT FROM cursor_processPlant
			INTO @processGroup, @processOrder

			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC BaseExeReportByProcess @paramLocationCode, @paramBrandCode, @processGroup, @kpsYear, @kpsWeek, 'SYSTEM', 'SYSTEM', @dateCurr, @paramUnitCode
				
				FETCH NEXT FROM cursor_processPlant
				INTO @processGroup, @processOrder
			END

			CLOSE cursor_processPlant;  
			DEALLOCATE cursor_processPlant; 
			
		END
		ELSE
		BEGIN
			DECLARE cursor_processTPO CURSOR LOCAl FOR
			SELECT DISTINCT ProcessGroup, ProcessOrder FROM ExeTPOProductionEntryVerification 
			WHERE LocationCode = @paramLocationCode AND BrandCode = @paramBrandCode
			AND ProductionDate = @dateCurr AND KPSWeek = @kpsWeek AND KPSYear = @kpsYear
			ORDER BY ProcessOrder desc
			
			OPEN cursor_processTPO

			FETCH NEXT FROM cursor_processTPO
			INTO @processGroup, @processOrder

			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC BaseExeReportByProcess @paramLocationCode, @paramBrandCode, @processGroup, @kpsYear, @kpsWeek, 'SYSTEM', 'SYSTEM', @dateCurr, 'PROD'
				
				FETCH NEXT FROM cursor_processTPO
				INTO @processGroup, @processOrder
			END

			CLOSE cursor_processTPO;  
			DEALLOCATE cursor_processTPO; 
		END

		DECLARE @adjLocationCode VARCHAR(8);
		DECLARE @adjBrandCode VARCHAR(11);
		DECLARE @adjProductionDate DATETIME;
		DECLARE @adjUnitCode VARCHAR(4);
		DECLARE @adjType VARCHAR(64);
		DECLARE @adjRemark VARCHAR(128);
		DECLARE @adjValue INT;
		DECLARE @adjcreatedBy VARCHAR(64);
		DECLARE @adjupdatedBy VARCHAR(64);
		DECLARE @adjGroupCode VARCHAR(4);
		DECLARE @adjShift INT;
		DECLARE @adjProcess VARCHAR(16);

		------------------ Adjustment  -----------
		DECLARE cursor_adj CURSOR LOCAl FOR
		SELECT LocationCode, BrandCode, ProductionDate, UnitCode, AdjustmentType, AdjustmentRemark, AdjustmentValue, CreatedBy, UpdatedBy, GroupCode, [Shift], ProcessGroup
		FROM ProductAdjustment WHERE LocationCode = @paramLocationCode AND BrandCode = @paramBrandCode AND ProductionDate = @dateCurr

		OPEN cursor_adj

		FETCH NEXT FROM cursor_adj
		INTO @adjLocationCode, @adjBrandCode, @adjProductionDate, @adjUnitCode, @adjType, @adjRemark, 
		@adjValue, @adjcreatedBy, @adjupdatedBy, @adjGroupCode, @adjShift, @adjProcess;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @adjValueDif INT;
			DECLARE @TempAjdBrandGroupCode VARCHAR(20);
			DECLARE @dbResultAdj TABLE ([ProductionDate] DATETIME,
									[UnitCode] VARCHAR(4),
									[LocationCode] VARCHAR(8),
									[Shift] INT,
									[BrandCode] VARCHAR(11),
									[ProcessGroup] VARCHAR(16),
									[AdjustmentType] VARCHAR(64),
									[CreatedDate] DATETIME,
									[CreatedBy] VARCHAR(64),
									[UpdatedDate] DATETIME,
									[UpdatedBy] VARCHAR(64),
									[AdjustmentValue] INT,
									[AdjustmentRemark] VARCHAR(128),
									[StatusEmp] VARCHAR(16),
									[StatusIdentifier] CHAR(1),
									[GroupCode] VARCHAR(4));

			--GET BrandGroupCode FROM MstGenBrand AND SET TO VAR
			SET @TempAjdBrandGroupCode = (SELECT BrandGroupCode FROM MstGenBrand WHERE BrandCode = @adjBrandCode);

			-- GET DATA FROM MstGenProcessSettings BY STEP BEFORE
			SET @adjProcess = (SELECT TOP 1 ProcessGroup FROM MstGenProcessSettings WHERE BrandGroupCode = @TempAjdBrandGroupCode);

			INSERT INTO @dbResultAdj
			SELECT * 
			FROM ProductAdjustment 
			WHERE ProductionDate = @adjProductionDate AND UnitCode = @adjUnitCode AND LocationCode = @adjLocationCode
			AND Shift = @adjShift AND BrandCode = @adjBrandCode AND ProcessGroup = @adjProcess AND AdjustmentType = @adjType;

			IF(@adjType = '0/External Move' OR @adjType = '0/Panel Cigarettes')
			BEGIN
				DECLARE @reportLocationCode VARCHAR(8);
				DECLARE @reportUnitCode VARCHAR(4);
				DECLARE @reportBrandCode VARCHAR(11);
				DECLARE @reportKPSYear INT;
				DECLARE @reportKPSWeek INT;
				DECLARE @reportProductionDate DATETIME;
				DECLARE @reportProcessGroup VARCHAR(11);
				DECLARE @reportProcessOrder INT;
				DECLARE @reportShift INT;
				DECLARE @reportDescription VARCHAR(64);
				DECLARE @reportUOM VARCHAR(11);
				DECLARE @reportUomOrder INT;
				DECLARE @reportProduction FLOAT;
				DECLARE @reportKeluarBersih FLOAT;
				DECLARE @reportRejectSample FLOAT;
				DECLARE @reportBeginningStock FLOAT;
				DECLARE @reportEndingStock FLOAT;
				DECLARE @reportCreatedDate  DATETIME;
				DECLARE @reportCreatedBy  VARCHAR(64);
				DECLARE @reportUpdatedDate  DATETIME;
				DECLARE @reportUpdatedBy  VARCHAR(64);

				DECLARE @dbResultReport TABLE(
				LocationCode VARCHAR(8), 
				UnitCode VARCHAR(4), 
				BrandCode VARCHAR(11), 
				KPSYear  INT,
				KPSWeek  INT,
				ProductionDate DATETIME,
				ProcessGroup  VARCHAR(16),
				ProcessOrder  INT,
				Shift  INT,
				Description  VARCHAR(64),
				UOM  VARCHAR(11),
				UOMOrder INT, 
				Production FLOAT,
				KeluarBersih FLOAT,
				RejectSample FLOAT,
				BeginningStock FLOAT,  
				EndingStock FLOAT, 
				CreatedDate  DATETIME,
				CreatedBy  VARCHAR(64),
				UpdatedDate  DATETIME,
				UpdatedBy  VARCHAR(64),
				PRIMARY KEY (LocationCode, UnitCode, BrandCode, ProductionDate,UOMOrder,ProcessOrder));

				IF(@adjType = '0/Panel Cigarettes')
				BEGIN
					DELETE FROM @dbResultReport 
					INSERT INTO @dbResultReport 
					--SELECT Production, RejectSample, KeluarBersih, EndingStock, UOMOrder, BeginningStock,
					--LocationCode,UnitCode, BrandCode, ProductionDate
					SELECT *
					FROM ExeReportByProcess
					WHERE LocationCode = @adjLocationCode AND
					UnitCode = @adjUnitCode AND
					BrandCode = @adjBrandCode AND
					ProductionDate = @adjProductionDate AND
					ProcessGroup = 'STAMPING' AND
					(UOMOrder = 7 OR UOMOrder = 8 OR UOMOrder = 8 OR UOMOrder = 9 OR UOMOrder = 10 OR UOMOrder = 11 OR UOMOrder = 12 OR UOMOrder = 13 OR UOMOrder = 14)
					ORDER BY UOMOrder ASC;
				END
				ELSE IF(@adjType = '0/External Move')
				BEGIN
					DELETE FROM @dbResultReport 
					INSERT INTO @dbResultReport 
					--SELECT Production, RejectSample, KeluarBersih, EndingStock, UOMOrder, BeginningStock,
					--LocationCode,UnitCode, BrandCode, ProductionDate
					SELECT *
					FROM ExeReportByProcess
					WHERE LocationCode = @adjLocationCode AND
					UnitCode = @adjUnitCode AND
					BrandCode = @adjBrandCode AND
					ProductionDate = @adjProductionDate AND
					ProcessGroup = 'STAMPING' AND
					(UOMOrder = 11 OR UOMOrder = 12 OR UOMOrder = 13 OR UOMOrder = 14)
					ORDER BY UOMOrder ASC;
				END

				DECLARE @keluarBersih TABLE (kbIndex INT PRIMARY KEY, kbValue REAL);
				DECLARE @beginingStock TABLE (bsIndex INT PRIMARY KEY, bsValue REAL);
				INSERT INTO @keluarBersih VALUES(0,0.0);
				INSERT INTO @keluarBersih VALUES(1,0.0);
				INSERT INTO @keluarBersih VALUES(2,0.0);
				INSERT INTO @keluarBersih VALUES(3,0.0);
				INSERT INTO @keluarBersih VALUES(4,0.0);
				INSERT INTO @keluarBersih VALUES(5,0.0);
				INSERT INTO @keluarBersih VALUES(6,0.0);
				INSERT INTO @keluarBersih VALUES(7,0.0);
				INSERT INTO @keluarBersih VALUES(8,0.0);
				INSERT INTO @keluarBersih VALUES(9,0.0);
				INSERT INTO @keluarBersih VALUES(10,0.0);
				INSERT INTO @keluarBersih VALUES(11,0.0);
				INSERT INTO @keluarBersih VALUES(12,0.0);
				INSERT INTO @keluarBersih VALUES(13,0.0);

				INSERT INTO @beginingStock VALUES(0,0.0);
				INSERT INTO @beginingStock VALUES(1,0.0);
				INSERT INTO @beginingStock VALUES(2,0.0);
				INSERT INTO @beginingStock VALUES(3,0.0);
				INSERT INTO @beginingStock VALUES(4,0.0);
				INSERT INTO @beginingStock VALUES(5,0.0);
				INSERT INTO @beginingStock VALUES(6,0.0);
				INSERT INTO @beginingStock VALUES(7,0.0);
				INSERT INTO @beginingStock VALUES(8,0.0);
				INSERT INTO @beginingStock VALUES(9,0.0);
				INSERT INTO @beginingStock VALUES(10,0.0);
				INSERT INTO @beginingStock VALUES(11,0.0);
				INSERT INTO @beginingStock VALUES(12,0.0);
				INSERT INTO @beginingStock VALUES(13,0.0);
				--LOOP EXE REPORT BY PROCESS

				DECLARE cursor_exeReportByProcess CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
				SELECT * FROM @dbResultReport
				OPEN cursor_exeReportByProcess
				FETCH NEXT FROM cursor_exeReportByProcess INTO   @reportLocationCode, @reportUnitCode, @reportBrandCode, @reportKPSYear, @reportKPSWeek
				  ,@reportProductionDate ,@reportProcessGroup , @reportProcessOrder, @reportShift , @reportDescription, @reportUOM, @reportUomOrder, @reportProduction  
				  ,@reportKeluarBersih,@reportRejectSample,@reportBeginningStock,@reportEndingStock,@reportCreatedDate,@reportCreatedBy,@reportUpdatedDate,@reportUpdatedBy 
				WHILE @@FETCH_STATUS = 0 BEGIN 
					DECLARE @brandGroupReport VARCHAR(20);
					DECLARE @packReport TABLE ([StickPerPack] INT, [StickPerSlof] INT, [StickPerBox] INT);
					DECLARE @oldEndingStock FLOAT; 
					DELETE FROM @packReport
					SELECT @brandGroupReport= BrandGroupCode FROM MstGenBrand WHERE BrandCode = @adjBrandCode;
					INSERT INTO @packReport
					SELECT StickPerPack, StickPerSlof, StickPerBox FROM MstGenBrandGroup WHERE BrandGroupCode = @brandGroupReport;
					
					SET @oldEndingStock = @reportEndingStock;

					IF(@reportUomOrder = 7) -- StampPack Stick
					BEGIN
						SET @reportRejectSample = CONVERT(REAL, @adjValue);
						SET @reportKeluarBersih = @reportProduction - @reportRejectSample;
						
						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 7
					END
					ELSE IF(@reportUomOrder = 8) -- StampPack Pack
					BEGIN
						SET @reportRejectSample = CONVERT(REAL, @adjValue) / (SELECT StickPerPack FROM @packReport);
						SET @reportKeluarBersih = @reportProduction - @reportRejectSample;

						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 8
					END
					ELSE IF(@reportUomOrder = 9) -- displaycartoon stick
					BEGIN
						SET @reportProduction = (SELECT kbValue FROM @keluarBersih where kbIndex = 7);
						SET @reportKeluarBersih = @reportProduction;
						
						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 9
						SET @reportRejectSample = 0;
						SET @reportEndingStock = 0;
					END
					ELSE IF(@reportUomOrder = 10) -- DisplayCarton Slof
					BEGIN
						SET @reportProduction = ((SELECT kbValue FROM @keluarBersih where kbIndex = 7) / (ROUND(CONVERT(REAL,(SELECT StickPerSlof FROM @packReport)),2)));
						SET @reportKeluarBersih = @reportProduction;
						
						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 10
						SET @reportRejectSample = 0;
						SET @reportEndingStock = 0;
					END
					ELSE IF(@reportUomOrder = 11) --InternalMove Stick
					BEGIN
						IF(@adjType != '0/External Move')
						BEGIN
							SET @reportProduction = (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 9);
							SET @reportKeluarBersih = @reportProduction;
							SET @reportRejectSample = 0;
							SET @reportEndingStock = 0;
						END

						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 11;
						UPDATE @beginingStock SET bsValue = @reportBeginningStock
						WHERE bsIndex = 11;
					END
					ELSE IF(@reportUomOrder = 12) -- InternalMove Box
					BEGIN
						IF(@adjType != '0/External Move')
						BEGIN
							SET @reportProduction = ((SELECT kbValue FROM @keluarBersih WHERE kbIndex = 9) / (ROUND(CONVERT(REAL,(SELECT StickPerBox FROM @packReport)),2)));
							SET @reportKeluarBersih = @reportProduction;
							SET @reportRejectSample = 0;
							SET @reportEndingStock = 0;
						END
						
						UPDATE @keluarBersih SET kbValue = @reportKeluarBersih
						WHERE kbIndex = 12;
						UPDATE @beginingStock SET bsValue = @reportBeginningStock
						WHERE bsIndex = 12;
					END
					ELSE IF(@reportUomOrder = 13) --ExternalMove Stick
					BEGIN
						IF(@adjType = '0/Panel Cigarettes')
						BEGIN
							SET @reportEndingStock = (@reportBeginningStock + (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 11)) - @reportKeluarBersih;
							--get deltavalue
							DECLARE @delta FLOAT;
							SET @delta = @reportEndingStock - @oldEndingStock;
							UPDATE ExeReportByProcess SET EndingStock = ISNULL(EndingStock + @delta, 0.0),
							BeginningStock = ISNULL(BeginningStock + @delta, 0.0)
							WHERE LocationCode = @reportLocationCode AND
							UnitCode = @reportUnitCode AND
							BrandCode = @reportBrandCode AND
							ProductionDate > @reportProductionDate AND
							UOMOrder = @reportUomOrder;
							---------------------------------------------------------------
							SET @reportProduction = 0;
						END
						ELSE IF(@adjType = '0/External Move')
						BEGIN
							SET @reportKeluarBersih = CONVERT(float, @adjValue);
							SET @reportEndingStock = (@reportBeginningStock + (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 11)) - @reportKeluarBersih;
							IF(@adjValue = 0)
							BEGIN
								SET @reportEndingStock = (@reportBeginningStock + CASE (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 11) WHEN NULL THEN 0.0 END) - @reportKeluarBersih;
							END
							-- get deltavalue
							DECLARE @delta1 FLOAT;
							SET @delta1 = @reportEndingStock - @oldEndingStock;

							UPDATE ExeReportByProcess SET EndingStock = ISNULL(EndingStock + @delta1, 0.0), --CASE EndingStock + @delta1 WHEN NULL THEN 0.0 END, 
							BeginningStock = ISNULL(BeginningStock + @delta1,0.0) --CASE BeginningStock + @delta1 WHEN NULL THEN 0.0 END
							WHERE LocationCode = @reportLocationCode AND
							UnitCode = @reportUnitCode AND
							BrandCode = @reportBrandCode AND
							ProductionDate > @reportProductionDate AND
							UOMOrder = @reportUomOrder;
							---------------------------------------------------------------
							SET @reportRejectSample = 0;
						END
					END
					ELSE IF(@reportUomOrder = 14) -- ExternalMove Box
					BEGIN
						IF(@adjType = '0/Panel Cigarettes')
						BEGIN
							SET @reportEndingStock = (@reportBeginningStock + (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 12)) - @reportKeluarBersih;
							--get deltavalue
							DECLARE @delta2 FLOAT;
							SET @delta2 = @reportEndingStock - @oldEndingStock;
							
							UPDATE ExeReportByProcess SET EndingStock = ISNULL(EndingStock + @delta2, 0.0), --CASE EndingStock + @delta2 WHEN NULL THEN 0.0 END, 
							BeginningStock = ISNULL(BeginningStock + @delta2, 0.0) --CASE BeginningStock + @delta2 WHEN NULL THEN 0.0 END
							WHERE LocationCode = @reportLocationCode AND
							UnitCode = @reportUnitCode AND
							BrandCode = @reportBrandCode AND
							ProductionDate > @reportProductionDate AND
							UOMOrder = @reportUomOrder;
							---------------------------------------------------------------
							SET @reportProduction = 0;
						END
						ELSE IF(@adjType = '0/External Move')
						BEGIN
							SET @reportKeluarBersih = CONVERT(float, @adjValue) / CONVERT(float, (SELECT StickPerBox FROM @packReport));
							SET @reportEndingStock = (@reportBeginningStock + (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 12)) - @reportKeluarBersih;
							IF(@adjValue = 0)
							BEGIN
								SET @reportEndingStock = (@reportBeginningStock + (SELECT kbValue FROM @keluarBersih WHERE kbIndex = 12)) - @reportKeluarBersih;
							END

							-- get deltavalue
							DECLARE @delta3 FLOAT;
							SET @delta3 = @reportEndingStock - @oldEndingStock;
							
							UPDATE ExeReportByProcess SET EndingStock = ISNULL(EndingStock + @delta3,0.0), --CASE EndingStock + @delta3 WHEN NULL THEN 0.0 END, 
							BeginningStock = ISNULL(BeginningStock + @delta3, 0.0) --CASE BeginningStock + @delta3 WHEN NULL THEN 0.0 END
							WHERE LocationCode = @reportLocationCode AND
							UnitCode = @reportUnitCode AND
							BrandCode = @reportBrandCode AND
							ProductionDate > @reportProductionDate AND
							UOMOrder = @reportUomOrder;
							---------------------------------------------------------------
							SET @reportRejectSample = 0;
						END
					END

					UPDATE ExeReportByProcess SET Production = ISNULL(@reportProduction,0),
					RejectSample = ISNULL(@reportRejectSample,0),
					KeluarBersih = ISNULL(@reportKeluarBersih,0),
					EndingStock = ISNULL(@reportEndingStock,0),
					BeginningStock = ISNULL(@reportBeginningStock,0)
					WHERE LocationCode = @reportLocationCode AND
					UnitCode = @reportUnitCode AND
					BrandCode = @reportBrandCode AND
					ProductionDate = @reportProductionDate AND
					ProcessGroup = 'STAMPING' AND
					UOMOrder = @reportUomOrder;

				

				FETCH NEXT FROM cursor_exeReportByProcess INTO @reportLocationCode, @reportUnitCode, @reportBrandCode, @reportKPSYear, @reportKPSWeek
				  ,@reportProductionDate ,@reportProcessGroup , @reportProcessOrder, @reportShift , @reportDescription, @reportUOM, @reportUomOrder, @reportProduction  
				  ,@reportKeluarBersih,@reportRejectSample,@reportBeginningStock,@reportEndingStock,@reportCreatedDate,@reportCreatedBy,@reportUpdatedDate,@reportUpdatedBy 
				END
				CLOSE cursor_exeReportByProcess
				DEALLOCATE cursor_exeReportByProcess
				DELETE FROM @keluarBersih
				DELETE FROM @beginingStock
			END

			FETCH NEXT FROM cursor_adj
			INTO @adjLocationCode, @adjBrandCode, @adjProductionDate, @adjUnitCode, @adjType, @adjRemark, @adjValue, @adjcreatedBy, @adjupdatedBy, @adjGroupCode, @adjShift, @adjProcess
		END

		CLOSE cursor_adj;  
		DEALLOCATE cursor_adj; 
		------------------ Adjustment  -----------
		EXEC RecalculateStockExeReportByProcess @dateCurr,@paramBrandCode,@dateCurr
		
		-- --------------------------------------------

		SET @dateCurr = DATEADD(DAY, 1, @dateCurr);
	END
	DECLARE @TableWeek TABLE ([Year] int, [Week] int, [StartDate] DATETIME);
	DECLARE @CountWeek INT;
	DECLARE @Week INT;
	DECLARE @Year INT;

	INSERT INTO @TableWeek
	SELECT [Year],[Week], StartDate FROM MstGenWeek WHERE (StartDate <= @paramProductionDate AND EndDate >= @paramProductionDate) OR 
	(StartDate <= @dateTo AND EndDate >= @dateTo) OR
	(StartDate >= @paramProductionDate AND StartDate <=@dateTo);

	SET @CountWeek = (SELECT COUNT(*) from @TableWeek);
	DECLARE @startDateWeek DATETIME;

	IF(@CountWeek > 0)
	BEGIN
		DECLARE CURSOR_week CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
		SELECT [Year],[Week],[StartDate] from @TableWeek 
		OPEN CURSOR_week
		FETCH NEXT FROM CURSOR_week INTO @Year,@Week, @startDateWeek
		WHILE @@FETCH_STATUS = 0 BEGIN 
			IF((SELECT dbo.GetParentLocationLastChild(@paramLocationCode)) = 'TPO')
			BEGIN
				exec TPOProductionEntryVerificationGenerateReport @paramLocationCode, @paramBrandCode, 
				@Year,@Week,@startDateWeek, 'system'
			END
		FETCH NEXT FROM CURSOR_week INTO @Year,@Week, @startDateWeek
		END
		CLOSE CURSOR_week
		DEALLOCATE CURSOR_week
		
	END


END