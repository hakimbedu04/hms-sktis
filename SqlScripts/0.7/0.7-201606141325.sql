-- =============================================
-- Description: TransactionLog check latest entry
-- Ticket: http://tp.voxteneo.co.id/entity/3447
-- Author: Dwi Yudha
-- Version: 2.5
-- =============================================

ALTER PROCEDURE [dbo].[TPOProductionEntryVerificationGenerateReport]
	@LocationCode varchar(8),
	@BrandCode varchar(11),
	@KPSYear int,
	@KPSWeek int,
	@ProductionDate date,
	@UserName VARCHAR(32)
AS
BEGIN

SET NOCOUNT ON;
    -- GLOBAL VARIABLE
    DECLARE @TPOFeeCode VARCHAR(64), @StickPerBox INT;

    -- GLOBAL DATA
    -- Master General Brand Group	
    DECLARE @BrandGroupCode VARCHAR(20);

    SELECT @BrandGroupCode = bg.BrandGroupCode, @StickPerBox = bg.StickPerBox
    FROM MstGenBrandGroup bg
         INNER JOIN MstGenBrand b ON b.BrandGroupCode = bg.BrandGroupCode
    WHERE b.BrandCode = @BrandCode;

SET @TPOFeeCode = 'FEE/' + @LocationCode + '/' + @BrandGroupCode + '/' + CAST(@KPSYear AS VARCHAR) + '/' + CAST(@KPSWeek AS VARCHAR);

    --START INSERT DATA TpoFeeHdr ################################################
    DECLARE @TPOFeeHdrExist INT;

    SELECT @TPOFeeHdrExist = COUNT(*) FROM TPOFeeHdr
    WHERE KPSYear = @KPSYear
	  AND KPSWeek = @KPSWeek
	  AND BrandGroupCode = @BrandGroupCode
	  AND LocationCode = @LocationCode;

	DECLARE @TPOPackageValue REAL;
	-- Set @TPOPackageValue with Package from MstTPOPackage
	-- IF datas > 1 => higest
	SELECT TOP 1 @TPOPackageValue = Package
	FROM MstTPOPackage
	WHERE dbo.MstTPOPackage.LocationCode = @LocationCode
	  AND dbo.MstTPOPackage.BrandGroupCode = @BrandGroupCode
	  AND @ProductionDate >= EffectiveDate
	  AND @ProductionDate <= ExpiredDate
	ORDER BY dbo.MstTPOPackage.Package DESC;
	
	IF (@TPOPackageValue IS NULL)
	BEGIN
		SET @TPOPackageValue = 0;
	END;
	--adding by bagus ( for round package value )
	set @TPOPackageValue = Round(@TPOPackageValue,2);
	-- Master TPO Info
	DECLARE @PengirimanL1 VARCHAR(64), @PengirimanL2 VARCHAR(64), @PengirimanL3 VARCHAR(64), @PengirimanL4 VARCHAR(64);

	SELECT @PengirimanL1 = VendorName, @PengirimanL2 = BankAccountNumber, @PengirimanL3 = BankAccountName, @PengirimanL4 = BankBranch
	FROM MstTPOInfo WHERE LocationCode = @LocationCode;	

	-- Master TPO Package
	DECLARE @StartDate       DATETIME,
			@EndDate         DATETIME;

	-- Get StartData and EndDate from MstGenWeek based Year and Week
	SELECT @StartDate = mgw.StartDate, @EndDate = EndDate
	FROM dbo.MstGenWeek mgw
	WHERE mgw.[Year] = @KPSYear
	  AND mgw.Week = @KPSWeek;
	  
    IF @TPOFeeHdrExist <= 0
        BEGIN        
            INSERT INTO TPOFeeHdr
                   ( KPSYear,
                     KPSWeek,
                     BrandGroupCode,
                     LocationCode,
                     CurrentApproval,
                     CreatedDate,
                     CreatedBy,
                     UpdatedDate,
                     UpdatedBy,
                     TPOFeeCode,
                     Status,
                     PengirimanL1,
                     PengirimanL2,
                     PengirimanL3,
                     PengirimanL4,
                     StickPerBox,
                     TPOPackageValue
                   )
            SELECT @KPSYear,
                   @KPSWeek,
                   @BrandGroupCode,
                   @LocationCode,
                   'PMI/User',
                   GETDATE(),
                   'PMI/User',
                   GETDATE(),
                   'PMI/User',
                   @TPOFeeCode,
                   'OPEN',
                   @PengirimanL1,
                   @PengirimanL2,
                   @PengirimanL3,
                   @PengirimanL4,
                   @StickPerBox,
                   @TPOPackageValue;
        END;
	ELSE
		BEGIN
			-- Update TPO Fee Header
			UPDATE TPOFeeHdr
			SET BrandGroupCode = @BrandGroupCode,
				UpdatedDate = GETDATE(),
				TPOFeeCode = @TPOFeeCode,
				Status = 'OPEN',
                PengirimanL1 = @PengirimanL1,
                PengirimanL2 = @PengirimanL2,
                PengirimanL3 = @PengirimanL3,
                PengirimanL4 = @PengirimanL4,
				StickPerBox = @StickPerBox,
				TPOPackageValue = @TPOPackageValue
			WHERE KPSYear = @KPSYear
			  AND KPSWeek = @KPSWeek
			  AND BrandGroupCode = @BrandGroupCode
			  AND LocationCode = @LocationCode;
		END;
    --END INSERT DATA TpoFeeHdr ################################################

    --START INSERT DATA TPOFeeProductionDaily ################################################
	DECLARE @TempTable TABLE
	(
	  BrandCode VARCHAR(11)
	)
	
	DECLARE @TempBrandCode VARCHAR(11);
	DECLARE @LogExist INT;
	DECLARE @DayOfWeek INT = 0;
	DECLARE @StatusDraft INT = 1;
	
	WHILE @StartDate <= @EndDate
		BEGIN
		
			-- LOOPING EACH BRAND IN BRAND GROUP ON EACH DAY --
			SET @DayOfWeek = @DayOfWeek + 1;
			
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
			DECLARE @JL1Sum FLOAT = 0;
			DECLARE @JL2Sum FLOAT = 0;
			DECLARE @JL3Sum FLOAT = 0;
			DECLARE @JL4Sum FLOAT = 0;
			
			WHILE((SELECT COUNT(*) FROM @TempTable) > 0)
			BEGIN
				SET @TempBrandCode = (Select Top 1 BrandCode From @TempTable);
				
				-- CHECK TRANSLOG for stamping submit --
				SELECT TOP 1 @LogExist = IDFlow
				FROM UtilTransactionLogs
				WHERE TransactionCode = 'EBL/' + @LocationCode + '/4/' + @TempBrandCode + '/' + CAST(@KPSYear AS VARCHAR) + '/' + CAST(@KPSWeek AS VARCHAR) + '/' + CAST(@DayOfWeek AS VARCHAR)
				ORDER BY TransactionDate DESC

				DECLARE @TPOFeeProductionDailyExist INT;

				SELECT @TPOFeeProductionDailyExist = COUNT(*)
				FROM TPOFeeProductionDaily
				WHERE TPOFeeCode = @TPOFeeCode
				  AND FeeDate = @StartDate
				  AND KPSYear = @KPSYear
				  AND KPSWeek = @KPSWeek;
			
				-- IF ( @LogExist > 0 OR ( @TempBrandCode = @BrandCode AND @ProductionDate = @StartDate ))
				--IDFlow = 38
				IF ( @LogExist = 38 )
				BEGIN
					-- REAL CALCULATION LOOP AND SUMARIZING --
					
					DECLARE @Outputbox FLOAT;
					DECLARE @ProcessGroupVerification VARCHAR(20);

					-- TODO @Outputbox
					-- Get data dari TotalActualValue TPOProductionEntryVerifivation, yang Proces Ordernya paling besar
					-- Jika ada dua brandCode di TPOProductionEntryVerification, maka nilainya harus dijumlahkan
					/* SELECT TOP 1 @Outputbox = asd.TotalActualValue, @ProcessGroupVerification = asd.ProcessGroup
					FROM( SELECT SUM(etev.TotalActualValue) TotalActualValue, mgp.ProcessOrder, etev.ProcessGroup
						  FROM dbo.ExeTPOProductionEntryVerificationView etev
						  JOIN dbo.MstGenProcess mgp ON mgp.ProcessGroup = etev.ProcessGroup
						  WHERE etev.LocationCode = @LocationCode
							AND etev.BrandCode IN ( SELECT dbo.MstGenBrand.BrandCode FROM dbo.MstGenBrand WHERE dbo.MstGenBrand.BrandGroupCode = @BrandGroupCode )
							AND etev.KPSYear = @KPSYear
							AND etev.KPSWeek = @KPSWeek
							AND etev.ProductionDate = @StartDate
						  GROUP BY mgp.ProcessOrder, etev.ProcessGroup ) AS asd
					ORDER BY asd.ProcessOrder DESC; */
					
					SELECT TOP 1 @Outputbox = etev.KeluarBersih, @ProcessGroupVerification = etev.ProcessGroup
					FROM ExeReportByProcess etev
					WHERE etev.LocationCode = @LocationCode
						AND etev.BrandCode = @TempBrandCode
						AND etev.KPSYear = @KPSYear
						AND etev.KPSWeek = @KPSWeek
						AND etev.ProductionDate = @StartDate
						AND etev.UOMOrder = 12

					IF (@Outputbox IS NULL)
					BEGIN
						SET @Outputbox = 0;
					END;

					DECLARE @UOMEblek INT;

					SELECT @UOMEblek = UOMEblek
						FROM ProcessSettingsAndLocationView ps
						WHERE ps.LocationCode = @LocationCode
							AND ps.BrandGroupCode = @BrandGroupCode
							AND ps.ProcessGroup = @ProcessGroupVerification

					IF (@UOMEblek IS NULL)
					BEGIN
						SET @UOMEblek = 10;
					END;
					
					DECLARE @ResultUOM FLOAT = @StickPerBox / @UOMEblek;
					SET @ResultUOM = 1;
					SET @OutputBox = @OutputBox / @ResultUOM;

					DECLARE @OutputSticks REAL;
					-- TODO @OutputSticks
					-- @OutputSticks * @StickPerBox       
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
					
					--New JKN, JL1, JL2, JL3, JL4 Condition
					-- TODO JKN, JL1, JL2, JL3, JL4 => DONE
					-- Get data dari MstGenHoloday,
					-- Klo misalkan hari tersebut hari libur (diliat dari MstGenHoliday), ambil yang Holiday, sebaliknya Non-Holiday dari mstGenStandarHours
					-- Check data mstGenStandarHours 1-7 (1 Senin, 2 Selasa, ...)
					DECLARE @IsHoliday INT;
					
					SELECT @IsHoliday = COUNT(*) FROM dbo.MstGenHoliday mgh WHERE mgh.LocationCode = @LocationCode AND mgh.StatusActive = '1' AND mgh.HolidayDate = @StartDate
					IF @IsHoliday <= 0
						BEGIN
							SELECT @JKNJamStd = mgsh.JknHour,
								   @Jl1Jam = mgsh.Jl1Hour,
								   @Jl2Jam = mgsh.Jl2Hour,
								   @Jl3Jam = mgsh.Jl3Hour,
								   @Jl4Jam = mgsh.Jl4Hour
							FROM dbo.MstGenStandardHours mgsh
							WHERE mgsh.Day = ( CASE DATEPART(DW, @StartDate) - 1 WHEN 0 THEN 7 ELSE DATEPART(DW, @StartDate) - 1 END )
							  AND mgsh.DayType = 'Non-Holiday';
						END;
					ELSE
						BEGIN
							SELECT @JKNJamStd = mgsh.JknHour,
								   @Jl1Jam = mgsh.Jl1Hour,
								   @Jl2Jam = mgsh.Jl2Hour,
								   @Jl3Jam = mgsh.Jl3Hour,
								   @Jl4Jam = mgsh.Jl4Hour
							FROM dbo.MstGenStandardHours mgsh
							WHERE mgsh.Day = ( CASE DATEPART(DW, @StartDate) - 1 WHEN 0 THEN 7 ELSE DATEPART(DW, @StartDate) - 1 END )
							  AND mgsh.DayType = 'Holiday';
						END;
					
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

					-- SET @EmpPackage
					SELECT @EmpPackage = dbo.MstGenBrandGroup.EmpPackage
					FROM MstGenBrandGroup
					WHERE dbo.MstGenBrandGroup.BrandGroupCode = @BrandGroupCode;

					--SELECT @StdStickPerHours = mgps.StdStickPerHour FROM dbo.MstGenProcessSettings mgps WHERE mgps.BrandGroupCode = @BrandGroupCode AND mgps.ProcessGroup='ROLLING'
					-- TODO JKN
					-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
					-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JKNJam * 
					-- (Get data EmpPackage from MstGenBrandGroup where Filter) * @StickPerBox
					--SET @JKN = ROUND((( @TPOPackageValue * @StdStickPerHours * @JKNJamStd * @EmpPackage ) / @StickPerBox ), 2);
					SET @JKN =((@TPOPackageValue * @StdStickPerHours * @JKNJamStd * @EmpPackage ) / @StickPerBox );		
					-- TODO JL1
					-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
					-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL1Jam * 
					-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
					--SET @JL1 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl1Jam * @EmpPackage ) / @StickPerBox ), 2);
					SET @JL1 = (( @TPOPackageValue * @StdStickPerHours * @Jl1Jam * @EmpPackage ) / @StickPerBox );
					-- TODO JL2
					-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
					-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL2Jam * 
					-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
					--SET @JL2 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl2Jam * @EmpPackage ) / @StickPerBox ), 2);
					SET @JL2 =(( @TPOPackageValue * @StdStickPerHours * @Jl2Jam * @EmpPackage ) / @StickPerBox );

					-- TODO JL3
					-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
					-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL3Jam * 
					-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
					--SET @JL3 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl3Jam * @EmpPackage ) / @StickPerBox ), 2);
					SET @JL3 = (( @TPOPackageValue * @StdStickPerHours * @Jl3Jam * @EmpPackage ) / @StickPerBox );


					-- TODO JL4
					-- package(1)*stdstick/hour(3)*WH(5)*jmlhorang (4)/stick/box ( 6 )
					-- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * JL4Jam * 
					-- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
					--SET @JL4 = ROUND((( @TPOPackageValue * @StdStickPerHours * @Jl4Jam * @EmpPackage ) / @StickPerBox ), 2);
					SET @JL4 = (( @TPOPackageValue * @StdStickPerHours * @Jl4Jam * @EmpPackage ) / @StickPerBox );			
					-- Keterangan :
					-- @Outputbox atau OutputBox harus sama dengan @JKN + JL1 + JL2 + + JL3 + + JL4       
					-- Jika @Outputbox > @JKN
					--	 Maka : Nilai JKN = @JKN
					--		   Untuk Nilai JL1 = @Outputbox - @JKN
					--		   Dan jika Hasil @Outputbox - @JKN > @JL1
					--		   Maka Nilai JL1 = @JL1, dst...
					--SELECT @JKN, @JL1, @JL2, @JL3, @JL4
					
					DECLARE @Sisa FLOAT;
					SET @Sisa = ROUND((@Outputbox - @JKN),2);	  
						
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
									SET @Sisa = ROUND((@Sisa - @JL1), 2);
									
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
									SET @Sisa = ROUND((@Sisa - @JL3), 2);
									
									SET @JL4 = @Sisa;
								END;
							
							SET @JKN = 0;
						END;
					
					
					SET @JKNJamSum = @JKNJamSum + @JKN;
					SET @JL1Sum = @JL1Sum + @JL1;
					SET @JL2Sum = @JL2Sum + @JL2;
					SET @JL3Sum = @JL3Sum + @JL3;
					SET @JL4Sum = @JL4Sum + @JL4;
					
					SET @JKNJamStdSum = @JKNJamStdSum + @JKNJamStd;
					SET @Jl1JamSum = @Jl1JamSum + @Jl1Jam;
					SET @Jl2JamSum = @Jl2JamSum + @Jl2Jam;
					SET @Jl3JamSum = @Jl3JamSum + @Jl3Jam;
					SET @Jl4JamSum = @Jl4JamSum + @Jl4Jam;
					
					SET @OutputboxSum = @OutputboxSum + @Outputbox;
					SET @OutputSticksSum = @OutputSticksSum + @OutputSticks;
					-- END OF REAL CALCULATION LOOP --
				END;

				DELETE @TempTable Where BrandCode = @TempBrandCode;
			END
			
			SET @JKN = @JKNJamSum;
			SET @JL1 = @JL1Sum;
			SET @JL2 = @JL2Sum;
			SET @JL3 = @JL3Sum;
			SET @JL4 = @JL4Sum;
			
			SET @JKNJamStd = @JKNJamStdSum;
			SET @Jl1Jam = @Jl1JamSum;
			SET @Jl2Jam = @Jl2JamSum;
			SET @Jl3Jam = @Jl3JamSum;
			SET @Jl4Jam = @Jl4JamSum;
			
			SET @OutputSticks = @OutputSticksSum;
			SET @Outputbox = @OutputboxSum;
			
			IF @TPOFeeProductionDailyExist <= 0
				BEGIN
					INSERT INTO TPOFeeProductionDaily
						SELECT @TPOFeeCode,
							   @StartDate,
							   @KPSYear,
							   @KPSWeek,
							   @OutputSticks,
							   @Outputbox,
							   @JKN,
							   @JL1,
							   @JL2,
							   @JL3,
							   @JL4,
							   GETDATE(),
							   'PMI/User',
							   GETDATE(),
							   'PMI/User',
							   @JKNJamStd,
							   @Jl1Jam,
							   @Jl2Jam,
							   @Jl3Jam,
							   @Jl4Jam,
							   '',
							   '',
							   '',
							   '',
							   '',
							   '',
							   '',
							   '',
							   '',
							   '';
				END;
			ELSE
				BEGIN
					-- UPDATE TPOFeeProductionDaily
					UPDATE TPOFeeProductionDaily
						SET OuputSticks = @OutputSticks,
							OutputBox = @Outputbox,
							JKN = @JKN,
							JL1 = @JL1,
							Jl2 = @JL2,
							Jl3 = @JL3,
							Jl4 = @JL4,
							JKNJam = @JKNJamStd,
							JL1Jam = @Jl1Jam,
							JL2Jam = @Jl2Jam,
							JL3Jam = @Jl3Jam,
							JL4Jam = @Jl4Jam,
							UpdatedDate = GETDATE()
						WHERE TPOFeeCode = @TPOFeeCode
						  AND FeeDate = @StartDate
						  AND KPSYear = @KPSYear
						  AND KPSWeek = @KPSWeek;
				END;
				
			-- PENGECEKAN STATUS KE PLAN --
			IF @StatusDraft = 1
			BEGIN
				DECLARE @OutputBoxPlan FLOAT;

				SELECT @OutputBoxPlan = OutputBox
				FROM TPOFeeProductionDailyPlan
				WHERE TPOFeeCode = @TPOFeeCode
				  AND FeeDate = @StartDate
				  AND KPSYear = @KPSYear
				  AND KPSWeek = @KPSWeek;
				  
				IF (@OutputBoxPlan IS NULL)
				BEGIN
					SET @OutputBoxPlan = 0;
				END
			
				IF @OutputBoxPlan > 0 AND @Outputbox > 0
					BEGIN
						SET @StatusDraft = 1;
					END
				ELSE IF @OutputBoxPlan = 0
					BEGIN
						SET @StatusDraft = 1;
					END;
				ELSE
					BEGIN
						SET @StatusDraft = 0;
					END;
			END
			-- EOF PENGECEKAN
			
			SET @StartDate = DATEADD(DAY, 1, @StartDate);
			-- END OF LOOP --
		END;
		
		DECLARE @IdFlow INT = 40;
		IF @StatusDraft = 1
		BEGIN
			SET @IdFlow = 41;
		END
		-- CHANGE STATUS BY TRANSLOG --
		INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
		VALUES( @TPOFeeCode, GETDATE(), @IdFlow, NULL, GETDATE(), @UserName, GETDATE(), @UserName );
		
		
END;
--END INSERT DATA TPOFeeProductionDaily ################################################
