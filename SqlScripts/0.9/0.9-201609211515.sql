-- =============================================
-- Description: change calculation rounding jkn~jl4
-- Ticket: http://tp.voxteneo.co.id/entity/7082
-- Author: Dwi Yudha
-- Version: 2.13
-- =============================================
-- =============================================
-- Description: Round PPN Biaya Produksi 10% , PPN Jasa Manajemen 10% & Total dibayar
-- Ticket: http://tp.voxteneo.co.id/entity/3447 point 1 & 5
-- Author: Abdurrahman Hakim
-- Date  : 2016-05-10 14:27
-- Version: 2.8
-- =============================================

-- =============================================
-- Description: change label Total Biaya Produksi & Jasa Maklon
-- Ticket: http://tp.voxteneo.co.id/entity/9980
-- Author: Abdurrahman Hakim
-- Date  : 2016-09-08 16:54
-- Version: 2.9
-- =============================================

ALTER TRIGGER [dbo].[TPOFeeProductionDailyAfterInsert] ON [dbo].[TPOFeeProductionDaily]
FOR INSERT, UPDATE, DELETE
AS

	SET NOCOUNT ON;

    --
    -- Check if this is an INSERT, UPDATE or DELETE Action.
    -- 
    DECLARE @action as char(1);

    SET @action = 'I'; -- Set Action to Insert by default.
    IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			SET @action = 
				CASE
					WHEN EXISTS(SELECT * FROM INSERTED) THEN 'U' -- Set Action to Updated.
					ELSE 'D' -- Set Action to Deleted.       
				END;
		END;
    ELSE 
		BEGIN
			IF NOT EXISTS(SELECT * FROM INSERTED) RETURN; -- Nothing updated or inserted.
		END;
	
	
	DECLARE @parentid       VARCHAR(64),
			@kpsYear        INT,
			@kpsWeek        INT,
			@ProductionDate DATETIME,
			@LocationCode varchar(8),
			@BrandGroupCode VARCHAR(20),
			@TPOPackageValue FLOAT,
			@StickPerBox INT;

	DECLARE @TotalProdStick FLOAT;
	DECLARE @TotalProdBox FLOAT;
	DECLARE @TotalProdJKN FLOAT;
	DECLARE @TotalProdJl1 FLOAT;
	DECLARE @TotalProdJl2 FLOAT;
	DECLARE @TotalProdJl3 FLOAT;
	DECLARE @TotalProdJl4 FLOAT;
	DECLARE @tProdJKN FLOAT;
	DECLARE @tProdJl1 FLOAT;
	DECLARE @tProdJl2 FLOAT;
	DECLARE @tProdJl3 FLOAT;
	DECLARE @tProdJl4 FLOAT;
	DECLARE @TotalJKN FLOAT;
	DECLARE @TotalJL1 FLOAT;
	DECLARE @TotalJL2 FLOAT;
	DECLARE @TotalJL3 FLOAT;
	DECLARE @TotalJL4 FLOAT;

	SELECT @parentid = i.TPOFeeCode, @kpsYear = i.KPSYear, @kpsWeek = i.KPSWeek, @ProductionDate = FeeDate FROM inserted i;

	SELECT @TotalProdStick = SUM(OuputSticks) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalProdBox = SUM(OutputBox) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @tProdJKN = SUM(JKN) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @tProdJl1 = SUM(JL1) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @tProdJl2 = SUM(JL2) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @tProdJl3 = SUM(JL3) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @tProdJl4 = SUM(JL4) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalJKN = SUM(JKNJam) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalJL1 = SUM(JL1Jam) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalJL2 = SUM(JL2Jam) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalJL3 = SUM(JL3Jam) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;
	SELECT @TotalJL4 = SUM(JL4Jam) FROM TPOFeeProductionDaily WHERE TPOFeeCode = @parentid;

	-- PERHITUNGAN SEBELUMNYA
	-- DECLARE @Sisa FLOAT;
	-- SET @Sisa = @TotalProdJl4 - FLOOR(@TotalProdJl4);
	-- IF @Sisa > 0
	-- BEGIN
	   -- SET @TotalProdJl4 = FLOOR(@TotalProdJl4);
	   -- SET @TotalProdJl3 = @TotalProdJl3 + @Sisa;
	-- END

	-- SET @Sisa = @TotalProdJl3 - FLOOR(@TotalProdJl3);
	-- IF @Sisa > 0
	-- BEGIN
	   -- SET @TotalProdJl3 = FLOOR(@TotalProdJl3);
	   -- SET @TotalProdJl2 = @TotalProdJl2 + @Sisa;
	-- END

	-- SET @Sisa = @TotalProdJl2 - FLOOR(@TotalProdJl2);
	-- IF @Sisa > 0
	-- BEGIN
	   -- SET @TotalProdJl2 = FLOOR(@TotalProdJl2);
	   -- SET @TotalProdJl1 = @TotalProdJl1 + @Sisa;
	-- END

	-- SET @Sisa = @TotalProdJl1 - FLOOR(@TotalProdJl1);
	-- IF @Sisa > 0
	-- BEGIN
	   -- SET @TotalProdJl1 = FLOOR(@TotalProdJl1);
	   -- SET @TotalProdJKN = @TotalProdJKN + @Sisa;
	-- END

	-- SET @TotalProdJKN = ROUND(@TotalProdJKN, 0)
	
	-- PERHITUNGAN BARU
	
	SELECT @BrandGroupCode = BrandGroupCode, @LocationCode = LocationCode
	FROM TPOFeeHdr WHERE TPOFeeCode = @parentid;
	
	DECLARE @StdStickPerHours INT,
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
	 
	SELECT @StickPerBox = bg.StickPerBox
	FROM MstGenBrandGroup bg
	WHERE bg.BrandGroupCode = @BrandGroupCode;
	
	SET @TotalProdJKN = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJKN * @EmpPackage ) / @StickPerBox ), 4);
	SET @TotalProdJl1 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL1 * @EmpPackage ) / @StickPerBox ), 4);
	SET @TotalProdJl2 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL2 * @EmpPackage ) / @StickPerBox ), 4);
	SET @TotalProdJl3 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL3 * @EmpPackage ) / @StickPerBox ), 4);
	SET @TotalProdJl4 = ROUND((( @TPOPackageValue * @StdStickPerHours * @TotalJL4 * @EmpPackage ) / @StickPerBox ), 4);
	
	-- JKN (round tanpa desimal)
	IF (ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @tProdJl3 - @tProdJl4, 4) <= @TotalProdJKN)
		BEGIN
			SET @TotalProdJKN = ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @tProdJl3 - @tProdJl4, 4);
		END
	
	-- JL1 (round tanpa desimal)
	IF(ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @tProdJl3 - @tProdJl4, 4) > 0 )
		BEGIN
			IF(ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @tProdJl3 - @tProdJl4, 4) <= @TotalProdJl1)
				BEGIN
					SET @TotalProdJl1 = ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @tProdJl3 - @tProdJl4, 4);
				END
		END
	ELSE
		BEGIN
			SET @TotalProdJl1 = 0;
		END
		
	-- JL2
	IF(ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @TotalProdJl1 - @tProdJl3 - @tProdJl4, 4) > 0 )
		BEGIN
			-- SET @TotalProdJl2 = ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @TotalProdJl1 - @tProdJl3 - @tProdJl4, 2);
			SET @TotalProdJl2 = ROUND(@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @TotalProdJl1 - @tProdJl3 - @tProdJl4, 4);
			--SET @TotalProdJl2 = (FLOOR((@tProdJKN + @tProdJl1 + @tProdJl2 - @TotalProdJKN - @TotalProdJl1 - @tProdJl3 - @tProdJl4) * 100)) / 100;
		END
	ELSE
		BEGIN
			SET @TotalProdJl2 = 0;
		END
		
	-- JL3
	DECLARE @Standard60 FLOAT = ROUND((( @TPOPackageValue * @StdStickPerHours * @EmpPackage * 60 ) / @StickPerBox ), 4);
	IF(@TotalProdBox - @Standard60 <= @TotalProdJl3)
		BEGIN
			IF(@TotalProdBox - @Standard60 > 0)
				BEGIN
					SET @TotalProdJl3 = @TotalProdBox - @Standard60;
				END
			ELSE
				BEGIN
					SET @TotalProdJl3 = 0;
				END
		END
		
	-- Jl4
	IF(@TotalProdBox - @Standard60 - @TotalProdJl3 > 0)
		BEGIN
			SET @TotalProdJl4 = @TotalProdBox - @Standard60 - @TotalProdJl3;
		END
	ELSE
		BEGIN
			SET @TotalProdJl4 = 0;
		END
	
	--- EOF PERHITUNGAN BARU

	UPDATE TPOFeeHdr
       SET TotalProdStick = @TotalProdStick,
           TotalProdBox = @TotalProdBox,
           TotalProdJKN = ROUND(@TotalProdJKN,0),
           TotalProdJl1 = ROUND(@TotalProdJl1,0),
           TotalProdJl2 = ROUND(@TotalProdJl2,0),
           TotalProdJl3 = ROUND(@TotalProdJl3,0),
           TotalProdJl4 = ROUND(@TotalProdJl4,0),
           TotalJKN = @TotalJKN,
           TotalJL1 = @TotalJL1,
           TotalJL2 = @TotalJL2,
           TotalJL3 = @TotalJL3,
           TotalJL4 = @TotalJL4
     WHERE TPOFeeCode = @parentid;

     DECLARE @TPORateJKN             FLOAT,
             @TPORateJl1             FLOAT,
             @TPORateJl2             FLOAT,
             @TPORateJl3             FLOAT,
             @TPORateJl4             FLOAT,
             @ManagementFee          FLOAT,
             @ProductivityIncentives FLOAT;


     SELECT TOP 1 @TPORateJKN = JKN,
                  @TPORateJl1 = Jl1,
                  @TPORateJl2 = Jl2,
                  @TPORateJl3 = Jl3,
                  @TPORateJl4 = Jl4,
                  @ManagementFee = dbo.MstTPOFeeRate.ManagementFee,
                  @ProductivityIncentives = dbo.MstTPOFeeRate.ProductivityIncentives
     FROM MstTPOFeeRate
     WHERE LocationCode = @LocationCode
       AND BrandGroupCode = @BrandGroupCode
       AND @ProductionDate >= EffectiveDate
       AND @ProductionDate <= ExpiredDate
     ORDER BY ManagementFee, ProductivityIncentives DESC;

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

	 -- SET @ProductivityIncentives IF NULL
	 IF (@ProductivityIncentives IS NULL)
	 BEGIN
		SET @ProductivityIncentives = 0;
	 END;

     -- JKN #OrderFeeType 1
     -- TODO JKN @OutputProduction1
     -- SUM JKNJam di TPOFeeDaily => @SumJKNJam
     -- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJKNJam * 
     -- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
     SET @OutputProduction1 = ROUND(@TotalProdJKN,0);
     --SET @OutputProduction1 = @TotalProdJKN--( @TPOPackageValue * @StdStickPerHours * @TotalJKN * @EmpPackage ) / @StickPerBox;
     SET @OutputBiaya1 = @TPORateJKN;
     SET @Calculate1 = @OutputProduction1 * @OutputBiaya1;

     -- JL1 #OrderFeeType 2
     -- TODO JKN @OutputProduction2
     -- SUM JKNJam di TPOFeeDaily => @SumJL1Jam
     -- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL1Jam * 
     -- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
     SET @OutputProduction2 = ROUND(@TotalProdJl1,0);
     --SET @OutputProduction2 = @TotalProdJl1--( @TPOPackageValue * @StdStickPerHours * @TotalJL1 * @EmpPackage ) / @StickPerBox;
     SET @OutputBiaya2 = @TPORateJl1;
     SET @Calculate2 = @OutputProduction2 * @OutputBiaya2;

     -- JL2 #OrderFeeType 3
     -- TODO JKN @OutputProduction2
     -- SUM JKNJam di TPOFeeDaily => @SumJL2Jam
     -- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL2Jam * 
     -- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
     SET @OutputProduction3 = ROUND(@TotalProdJl2, 0);
     --SET @OutputProduction3 = @TotalProdJl2--( @TPOPackageValue * @StdStickPerHours * @TotalJL2 * @EmpPackage ) / @StickPerBox;
     SET @OutputBiaya3 = @TPORateJl2;
     SET @Calculate3 = @OutputProduction3 * @OutputBiaya3;

     -- JL3 #OrderFeeType 4
     -- TODO JKN @OutputProduction3
     -- SUM JKNJam di TPOFeeDaily => @SumJL3Jam
     -- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL3Jam * 
     -- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
     SET @OutputProduction4 = ROUND(@TotalProdJl3, 0);
     --SET @OutputProduction4 = @TotalProdJl3--( @TPOPackageValue * @StdStickPerHours * @TotalJL3 * @EmpPackage ) / @StickPerBox;
     SET @OutputBiaya4 = @TPORateJl3;
     SET @Calculate4 = @OutputProduction4 * @OutputBiaya4;
	
     -- JL4 #OrderFeeType 5
     -- TODO JKN @OutputProduction4
     -- SUM JKNJam di TPOFeeDaily => @SumJL4Jam
     -- @TPOPackageValue * (Get data StdStickPerHour dari view ProcessSettingsAndLocationView berdasarkan Filter dan ROLLING) * @SumJL4Jam * 
     -- (Get data EmpPackage from MstGenBrandGroup where Filter) / @StickPerBox
     SET @OutputProduction5 = ROUND(@TotalProdJl4, 0);
     --SET @OutputProduction5 = @TotalProdJl4 --( @TPOPackageValue * @StdStickPerHours * @TotalJL4 * @EmpPackage ) / @StickPerBox;
     SET @OutputBiaya5 = @TPORateJl4;
     SET @Calculate5 = @OutputProduction5 * @OutputBiaya5;

     -- Biaya Produksi #OrderFeeType 6
     -- TODO Biaya Produksi @OutputProduction6 = #OrderFeeType 1 + #OrderFeeType 2 + #OrderFeeType 3 + #OrderFeeType 4
     -- TODO Biaya Produksi @Calculate6 = #Calculate1 + #Calculate2 + #Calculate3 + #Calculate4
     SET @OutputProduction6 = @TotalProdJKN + @TotalProdJl1 +@TotalProdJl2 +@TotalProdJl3 +@TotalProdJl4;
     SET @OutputBiaya6 = NULL;
     SET @Calculate6 = @Calculate1 + @Calculate2 + @Calculate3 + @Calculate4 + @Calculate5;

     -- Jasa Maklon #OrderFeeType 7
     -- TODO Jasa Maklon @OutputProduction7 = @OutputProduction6
     SET @OutputProduction7 = @TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 + @TotalProdJl3 + @TotalProdJl4;
     SET @OutputBiaya7 = @ManagementFee;
     SET @Calculate7 = @OutputProduction7 * @OutputBiaya7;

     -- Productivity Incentives #OrderFeeType 16
     -- TODO Productivity Incentives @OutputProduction16 = @OutputProduction6
     SET @OutputProduction16 = @TotalProdJKN + @TotalProdJl1 + @TotalProdJl2 + @TotalProdJl3 + @TotalProdJl4;
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

     IF @parentid IS NULL
         BEGIN
             RETURN;
         END;


             IF NOT EXISTS( SELECT * FROM TPOFeeCalculation WHERE dbo.TPOFeeCalculation.TPOFeeCode = @parentid )
                 BEGIN
                     INSERT INTO dbo.TPOFeeCalculation
                            ( TPOFeeCode,
                              ProductionFeeType,
                              KPSYear,
                              KPSWeek,
                              OrderFeeType,
                              OutputProduction,
                              OutputBiaya,
                              Calculate
                            )
                     VALUES( @parentid, 'JKN', @KPSYear, @KPSWeek, 1, @OutputProduction1, @OutputBiaya1, FLOOR(@Calculate1) ),
                           ( @parentid, 'JL1', @KPSYear, @KPSWeek, 2, @OutputProduction2, @OutputBiaya2, FLOOR(@Calculate2) ),
                           ( @parentid, 'JL2', @KPSYear, @KPSWeek, 3, @OutputProduction3, @OutputBiaya3, FLOOR(@Calculate3) ),
                           ( @parentid, 'JL3', @KPSYear, @KPSWeek, 4, @OutputProduction4, @OutputBiaya4, FLOOR(@Calculate4) ),
                           ( @parentid, 'JL4', @KPSYear, @KPSWeek, 5, @OutputProduction5, @OutputBiaya5, FLOOR(@Calculate5) ),
                           ( @parentid, 'Biaya Produksi', @KPSYear, @KPSWeek, 6, @OutputProduction6, @OutputBiaya6, FLOOR(@Calculate6) ),
                           ( @parentid, 'Jasa Manajemen', @KPSYear, @KPSWeek, 7, @OutputProduction7, @OutputBiaya7, FLOOR(@Calculate7) ),
                           ( @parentid, 'Productivity Incentives', @KPSYear, @KPSWeek, 16, @OutputProduction16, @OutputBiaya16, FLOOR(@Calculate16) ),
                           ( @parentid, 'Total Biaya Produksi & Jasa Manajemen', @KPSYear, @KPSWeek, 8, @OutputProduction8, @OutputBiaya8, FLOOR(@Calculate8) ),
                           ( @parentid, 'Pajak Jasa Manajemen Sebesar 2%', @KPSYear, @KPSWeek, 9, @OutputProduction9, @OutputBiaya9, FLOOR(@Calculate9) ),
                           ( @parentid, 'Pajak Productivity Incentives sebesar 2 %', @KPSYear, @KPSWeek, 17, @OutputProduction17, @OutputBiaya17, FLOOR(@Calculate17) ),
                           ( @parentid, 'Total Biaya Yang Harus Dibayarkan Ke MPS', @KPSYear, @KPSWeek, 10, @OutputProduction10, @OutputBiaya10, FLOOR(@Calculate10) ),
                           ( @parentid, 'Pembayaran', @KPSYear, @KPSWeek, 11, @OutputProduction11, @OutputBiaya11, FLOOR(@Calculate11) ),
                           ( @parentid, 'Sisa yang harus dibayar', @KPSYear, @KPSWeek, 12, @OutputProduction12, @OutputBiaya12, FLOOR(@Calculate12) ),
                           ( @parentid, 'PPN Biaya Produksi 10%', @KPSYear, @KPSWeek, 13, @OutputProduction13, @OutputBiaya13, FLOOR(@Calculate13) ),
                           ( @parentid, 'PPN Jasa Manajemen 10%', @KPSYear, @KPSWeek, 14, @OutputProduction14, @OutputBiaya14, FLOOR(@Calculate14) ),
                           ( @parentid, 'PPN Productivity Incentives 10 %', @KPSYear, @KPSWeek, 18, @OutputProduction18, @OutputBiaya18, FLOOR(@Calculate18) ),
                           ( @parentid, 'Total Bayar', @KPSYear, @KPSWeek, 15, @OutputProduction15, @OutputBiaya15, FLOOR(@Calculate15) );
                 END;
             ELSE
                 BEGIN
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction1, OutputBiaya = @OutputBiaya1, Calculate = FLOOR(@Calculate1) WHERE TPOFeeCode = @parentid AND OrderFeeType = 1;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction2, OutputBiaya = @OutputBiaya2, Calculate = FLOOR(@Calculate2) WHERE TPOFeeCode = @parentid AND OrderFeeType = 2;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction3, OutputBiaya = @OutputBiaya3, Calculate = FLOOR(@Calculate3) WHERE TPOFeeCode = @parentid AND OrderFeeType = 3;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction4, OutputBiaya = @OutputBiaya4, Calculate = FLOOR(@Calculate4) WHERE TPOFeeCode = @parentid AND OrderFeeType = 4; 
					 UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction5, OutputBiaya = @OutputBiaya5, Calculate = FLOOR(@Calculate5) WHERE TPOFeeCode = @parentid AND OrderFeeType = 5;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction6, OutputBiaya = @OutputBiaya6, Calculate = FLOOR(@Calculate6) WHERE TPOFeeCode = @parentid AND OrderFeeType = 6;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction7, OutputBiaya = @OutputBiaya7, Calculate = FLOOR(@Calculate7) WHERE TPOFeeCode = @parentid AND OrderFeeType = 7;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction16, OutputBiaya = @OutputBiaya16, Calculate = FLOOR(@Calculate16) WHERE TPOFeeCode = @parentid AND OrderFeeType = 16;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction8, OutputBiaya = @OutputBiaya8, Calculate = FLOOR(@Calculate8) WHERE TPOFeeCode = @parentid AND OrderFeeType = 8;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction9, OutputBiaya = @OutputBiaya9, Calculate = FLOOR(@Calculate9) WHERE TPOFeeCode = @parentid AND OrderFeeType = 9;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction17, OutputBiaya = @OutputBiaya17, Calculate = FLOOR(@Calculate17) WHERE TPOFeeCode = @parentid AND OrderFeeType = 17;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction10, OutputBiaya = @OutputBiaya10, Calculate = FLOOR(@Calculate10) WHERE TPOFeeCode = @parentid AND OrderFeeType = 10;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction11, OutputBiaya = @OutputBiaya11, Calculate = FLOOR(@Calculate11) WHERE TPOFeeCode = @parentid AND OrderFeeType = 11;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction12, OutputBiaya = @OutputBiaya12, Calculate = FLOOR(@Calculate12) WHERE TPOFeeCode = @parentid AND OrderFeeType = 12;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction13, OutputBiaya = @OutputBiaya13, Calculate = FLOOR(@Calculate13) WHERE TPOFeeCode = @parentid AND OrderFeeType = 13;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction14, OutputBiaya = @OutputBiaya14, Calculate = FLOOR(@Calculate14) WHERE TPOFeeCode = @parentid AND OrderFeeType = 14;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction18, OutputBiaya = @OutputBiaya18, Calculate = FLOOR(@Calculate18) WHERE TPOFeeCode = @parentid AND OrderFeeType = 18;
                     UPDATE dbo.TPOFeeCalculation SET OutputProduction = @OutputProduction15, OutputBiaya = @OutputBiaya15, Calculate = FLOOR(@Calculate15) WHERE TPOFeeCode = @parentid AND OrderFeeType = 15;
                 END;
