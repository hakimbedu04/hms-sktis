SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description: Penyesuaian actual
-- Ticket: http://tp.voxteneo.co.id/entity/3915
-- Author: Dwi Yudha
-- Version: 2.6
-- =============================================

-- =============================================
-- Description: add parameter username
-- Ticket: http://tp.voxteneo.co.id/entity/10463
-- Author: Azka
-- Version: 2.7
-- Date: 2016/10/17
-- =============================================

ALTER PROCEDURE [dbo].[SP_SubmitTpoTpk]
      @LocationCode VARCHAR(8),
      @BrandCode    VARCHAR(11),
      @KPSYear      INT,
      @KPSWeek      INT,
	  @UserName VARCHAR(32)
AS
    BEGIN
        SET NOCOUNT ON;
		SET DATEFIRST 1;
        -- GLOBAL VARIABLE
        DECLARE @TPOFeeCode VARCHAR(64);
        DECLARE @StickPerBox INT;

        -- GLOBAL DATA
        -- Master General Brand Group	
        DECLARE @BrandGroupCode VARCHAR(20);
        SELECT @BrandGroupCode = bg.BrandGroupCode,
               @StickPerBox = bg.StickPerBox
		FROM MstGenBrandGroup bg
		INNER JOIN MstGenBrand b ON b.BrandGroupCode = bg.BrandGroupCode
		WHERE b.BrandCode = @BrandCode;
        
		SET @TPOFeeCode = 'FEE/' + @LocationCode + '/' + @BrandGroupCode + '/' + CAST(@KPSYear AS VARCHAR) + '/' + CAST(@KPSWeek AS VARCHAR);

        -- Check TPO Fee Header
        DECLARE @TPOFeeHdrExist INT;
        SELECT @TPOFeeHdrExist = COUNT(*)
        FROM TPOFeeHdrPlan
        WHERE KPSYear = @KPSYear
          AND KPSWeek = @KPSWeek
          AND BrandGroupCode = @BrandGroupCode
          AND LocationCode = @LocationCode;
	  
		DECLARE @StartDate DATETIME;
		DECLARE @EndDate DATETIME;
		SELECT @StartDate = StartDate,
			   @EndDate = EndDate
		FROM MstGenWeek
		WHERE Year = @KPSYear
		  AND Week = @KPSWeek;
		  
		-- Master TPO Package
		DECLARE @TPOPackageValue REAL;
		SELECT @TPOPackageValue = Package
		FROM MstTPOPackage
		WHERE LocationCode = @LocationCode
		  AND BrandGroupCode = @BrandGroupCode
		  AND EffectiveDate <= @StartDate
		  AND ExpiredDate >= @EndDate
		ORDER BY Package DESC;
		  
		IF(@TPOPackageValue IS NULL)
		BEGIN
			SET @TPOPackageValue = 0;
		END;
		--adding by bagus ( for round package value )
		set @TPOPackageValue = Round(@TPOPackageValue,2);
		
        IF @TPOFeeHdrExist <= 0
            BEGIN	
                -- Insert TPO Fee Header
                INSERT INTO TPOFeeHdrPlan
                       ( KPSYear,
                         KPSWeek,
                         BrandGroupCode,
                         LocationCode,
                         CreatedDate,
                         CreatedBy,
                         UpdatedDate,
                         UpdatedBy,
                         TPOFeeCode,
                         Status,
                         StickPerBox,
                         TPOPackageValue
                       )
                       SELECT @KPSYear,
                              @KPSWeek,
                              @BrandGroupCode,
                              @LocationCode,
                              GETDATE(),
                              @UserName,
                              GETDATE(),
                              @UserName,
                              @TPOFeeCode,
                              'DRAFT',                       
                              @StickPerBox,
                              @TPOPackageValue;
            END;
		ELSE
			BEGIN
				-- Update TPO Fee Header
				UPDATE TPOFeeHdrPlan
				SET UpdatedDate = GETDATE(),
					TPOFeeCode = @TPOFeeCode,
					Status = 'DRAFT',
					StickPerBox = @StickPerBox,
					TPOPackageValue = @TPOPackageValue
				WHERE KPSYear = @KPSYear
				  AND KPSWeek = @KPSWeek
				  AND BrandGroupCode = @BrandGroupCode
				  AND LocationCode = @LocationCode;
			END;

        -- Check TPO Fee Production Dialy
        DECLARE @TPOFeeProductionDailyExist INT;
        SELECT @TPOFeeProductionDailyExist = COUNT(*)
        FROM TPOFeeProductionDailyPlan
        WHERE TPOFeeCode = @TPOFeeCode
          AND FeeDate >= @StartDate
          AND FeeDate <= @EndDate
          AND KPSYear = @KPSYear
          AND KPSWeek = @KPSWeek;
		
        DECLARE @HDRTotalProdBox FLOAT = 0;
        DECLARE @HDRTotalProdStick FLOAT = 0;
        DECLARE @HDRTotalProdJKN FLOAT = 0;
        DECLARE @HDRTotalProdJl1 FLOAT = 0;
        DECLARE @HDRTotalProdJl2 FLOAT = 0;
        DECLARE @HDRTotalProdJl3 FLOAT = 0;
        DECLARE @HDRTotalProdJl4 FLOAT = 0;
		
		DECLARE @TempTable TABLE
		(
		  BrandCode VARCHAR(11)
		)
		
		-- TEMPORARY TABLE TO CALCULATE RUPIAH AT THE END OF PROCESS --
		DECLARE @DailyJK TABLE
		(
			FeeDate DATETIME,
			JKN FLOAT,
			JL1 FLOAT,
			JL2 FLOAT,
			JL3 FLOAT,
			JL4 FLOAT,
			JKNJam FLOAT,
			JL1Jam FLOAT,
			JL2Jam FLOAT,
			JL3Jam FLOAT,
			JL4Jam FLOAT
		)
		
		DECLARE @TempBrandCode VARCHAR(11);
		DECLARE @LogExist INT;
		
		WHILE @StartDate <= @EndDate
			BEGIN
			
				-- LOOPING EACH BRAND IN BRAND GROUP ON EACH DAY --
				
				INSERT INTO @TempTable 
				SELECT BrandCode FROM MstGenBrand
				WHERE BrandGroupCode = @BrandGroupCode;

				DECLARE @OutputboxSum FLOAT = 0;
				DECLARE @OutputSticksSum FLOAT = 0;
				DECLARE @JKNJamSum FLOAT = 0;
				DECLARE @Jl1JamSum FLOAT = 0;
				DECLARE @Jl2JamSum FLOAT = 0;
				DECLARE @Jl3JamSum FLOAT = 0;
				DECLARE @Jl4JamSum FLOAT = 0;
				DECLARE @JKNJamStdSum FLOAT = 0;
				DECLARE @Jl1JamStdSum FLOAT = 0;
				DECLARE @Jl2JamStdSum FLOAT = 0;
				DECLARE @Jl3JamStdSum FLOAT = 0;
				DECLARE @Jl4JamStdSum FLOAT = 0;
				
				WHILE((SELECT COUNT(*) FROM @TempTable) > 0)
				BEGIN
					SET @TempBrandCode = (Select Top 1 BrandCode From @TempTable);
					
					SELECT @LogExist = COUNT(*)
					FROM UtilTransactionLogs
					WHERE TransactionCode = 'TPK/' + @LocationCode + '/' + @TempBrandCode + '/' + CAST(@KPSYear AS VARCHAR) + '/' + CAST(@KPSWeek AS VARCHAR);
					
					IF (@LogExist > 0 OR @TempBrandCode = @BrandCode)
						BEGIN
							-- REAL CALCULATION LOOP AND SUMARIZING --
							DECLARE @Outputbox FLOAT;
							SELECT @Outputbox = CASE
									WHEN DATENAME(dw, @StartDate) = 'Monday'
									THEN TargetManual1
									WHEN DATENAME(dw, @StartDate) = 'Tuesday'
									THEN TargetManual2
									WHEN DATENAME(dw, @StartDate) = 'Wednesday'
									THEN TargetManual3
									WHEN DATENAME(dw, @StartDate) = 'Thursday'
									THEN TargetManual4
									WHEN DATENAME(dw, @StartDate) = 'Friday'
									THEN TargetManual5
									WHEN DATENAME(dw, @StartDate) = 'Saturday'
									THEN TargetManual6
									WHEN DATENAME(dw, @StartDate) = 'Sunday'
									THEN TargetManual7
								END
							FROM PlanTPOTargetProductionKelompokBox
							WHERE LocationCode = @LocationCode
								AND KPSYear = @KPSYear
								AND KPSWeek = @KPSWeek
								AND BrandCode = @TempBrandCode;

							IF (@Outputbox IS NULL)
								BEGIN
									SET @Outputbox = 0;
								END;
								
							-- PlanTPOTargetProductionKelompok
							DECLARE @OutputSticks REAL;
							SET @OutputSticks = @Outputbox * @StickPerBox;
							  
							DECLARE @JKNJam FLOAT;
							DECLARE @Jl1Jam FLOAT;
							DECLARE @Jl2Jam FLOAT;
							DECLARE @Jl3Jam FLOAT;
							DECLARE @Jl4Jam FLOAT;
							DECLARE @JKNJamStd FLOAT;
							DECLARE @Jl1JamStd FLOAT;
							DECLARE @Jl2JamStd FLOAT;
							DECLARE @Jl3JamStd FLOAT;
							DECLARE @Jl4JamStd FLOAT;

							-- Check whether ProductionDate is Holiday (it so skip to JL2Jam)
							DECLARE @IsHoliday INT;
							SELECT @IsHoliday = COUNT(*)
							FROM MstGenHoliday
							WHERE HolidayDate = @StartDate
								AND LocationCode = @LocationCode
								AND StatusActive = '1';

							-- Standar Hours
							SELECT @JKNJamStd = JknHour,
								   @Jl1JamStd = Jl1Hour,
								   @Jl2JamStd = Jl2Hour,
								   @Jl3JamStd = Jl3Hour,
								   @Jl4JamStd = Jl4Hour
							FROM MstGenStandardHours
							WHERE UPPER(DayType) = UPPER(CASE
															 WHEN @IsHoliday > 0
															 THEN 'HOLIDAY'
															 ELSE 'NON-HOLIDAY'
														 END)
							  AND Day = DATEPART(dw, @StartDate);
							  
							DECLARE @JKN FLOAT,
								@JL1 FLOAT,
								@JL2 FLOAT,
								@JL3 FLOAT,
								@JL4 FLOAT,
								@StdStickPerHours INT,
								@EmpPackage       INT;
				
							-- SET @StdStickPerHours
							SELECT TOP 1 @StdStickPerHours = StdStickPerHour FROM ProcessSettingsAndLocationView
							WHERE dbo.ProcessSettingsAndLocationView.LocationCode = @LocationCode
							  AND dbo.ProcessSettingsAndLocationView.BrandGroupCode = @BrandGroupCode
							  AND dbo.ProcessSettingsAndLocationView.ProcessGroup = 'ROLLING';

							IF (@StdStickPerHours IS NULL)
								BEGIN
									SET @StdStickPerHours = 0;
								END;
								
							-- SET @EmpPackage
							SELECT @EmpPackage = dbo.MstGenBrandGroup.EmpPackage
							FROM MstGenBrandGroup
							WHERE dbo.MstGenBrandGroup.BrandGroupCode = @BrandGroupCode;
							
							IF (@EmpPackage IS NULL)
								BEGIN
									SET @EmpPackage = 0;
								END;
								
							--SELECT @StdStickPerHours = mgps.StdStickPerHour FROM dbo.MstGenProcessSettings mgps WHERE mgps.BrandGroupCode = @BrandGroupCode AND mgps.ProcessGroup='ROLLING'
							-- TODO JKN
							-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
							-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JKNJam * 
							-- (Get data EmpPackage from MstGenBrandGroup where Filter) * @StickPerBox
							-- SET @JKN = ROUND((( @TPOPackageValue * @StdStickPerHours * @JKNJamStd * @EmpPackage ) / @StickPerBox ), 2);
							SET @JKN = (( @TPOPackageValue * @StdStickPerHours * @JKNJamStd * @EmpPackage ) / @StickPerBox );

							-- TODO JL1
							-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
							-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL1Jam * 
							-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
							-- SET @JL1 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl1JamStd * @EmpPackage ) / @StickPerBox ), 2);
							SET @JL1 = (( @TPOPackageValue * @StdStickPerHours * @Jl1JamStd * @EmpPackage ) / @StickPerBox );

							-- TODO JL2
							-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
							-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL2Jam * 
							-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
							-- SET @JL2 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl2JamStd * @EmpPackage ) / @StickPerBox ), 2);
							SET @JL2 = (( @TPOPackageValue * @StdStickPerHours * @Jl2JamStd * @EmpPackage ) / @StickPerBox );

							-- TODO JL3
							-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
							-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL3Jam * 
							-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
							-- SET @JL3 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl3JamStd * @EmpPackage ) / @StickPerBox ), 2);
							SET @JL3 = (( @TPOPackageValue * @StdStickPerHours * @Jl3JamStd * @EmpPackage ) / @StickPerBox );

							-- TODO JL4
							-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
							-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL4Jam * 
							-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
							-- SET @JL4 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl4JamStd * @EmpPackage ) / @StickPerBox ), 2);
							SET @JL4 = (( @TPOPackageValue * @StdStickPerHours * @Jl4JamStd * @EmpPackage ) / @StickPerBox );

							DECLARE @Sisa FLOAT;
							SET @Sisa = ROUND((@Outputbox - @JKN),6);	  
								
							IF @IsHoliday <= 0 
							-- IF NOT HOLIDAY
							BEGIN
								IF (@Outputbox < @JKN)
									BEGIN
										SET @JKN = @OutputBox;
										SET @JL1 = 0;
										SET @JL2 = 0;
										SET @JL3 = 0;
										SET @JL4 = 0;
									END
								ELSE IF( @Sisa < @JL1 )
									BEGIN
										SET @JL1 = @Sisa;
										SET @JL2 = 0;
										SET @JL3 = 0;
										SET @JL4 = 0;
									END;
								ELSE
									BEGIN
										SET @Sisa = ROUND((@Sisa - @JL1), 6);
										
										SET @JL2 = @Sisa;
										SET @JL3 = 0;
										SET @JL4 = 0;
									END;
								END;
							ELSE
							-- IF HOLIDAY
								BEGIN
									SET @JL1 = 0;
									SET @JL2 = @JKN;
									
									IF (@Outputbox < @JKN)
										BEGIN
											SET @JKN = 0;
											SET @JL1 = 0;
											SET @JL2 = @OutputBox;
											SET @JL3 = 0;
											SET @JL4 = 0;
										END
									ELSE IF( @Sisa < @JL3 )
										BEGIN
											SET @JL3 = @Sisa;
											SET @JL4 = 0;
										END;
									ELSE
										BEGIN
											SET @Sisa = ROUND((@Sisa - @JL3), 6);
											
											SET @JL4 = @Sisa;
										END;
								
									SET @JKN = 0;
								END;
							
							SET @JKNJamSum = @JKNJamSum + @JKN;
							SET @Jl1JamSum = @Jl1JamSum + @JL1;
							SET @Jl2JamSum = @Jl2JamSum + @JL2;
							SET @Jl3JamSum = @Jl3JamSum + @JL3;
							SET @Jl4JamSum = @Jl4JamSum + @JL4;
							
							SET @JKNJamStdSum = @JKNJamStdSum + @JKNJamStd;
							SET @Jl1JamStdSum = @Jl1JamStdSum + @Jl1JamStd;
							SET @Jl2JamStdSum = @Jl2JamStdSum + @Jl2JamStd;
							SET @Jl3JamStdSum = @Jl3JamStdSum + @Jl3JamStd;
							SET @Jl4JamStdSum = @Jl4JamStdSum + @Jl4JamStd;
							
							SET @OutputboxSum = @OutputboxSum + @Outputbox;
							SET @OutputSticksSum = @OutputSticksSum + @OutputSticks;
							
							-- END OF REAL CALCULATION LOOP --
						END;

					DELETE @TempTable Where BrandCode = @TempBrandCode;
				END
				
				-- END LOOPING EACH BRAND IN BRAND GROUP ON EACH DAY --
				
				SET @JKNJam = ROUND(@JKNJamSum, 6);
				SET @Jl1Jam = ROUND(@Jl1JamSum, 6);
				SET @Jl2Jam = ROUND(@Jl2JamSum, 6);
				SET @Jl3Jam = ROUND(@Jl3JamSum, 6);
				SET @Jl4Jam = ROUND(@Jl4JamSum, 6);
				
				SET @OutputSticks = @OutputSticksSum;
				SET @Outputbox = @OutputboxSum;
				
				IF @TPOFeeProductionDailyExist <= 0
					BEGIN
						INSERT INTO TPOFeeProductionDailyPlan
							SELECT @TPOFeeCode,
								@StartDate,
								@KPSYear,
								@KPSWeek,
								@OutputSticks,
								@Outputbox,
								@JKNJam,
								@Jl1Jam,
								@Jl2Jam,
								@Jl3Jam,
								@Jl4Jam,
								GETDATE(),
								@UserName,
								GETDATE(),
								@UserName,
								0,
								0,
								0,
								0,
								0,
								@JKNJamStdSum,
								@Jl1JamStdSum,
								@Jl2JamStdSum,
								@Jl3JamStdSum,
								@Jl4JamStdSum;
					END;
				ELSE
					BEGIN
						UPDATE TPOFeeProductionDailyPlan
						SET OuputSticks = @OutputSticks,
							OutputBox = @Outputbox,
							JKN = @JKNJam,
							JL1 = @Jl1Jam,
							Jl2 = @Jl2Jam,
							Jl3 = @Jl3Jam,
							Jl4 = @Jl4Jam,
							JKNJam = @JKNJamStdSum,
							JL1Jam = @Jl1JamStdSum,
							JL2Jam = @Jl2JamStdSum,
							JL3Jam = @Jl3JamStdSum,
							JL4Jam = @Jl4JamStdSum,
							UpdatedDate = GETDATE()
						WHERE TPOFeeCode = @TPOFeeCode
						  AND FeeDate = @StartDate;
					END;

				SET @HDRTotalProdBox = @HDRTotalProdBox + @Outputbox;
				SET @HDRTotalProdStick = @HDRTotalProdStick + @OutputSticks;
				SET @HDRTotalProdJKN = @HDRTotalProdJKN + @JKNJam;
				SET @HDRTotalProdJl1 = @HDRTotalProdJl1 + @Jl1Jam;
				SET @HDRTotalProdJl2 = @HDRTotalProdJl2 + @Jl2Jam;
				SET @HDRTotalProdJl3 = @HDRTotalProdJl3 + @Jl3Jam;
				SET @HDRTotalProdJl4 = @HDRTotalProdJl4 + @Jl3Jam;
				
				-- INSERT INTO TEMP TABLE FOR CALC RUPIAH AT THE END OF PROCESS --
				INSERT INTO @DailyJK
				 SELECT @StartDate,
						@JKNJam,
						@Jl1Jam,
						@Jl2Jam,
						@Jl3Jam,
						@Jl4Jam,
						@JKNJamStdSum,
						@Jl1JamStdSum,
						@Jl2JamStdSum,
						@Jl3JamStdSum,
						@Jl4JamStdSum
				SET @StartDate = DATEADD(DAY, 1, @StartDate);
			END;
		
		DECLARE @TotalProdBox FLOAT = @HDRTotalProdBox;
		DECLARE @TotalProdJKN FLOAT = @HDRTotalProdJKN;
		DECLARE @TotalProdJl1 FLOAT = @HDRTotalProdJl1;
		DECLARE @TotalProdJl2 FLOAT = @HDRTotalProdJl2;
		DECLARE @TotalProdJl3 FLOAT = @HDRTotalProdJl3;
		DECLARE @TotalProdJl4 FLOAT = @HDRTotalProdJl4;
		
		-- PERHITUNGAN SEBELUMNYA
		-- DECLARE @SisaDecimal FLOAT;
		-- SET @SisaDecimal = @HDRTotalProdJl4 - FLOOR(@HDRTotalProdJl4);
		-- IF @SisaDecimal > 0
		-- BEGIN
		   -- SET @HDRTotalProdJl4 = FLOOR(@HDRTotalProdJl4);
		   -- SET @HDRTotalProdJl3 = @HDRTotalProdJl3 + @SisaDecimal;
		-- END

		-- SET @SisaDecimal = @HDRTotalProdJl3 - FLOOR(@HDRTotalProdJl3);
		-- IF @SisaDecimal > 0
		-- BEGIN
		   -- SET @HDRTotalProdJl3 = FLOOR(@HDRTotalProdJl3);
		   -- SET @HDRTotalProdJl2 = @HDRTotalProdJl2 + @SisaDecimal;
		-- END

		-- SET @SisaDecimal = @HDRTotalProdJl2 - FLOOR(@HDRTotalProdJl2);
		-- IF @SisaDecimal > 0
		-- BEGIN
		   -- SET @HDRTotalProdJl2 = FLOOR(@HDRTotalProdJl2);
		   -- SET @HDRTotalProdJl1 = @HDRTotalProdJl1 + @SisaDecimal;
		-- END

		-- SET @SisaDecimal = @HDRTotalProdJl1 - FLOOR(@HDRTotalProdJl1);
		-- IF @SisaDecimal > 0
		-- BEGIN
		   -- SET @HDRTotalProdJl1 = FLOOR(@HDRTotalProdJl1);
		   -- SET @HDRTotalProdJKN = @HDRTotalProdJKN + @SisaDecimal;
		-- END

		-- SET @HDRTotalProdJKN = ROUND(@HDRTotalProdJKN, 0)
		
		-- PERHITUNGAN BARU
		
		DECLARE @TotalJKN FLOAT;
		DECLARE @TotalJL1 FLOAT;
		DECLARE @TotalJL2 FLOAT;
		DECLARE @TotalJL3 FLOAT;
		DECLARE @TotalJL4 FLOAT;

		SELECT @TotalJKN = SUM(JKNJam) FROM @DailyJK;
		SELECT @TotalJL1 = SUM(JL1Jam) FROM @DailyJK;
		SELECT @TotalJL2 = SUM(JL2Jam) FROM @DailyJK;
		SELECT @TotalJL3 = SUM(JL3Jam) FROM @DailyJK;
		SELECT @TotalJL4 = SUM(JL4Jam) FROM @DailyJK;
		
		SET @HDRTotalProdJKN = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJKN * @EmpPackage ) / @StickPerBox ), 0);
		SET @HDRTotalProdJl1 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL1 * @EmpPackage ) / @StickPerBox ), 0);
		SET @HDRTotalProdJl2 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL2 * @EmpPackage ) / @StickPerBox ), 2);
		SET @HDRTotalProdJl3 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL3 * @EmpPackage ) / @StickPerBox ), 2);
		SET @HDRTotalProdJl4 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL4 * @EmpPackage ) / @StickPerBox ), 2);
		
		DECLARE @Standard60 FLOAT = ROUND((( @TPOPackageValue * @StdStickPerHours * @EmpPackage * (@TotalJKN + @TotalJL1 + @TotalJL2) ) / @StickPerBox ), 2);
	
		-- JKN (round tanpa desimal)
		IF (ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @TotalProdJl3 - @TotalProdJl4, 0) <= @HDRTotalProdJKN)
			BEGIN
				SET @HDRTotalProdJKN = ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @TotalProdJl3 - @TotalProdJl4, 0);
			END
		
		-- JL1 (round tanpa desimal)
		IF(ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @HDRTotalProdJKN - @TotalProdJl3 - @TotalProdJl4, 0) > 0 )
			BEGIN
				IF(ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @HDRTotalProdJKN - @TotalProdJl3 - @TotalProdJl4, 0) <= @HDRTotalProdJl1)
					BEGIN
						SET @HDRTotalProdJl1 = ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @HDRTotalProdJKN - @TotalProdJl3 - @TotalProdJl4, 0);
					END
			END
		ELSE
			BEGIN
				SET @HDRTotalProdJl1 = 0;
			END
			
		-- JL2
		IF(ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @HDRTotalProdJKN - @HDRTotalProdJl1 - @TotalProdJl3 - @TotalProdJl4, 2) > 0 )
			BEGIN
				SET @HDRTotalProdJl2 = ROUND(@TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 - @HDRTotalProdJKN - @HDRTotalProdJl1 - @TotalProdJl3 - @TotalProdJl4, 2);
			END
		ELSE
			BEGIN
				SET @HDRTotalProdJl2 = 0;
			END
			
		-- JL3
		IF(@TotalProdBox - @Standard60 <= @HDRTotalProdJl3)
			BEGIN
				IF(@TotalProdBox - @Standard60 > 0)
					BEGIN
						SET @HDRTotalProdJl3 = @TotalProdBox - @Standard60;
					END
				ELSE
					BEGIN
						SET @HDRTotalProdJl3 = 0;
					END
			END
			
		-- Jl4
		IF(@TotalProdBox - @Standard60 - @HDRTotalProdJl3 > 0)
			BEGIN
				SET @HDRTotalProdJl4 = @TotalProdBox - @Standard60 - @HDRTotalProdJl3;
			END
		ELSE
			BEGIN
				SET @HDRTotalProdJl4 = 0;
			END
		
		--- EOF PERHITUNGAN BARU
		
		DECLARE @TotalDibayarJKN FLOAT = @HDRTotalProdJKN;
		DECLARE @TotalDibayarJL1 FLOAT = @HDRTotalProdJl1;
		DECLARE @TotalDibayarJL2 FLOAT = @HDRTotalProdJl2;
		DECLARE @TotalDibayarJL3 FLOAT = @HDRTotalProdJl3;
		DECLARE @TotalDibayarJL4 FLOAT = @HDRTotalProdJl4;
		
		UPDATE TPOFeeHdrPlan
		  SET TotalProdBox = @HDRTotalProdBox,
				TotalProdStick = @HDRTotalProdStick,
				TotalProdJKN = @HDRTotalProdJKN,
				TotalProdJl1 = @HDRTotalProdJl1,
				TotalProdJl2 = @HDRTotalProdJl2,
				TotalProdJl3 = @HDRTotalProdJl3,
				TotalProdJl4 = @HDRTotalProdJl4
		WHERE TPOFeeCode = @TPOFeeCode
          AND LocationCode = @LocationCode
          AND BrandGroupCode = @BrandGroupCode
          AND KPSYear = @KPSYear
          AND KPSWeek = @KPSWeek
		  
        -- Check TPO Fee Calculation
        DECLARE @TPOFeeCalculationExist INT;
        SELECT @TPOFeeCalculationExist = COUNT(*)
        FROM TPOFeeCalculationPlan
        WHERE TPOFeeCode = @TPOFeeCode;
		
        DECLARE @TPORateJKN FLOAT;
        DECLARE @TPORateJl1 FLOAT;
        DECLARE @TPORateJl2 FLOAT;
        DECLARE @TPORateJl3 FLOAT;
        DECLARE @TPORateJl4 FLOAT;
        DECLARE @ManagementFee FLOAT;
        DECLARE @ProductivityIncentives FLOAT;

	   SELECT @StartDate = StartDate,
                       @EndDate = EndDate
                FROM MstGenWeek
                WHERE Year = @KPSYear
                  AND Week = @KPSWeek;
				  
		DECLARE @OutputProduction1  FLOAT, @OutputBiaya1       FLOAT, @Calculate1         FLOAT,
             @OutputProduction2  FLOAT, @OutputBiaya2       FLOAT, @Calculate2         FLOAT,
             @OutputProduction3  FLOAT, @OutputBiaya3       FLOAT, @Calculate3         FLOAT,
             @OutputProduction4  FLOAT, @OutputBiaya4       FLOAT, @Calculate4         FLOAT,
             @OutputProduction5  FLOAT, @OutputBiaya5       FLOAT, @Calculate5         FLOAT,
             @OutputProduction6  FLOAT, @OutputBiaya6       FLOAT, @Calculate6         FLOAT,
             @OutputProduction7  FLOAT, @OutputBiaya7       FLOAT, @Calculate7         FLOAT,
             @OutputProduction8  FLOAT, @OutputBiaya8       FLOAT, @Calculate8         FLOAT,
             @OutputProduction9  FLOAT, @OutputBiaya9       FLOAT, @Calculate9         FLOAT,
             @OutputProduction10 FLOAT, @OutputBiaya10      FLOAT, @Calculate10        FLOAT,
             @OutputProduction11 FLOAT, @OutputBiaya11      FLOAT, @Calculate11        FLOAT,
             @OutputProduction12 FLOAT, @OutputBiaya12      FLOAT, @Calculate12        FLOAT,
             @OutputProduction13 FLOAT, @OutputBiaya13      FLOAT, @Calculate13        FLOAT,
             @OutputProduction14 FLOAT, @OutputBiaya14      FLOAT, @Calculate14        FLOAT,
             @OutputProduction15 FLOAT, @OutputBiaya15      FLOAT, @Calculate15        FLOAT,
             @OutputProduction16 FLOAT, @OutputBiaya16      FLOAT, @Calculate16        FLOAT,
             @OutputProduction17 FLOAT, @OutputBiaya17      FLOAT, @Calculate17        FLOAT,
             @OutputProduction18 FLOAT, @OutputBiaya18      FLOAT, @Calculate18        FLOAT;
				  
		DECLARE @PajakJasaMaklon int,
		  @PPNBiayaProduksi int,
		  @PPNJasaMaklon int,
		  @PajakProductivityIncentives int,
		  @PPNProductivityIncentives int

		SELECT @PajakJasaMaklon = Value FROM TPOFeeSettingCalculation WHERE Name ='Pajak Jasa Maklon'
		SELECT @PPNBiayaProduksi = Value FROM TPOFeeSettingCalculation WHERE Name ='PPN Biaya Produksi'
		SELECT @PPNJasaMaklon = Value FROM TPOFeeSettingCalculation WHERE Name ='PPN Jasa Maklon'
		SELECT @PajakProductivityIncentives = Value FROM TPOFeeSettingCalculation WHERE Name ='Pajak Productivity Incentives'
		SELECT @PPNProductivityIncentives = Value FROM TPOFeeSettingCalculation WHERE Name ='PPN Productivity Incentives'

        SELECT @TPORateJKN = JKN,
               @TPORateJl1 = Jl1,
               @TPORateJl2 = Jl2,
               @TPORateJl3 = Jl3,
               @TPORateJl4 = Jl4,
               @ManagementFee = ManagementFee,
               @ProductivityIncentives = ProductivityIncentives
        FROM MstTPOFeeRate
        WHERE LocationCode = @LocationCode
          AND BrandGroupCode = @BrandGroupCode
          AND EffectiveDate <= @StartDate
		AND ExpiredDate >= @EndDate;

		IF (@ProductivityIncentives IS NULL)
		BEGIN
			SET @ProductivityIncentives = 0;
		END;
		IF (@ManagementFee IS NULL)
		BEGIN
			SET @ManagementFee = 0;
		END;
		IF (@TPORateJKN IS NULL)
		BEGIN
			SET @TPORateJKN = 0;
		END;
		IF (@TPORateJl1 IS NULL)
		BEGIN
			SET @TPORateJl1 = 0;
		END;
		IF (@TPORateJl2 IS NULL)
		BEGIN
			SET @TPORateJl2 = 0;
		END;
		IF (@TPORateJl3 IS NULL)
		BEGIN
			SET @TPORateJl3 = 0;
		END;
		IF (@TPORateJl4 IS NULL)
		BEGIN
			SET @TPORateJl4 = 0;
		END;

        DECLARE @OutputBiayaJKN FLOAT = ( @TPORateJKN );
        DECLARE @OutputBiayaJL1 FLOAT = ( @TPORateJl1 );
        DECLARE @OutputBiayaJL2 FLOAT = ( @TPORateJl2 );
        DECLARE @OutputBiayaJL3 FLOAT = ( @TPORateJl3 );
        DECLARE @OutputBiayaJL4 FLOAT = ( @TPORateJl4 );
		
		
		-- JKN #OrderFeeType 1
		-- TODO JKN @OutputProduction1
		-- SUM JKNJam di TPOFeeDaily => @SumJKNJam
		-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJKNJam * 
		-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
		--SET @OutputProduction1 = @HDRTotalProdJKN
		SET @OutputProduction1 = @HDRTotalProdJKN--( @TPOPackageValue * @StdStickPerHours * @TotalJKN * @EmpPackage ) / @StickPerBox;
		SET @OutputBiaya1 = @TPORateJKN;
		SET @Calculate1 = @OutputProduction1 * @OutputBiaya1;

		-- JL1 #OrderFeeType 2
		-- TODO JKN @OutputProduction2
		-- SUM JKNJam di TPOFeeDaily => @SumJL1Jam
		-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL1Jam * 
		-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
		--SET @OutputProduction2 = @HDRTotalProdJl1
		SET @OutputProduction2 = @HDRTotalProdJl1--( @TPOPackageValue * @StdStickPerHours * @TotalJL1 * @EmpPackage ) / @StickPerBox;
		SET @OutputBiaya2 = @TPORateJl1;
		SET @Calculate2 = @OutputProduction2 * @OutputBiaya2;

		-- JL2 #OrderFeeType 3
		-- TODO JKN @OutputProduction2
		-- SUM JKNJam di TPOFeeDaily => @SumJL2Jam
		-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL2Jam * 
		-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
		--SET @OutputProduction3 = @HDRTotalProdJl2
		SET @OutputProduction3 = @HDRTotalProdJl2--( @TPOPackageValue * @StdStickPerHours * @TotalJL2 * @EmpPackage ) / @StickPerBox;
		SET @OutputBiaya3 = @TPORateJl2;
		SET @Calculate3 = @OutputProduction3 * @OutputBiaya3;

		-- JL3 #OrderFeeType 4
		-- TODO JKN @OutputProduction3
		-- SUM JKNJam di TPOFeeDaily => @SumJL3Jam
		-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL3Jam * 
		-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
		--SET @OutputProduction4 = @HDRTotalProdJl3
		SET @OutputProduction4 = @HDRTotalProdJl3--( @TPOPackageValue * @StdStickPerHours * @TotalJL3 * @EmpPackage ) / @StickPerBox;
		SET @OutputBiaya4 = @TPORateJl3;
		SET @Calculate4 = @OutputProduction4 * @OutputBiaya4;

		-- JL4 #OrderFeeType 5
		-- TODO JKN @OutputProduction4
		-- SUM JKNJam di TPOFeeDaily => @SumJL4Jam
		-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL4Jam * 
		-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
		--SET @OutputProduction5 = @HDRTotalProdJl4
		SET @OutputProduction5 = @HDRTotalProdJl4 --( @TPOPackageValue * @StdStickPerHours * @TotalJL4 * @EmpPackage ) / @StickPerBox;
		SET @OutputBiaya5 = @TPORateJl4;
		SET @Calculate5 = @OutputProduction5 * @OutputBiaya5;

		-- Biaya Produksi #OrderFeeType 6
		-- TODO Biaya Produksi @OutputProduction6 = #OrderFeeType 1 + #OrderFeeType 2 + #OrderFeeType 3 + #OrderFeeType 4
		-- TODO Biaya Produksi @Calculate6 = #Calculate1 + #Calculate2 + #Calculate3 + #Calculate4
		SET @OutputProduction6 = @HDRTotalProdJKN + @HDRTotalProdJl1 +@HDRTotalProdJl2 +@HDRTotalProdJl3 +@HDRTotalProdJl4;
		SET @OutputBiaya6 = NULL;
		SET @Calculate6 = @Calculate1 + @Calculate2 + @Calculate3 + @Calculate4 + @Calculate5;

		-- Jasa Maklon #OrderFeeType 7
		-- TODO Jasa Maklon @OutputProduction7 = @OutputProduction6
		SET @OutputProduction7 = @HDRTotalProdJKN + @HDRTotalProdJl1 + @HDRTotalProdJl2 + @HDRTotalProdJl3 + @HDRTotalProdJl4;
		SET @OutputBiaya7 = @ManagementFee;
		SET @Calculate7 = @OutputProduction7 * @OutputBiaya7;

		-- Productivity Incentives #OrderFeeType 16
		-- TODO Productivity Incentives @OutputProduction16 = @OutputProduction6
		SET @OutputProduction16 = @HDRTotalProdJKN + @HDRTotalProdJl1 + @HDRTotalProdJl2 + @HDRTotalProdJl3 + @HDRTotalProdJl4;
		SET @OutputBiaya16 = @ProductivityIncentives;
		SET @Calculate16 = @OutputProduction16 * @OutputBiaya16;

		-- Total Biaya Produksi & Jasa Maklon Calculation #OrderFeeType 8
		-- TODO Biaya Produksi & Jasa Maklon @Calculate8 = @Calculate6 + @Calculate7 - @Calculate16
		SET @OutputProduction8 = NULL;
		SET @OutputBiaya8 = NULL;
		SET @Calculate8 = @Calculate6 + @Calculate7 + @Calculate16;

		-- Pajak Jasa Maklon Sebesar 2% #OrderFeeType 9
		-- TODO Create General Setting untuk variabel persentase pajak
		SET @OutputProduction9 = NULL;
		SET @OutputBiaya9 = NULL;
		SET @Calculate9 = FLOOR(( @Calculate7 * @PajakJasaMaklon ) / 100);

		-- Pajak Productivity Incentives sebesar 2 % #OrderFeeType 9
		-- TODO UDAH BENER
		SET @OutputProduction17 = NULL;
		SET @OutputBiaya17 = NULL;
		SET @Calculate17 = FLOOR(( @Calculate16 * @PajakProductivityIncentives ) / 100);

		-- Total Biaya Yang Harus Dibayarkan Ke MPS #OrderFeeType 10
		-- TODO Biaya yang harus dibayarkan ke MPS @Calculate10 = @Calculate8 - @Calculate9
		SET @OutputProduction10 = NULL;
		SET @OutputBiaya10 = NULL;
		SET @Calculate10 = @Calculate8 - @Calculate9;

		-- Pembayaran #OrderFeeType 11
		-- TODO UDAH BENER
		SET @OutputProduction11 = NULL;
		SET @OutputBiaya11 = NULL;
		SET @Calculate11 = @Calculate10;

		-- Sisa yang harus dibayar #OrderFeeType 12
		-- TODO UDAH BENER
		SET @OutputProduction12 = NULL;
		SET @OutputBiaya12 = NULL;
		SET @Calculate12 = 0;

		-- PPN Biaya Produksi 10% #OrderFeeType 13
		-- TODO UDAH BENER
		SET @OutputProduction13 = NULL;
		SET @OutputBiaya13 = NULL;
		--SET @Calculate13 = ROUND((@Calculate6 * @PPNBiayaProduksi ) / 100,0);
		SET @Calculate13 = FLOOR((@Calculate6 * @PPNBiayaProduksi ) / 100);


		-- PPN Jasa Maklon 10% #OrderFeeType 14
		-- TODO UDAH BENER
		SET @OutputProduction14 = NULL;
		SET @OutputBiaya14 = NULL;
		--SET @Calculate14 = ( @Calculate7 * @PPNJasaMaklon ) / 100;
		SET @Calculate14 = FLOOR(( @Calculate7 * @PPNJasaMaklon ) / 100);

		-- PPN Productivity Incentives 10 % #OrderFeeType 18
		-- TODO
		SET @OutputProduction18 = NULL;
		SET @OutputBiaya18 = NULL;
		SET @Calculate18 = FLOOR(( @Calculate16 * @PPNProductivityIncentives ) / 100);

		-- Total Bayar #OrderFeeType 15
		-- TODO Total Bayar Calculation = @Calculate11 + @Calculate13 + @Calculate14
		SET @OutputProduction15 = NULL;
		SET @OutputBiaya15 = NULL;
		SET @Calculate15 = @Calculate10 + @Calculate13 + @Calculate14 + @Calculate16 + @Calculate17 + @Calculate18;

        IF @TPOFeeCalculationExist <= 0
			--NEW METHOD
				BEGIN
                     INSERT INTO dbo.TPOFeeCalculationPlan
                            ( TPOFeeCode,
                              ProductionFeeType,
                              KPSYear,
                              KPSWeek,
                              OrderFeeType,
                              OutputProduction,
                              OutputBiaya,
                              Calculate
                            )
                     VALUES( @TPOFeeCode, 'JKN', @KPSYear, @KPSWeek, 1, @OutputProduction1, @OutputBiaya1, @Calculate1 ),
                           ( @TPOFeeCode, 'JL1', @KPSYear, @KPSWeek, 2, @OutputProduction2, @OutputBiaya2, @Calculate2 ),
                           ( @TPOFeeCode, 'JL2', @KPSYear, @KPSWeek, 3, @OutputProduction3, @OutputBiaya3, @Calculate3 ),
                           ( @TPOFeeCode, 'JL3', @KPSYear, @KPSWeek, 4, @OutputProduction4, @OutputBiaya4, @Calculate4 ),
                           ( @TPOFeeCode, 'JL4', @KPSYear, @KPSWeek, 5, @OutputProduction5, @OutputBiaya5, @Calculate5 ),
                           ( @TPOFeeCode, 'Biaya Produksi', @KPSYear, @KPSWeek, 6, @OutputProduction6, @OutputBiaya6, @Calculate6 ),
                           ( @TPOFeeCode, 'Jasa Manajemen', @KPSYear, @KPSWeek, 7, @OutputProduction7, @OutputBiaya7, @Calculate7 ),
                           ( @TPOFeeCode, 'Productivity Incentives', @KPSYear, @KPSWeek, 16, @OutputProduction16, @OutputBiaya16, @Calculate16 ),
                           ( @TPOFeeCode, 'Total Biaya Produksi & Jasa Manajemen', @KPSYear, @KPSWeek, 8, @OutputProduction8, @OutputBiaya8, @Calculate8 ),
                           ( @TPOFeeCode, 'Pajak Jasa Manajemen Sebesar 2%', @KPSYear, @KPSWeek, 9, @OutputProduction9, @OutputBiaya9, @Calculate9 ),
                           ( @TPOFeeCode, 'Pajak Productivity Incentives sebesar 2 %', @KPSYear, @KPSWeek, 17, @OutputProduction17, @OutputBiaya17, @Calculate17 ),
                           ( @TPOFeeCode, 'Total Biaya Yang Harus Dibayarkan Ke MPS', @KPSYear, @KPSWeek, 10, @OutputProduction10, @OutputBiaya10, @Calculate10 ),
                           ( @TPOFeeCode, 'Pembayaran', @KPSYear, @KPSWeek, 11, @OutputProduction11, @OutputBiaya11, @Calculate11 ),
                           ( @TPOFeeCode, 'Sisa yang harus dibayar', @KPSYear, @KPSWeek, 12, @OutputProduction12, @OutputBiaya12, @Calculate12 ),
                           ( @TPOFeeCode, 'PPN Biaya Produksi 10%', @KPSYear, @KPSWeek, 13, @OutputProduction13, @OutputBiaya13, @Calculate13 ),
                           ( @TPOFeeCode, 'PPN Jasa Manajemen 10%', @KPSYear, @KPSWeek, 14, @OutputProduction14, @OutputBiaya14, @Calculate14 ),
                           ( @TPOFeeCode, 'PPN Productivity Incentives 10 %', @KPSYear, @KPSWeek, 18, @OutputProduction18, @OutputBiaya18, @Calculate18 ),
                           ( @TPOFeeCode, 'Total Bayar', @KPSYear, @KPSWeek, 15, @OutputProduction15, @OutputBiaya15, @Calculate15 );
                 END;
             ELSE
                 BEGIN
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction1, OutputBiaya = @OutputBiaya1, Calculate = @Calculate1 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 1;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction2, OutputBiaya = @OutputBiaya2, Calculate = @Calculate2 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 2;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction3, OutputBiaya = @OutputBiaya3, Calculate = @Calculate3 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 3;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction4, OutputBiaya = @OutputBiaya4, Calculate = @Calculate4 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 4; 
					 UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction5, OutputBiaya = @OutputBiaya5, Calculate = @Calculate5 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 5;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction6, OutputBiaya = @OutputBiaya6, Calculate = @Calculate6 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 6;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction7, OutputBiaya = @OutputBiaya7, Calculate = @Calculate7 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 7;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction16, OutputBiaya = @OutputBiaya16, Calculate = @Calculate16 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 16;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction8, OutputBiaya = @OutputBiaya8, Calculate = @Calculate8 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 8;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction9, OutputBiaya = @OutputBiaya9, Calculate = @Calculate9 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 9;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction17, OutputBiaya = @OutputBiaya17, Calculate = @Calculate17 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 17;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction10, OutputBiaya = @OutputBiaya10, Calculate = @Calculate10 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 10;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction11, OutputBiaya = @OutputBiaya11, Calculate = @Calculate11 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 11;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction12, OutputBiaya = @OutputBiaya12, Calculate = @Calculate12 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 12;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction13, OutputBiaya = @OutputBiaya13, Calculate = @Calculate13 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 13;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction14, OutputBiaya = @OutputBiaya14, Calculate = @Calculate14 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 14;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction18, OutputBiaya = @OutputBiaya18, Calculate = @Calculate18 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 18;
                     UPDATE dbo.TPOFeeCalculationPlan SET OutputProduction = @OutputProduction15, OutputBiaya = @OutputBiaya15, Calculate = @Calculate15 WHERE TPOFeeCode = @TPOFeeCode AND OrderFeeType = 15;
                 END;
			--OLD METHOD
            -- BEGIN
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'JKN', @KPSYear, @KPSWeek, 1, @HDRTotalProdJKN, @OutputBiaya1, @HDRTotalProdJKN * @OutputBiaya1 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'JL1', @KPSYear, @KPSWeek, 2, @HDRTotalProdJl1, @OutputBiaya2, @HDRTotalProdJl1 * @OutputBiaya2 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'JL2', @KPSYear, @KPSWeek, 3, @HDRTotalProdJl2, @OutputBiaya3, @HDRTotalProdJl2 * @OutputBiaya3 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'JL3', @KPSYear, @KPSWeek, 4, @HDRTotalProdJl3, @OutputBiaya4, @HDRTotalProdJl3 * @OutputBiaya4 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'JL4', @KPSYear, @KPSWeek, 5, @HDRTotalProdJl4, @OutputBiaya5, @HDRTotalProdJl4 * @OutputBiaya5 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Biaya Produksi', @KPSYear, @KPSWeek, 6, @HDRTotalProdBox, NULL, @HDRTotalProdBox * @OutputBiaya1 );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Jasa Manajemen', @KPSYear, @KPSWeek, 7, @HDRTotalProdBox, @ManagementFee, @HDRTotalProdBox * @ManagementFee );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Total Biaya Produksi & Jasa Manajemen', @KPSYear, @KPSWeek, 8, NULL, NULL, (( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Pajak Jasa Manajemen Sebesar 2%', @KPSYear, @KPSWeek, 9, NULL, NULL, FLOOR((( @HDRTotalProdBox * @ManagementFee ) * @PajakJasaMaklon ) / 100 ));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Total Biaya Yang Harus Dibayarkan Ke MPS', @KPSYear, @KPSWeek, 10, NULL, NULL, ((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 )));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Pembayaran', @KPSYear, @KPSWeek, 11, NULL, NULL, ((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 )));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Sisa yang harus dibayar', @KPSYear, @KPSWeek, 12, NULL, NULL, 0 --???
                      -- );
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'PPN Biaya Produksi 10%', @KPSYear, @KPSWeek, 13, NULL, NULL, ((( @HDRTotalProdBox * @ManagementFee ) * 10 ) / 100 ));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'PPN Jasa Manajemen 10%', @KPSYear, @KPSWeek, 14, NULL, NULL, ((( @HDRTotalProdBox * @OutputBiaya1 ) * 10 ) / 100 ));
                -- INSERT INTO TPOFeeCalculationPlan
                -- VALUES( @TPOFeeCode, 'Total Bayar', @KPSYear, @KPSWeek, 15, NULL, NULL, (((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 )) + ((( @HDRTotalProdBox * @ManagementFee ) * 10 ) / 100 ) + ((( @HDRTotalProdBox * @OutputBiaya1 ) * 10 ) / 100 )));
            -- END;
        -- ELSE
            -- BEGIN
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdJKN,
                      -- OutputBiaya = @OutputBiaya1,
                      -- Calculate = @HDRTotalProdJKN * @OutputBiaya1
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 1
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdJl1,
                      -- OutputBiaya = @OutputBiaya2,
                      -- Calculate = @HDRTotalProdJl1 * @OutputBiaya2
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 2
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdJl2,
                      -- OutputBiaya = @OutputBiaya3,
                      -- Calculate = @HDRTotalProdJl2 * @OutputBiaya3
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 3
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdJl3,
                      -- OutputBiaya = @OutputBiaya4,
                      -- Calculate = @HDRTotalProdJl3 * @OutputBiaya4
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 4
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdJl4,
                      -- OutputBiaya = @OutputBiaya5,
                      -- Calculate = @HDRTotalProdJl4 * @OutputBiaya5
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 5
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdBox,
                      -- Calculate = @HDRTotalProdBox * @OutputBiaya1
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 6
                -- UPDATE TPOFeeCalculationPlan
                  -- SET OutputProduction = @HDRTotalProdBox,
                      -- OutputBiaya = @ManagementFee,
                      -- Calculate = @HDRTotalProdBox * @ManagementFee
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 7
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = (( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives ))
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 8
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 )
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 9
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = ((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 ))
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 10
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = ((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 ))
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 11
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = 0 --???
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 12
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = ((( @HDRTotalProdBox * @ManagementFee ) * 10 ) / 100 )
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 13
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = ((( @HDRTotalProdBox * @OutputBiaya1 ) * 10 ) / 100 )
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 14
                -- UPDATE TPOFeeCalculationPlan
                  -- SET Calculate = (((( @HDRTotalProdBox * @OutputBiaya1 ) + ( @HDRTotalProdBox * @ManagementFee ) - ( @HDRTotalProdBox * @ProductivityIncentives )) - ((( @HDRTotalProdBox * @ManagementFee ) * 2 ) / 100 )) + ((( @HDRTotalProdBox * @ManagementFee ) * 10 ) / 100 ) + ((( @HDRTotalProdBox * @OutputBiaya1 ) * 10 ) / 100 ))
                -- WHERE TPOFeeCode = @TPOFeeCode
                  -- AND KPSYear = @KPSYear
                  -- AND KPSWeek = @KPSWeek
                  -- AND OrderFeeType = 15
            -- END;
			
		-- KALKULASI RUPIAH SETELAH SEMUA KALKULASI DILAKUKAN --
		SELECT @StartDate = StartDate,
			   @EndDate = EndDate
		FROM MstGenWeek
		WHERE Year = @KPSYear
		  AND Week = @KPSWeek;
		
		DECLARE @totalRupiahJKN FLOAT = @HDRTotalProdJKN * @OutputBiayaJKN;
		DECLARE @totalRupiahJL1 FLOAT = @HDRTotalProdJl1 * @OutputBiayaJL1;
		DECLARE @totalRupiahJL2 FLOAT = @HDRTotalProdJl2 * @OutputBiayaJL2;
		DECLARE @totalRupiahJL3 FLOAT = @HDRTotalProdJl3 * @OutputBiayaJL3;
		DECLARE @totalRupiahJL4 FLOAT = @HDRTotalProdJl4 * @OutputBiayaJL4;
		
		WHILE @StartDate <= @EndDate
			BEGIN
				DECLARE @JKNRp FLOAT = 0;
				DECLARE @JL1Rp FLOAT = 0;
				DECLARE @JL2Rp FLOAT = 0;
				DECLARE @JL3Rp FLOAT = 0;
				DECLARE @JL4Rp FLOAT = 0;
				
				DECLARE @convertJKN FLOAT = 0;
				DECLARE @convertJL1 FLOAT = 0;
				DECLARE @convertJL2 FLOAT = 0;
				DECLARE @convertJL3 FLOAT = 0;
				DECLARE @convertJL4 FLOAT = 0;
				
				DECLARE @DailyJKN FLOAT;
				DECLARE @DailyJL1 FLOAT;
				DECLARE @DailyJL2 FLOAT;
				DECLARE @DailyJL3 FLOAT;
				DECLARE @DailyJL4 FLOAT;
				
				SELECT @DailyJKN = JKN,
					@DailyJL1 = JL1,
					@DailyJL2 = JL2,
					@DailyJL3 = JL3,
					@DailyJL4 = JL4
				FROM @DailyJK
				WHERE FeeDate = @StartDate
				
				-- JKN --
				IF(@TotalProdJKN > 0 AND @TotalDibayarJKN > 0 AND @totalRupiahJKN > 0)
					BEGIN
						SET @convertJKN = @DailyJKN / @TotalProdJKN * @TotalDibayarJKN;
						SET @JKNRp = @convertJKN / @TotalDibayarJKN * @totalRupiahJKN;
					END
					
				-- JL1 --
				IF(@TotalProdJl1 > 0 AND @TotalDibayarJL1 > 0 AND @totalRupiahJL1 > 0)
					BEGIN
						SET @convertJL1 = @DailyJL1 / @TotalProdJl1 * @TotalDibayarJL1;
						SET @JL1Rp = @convertJL1 /@TotalDibayarJL1 * @totalRupiahJL1;
					END
				
				-- JL2 --
				IF(@TotalProdJl2 > 0 AND @TotalDibayarJL2 > 0 AND @totalRupiahJL2 > 0)
					BEGIN
						SET @convertJL2 = @DailyJL2 / @TotalProdJl2 * @TotalDibayarJL2;
						SET @JL2Rp = @convertJL2 /@TotalDibayarJL2 * @totalRupiahJL2;
					END
				
				-- JL3 --
				IF(@TotalProdJl3 > 0 AND @TotalDibayarJL3 > 0 AND @totalRupiahJL3 > 0)
					BEGIN
						SET @convertJL3 = @DailyJL3 / @TotalProdJl3 * @TotalDibayarJL3;
						SET @JL3Rp = @convertJL3 /@TotalDibayarJL3 * @totalRupiahJL3;
					END
				
				-- JL4 --
				IF(@TotalProdJl4 > 0 AND @TotalDibayarJL4 > 0 AND @totalRupiahJL4 > 0)
					BEGIN
						SET @convertJL4 = @DailyJL4 / @TotalProdJl4 * @TotalDibayarJL4;
						SET @JL4Rp = @convertJL4 /@TotalDibayarJL4 * @totalRupiahJL4;
					END
				
				UPDATE TPOFeeProductionDailyPlan
				SET JKNRp = @JKNRp,
					JL1Rp = @JL1Rp,
					JL2Rp = @JL2Rp,
					JL3Rp = @JL3Rp,
					JL4Rp = @JL4Rp,
					UpdatedDate = GETDATE()
				WHERE TPOFeeCode = @TPOFeeCode
				  AND FeeDate = @StartDate;
				  
				SET @StartDate = DATEADD(DAY, 1, @StartDate);
			END;
    END;

