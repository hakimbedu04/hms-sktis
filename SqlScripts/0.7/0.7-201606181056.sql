/****** Object:  StoredProcedure [dbo].[TPOProductionEntryVerificationCancelReport]    Script Date: 6/18/2016 10:55:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Description: Canceling Production Entry Verification Report
-- Ticket: http://tp.voxteneo.com/entity/57888
-- Author: Whisnu Sucitanuary

-- Add remove ExeReportByGroup, TransLog & ExeReportByProcess
-- Ticket : http://tp.voxteneo.co.id/entity/3282
-- Author : Robby Prima

-- Add ProductionDate param to ExeReportByGroup & ExeReportByProcess removal
-- Ticket : http://tp.voxteneo.co.id/entity/3282
-- Author : Dwi Yudha

-- Comment Delete UtilTransactionLogs
-- Ticket : http://tp.voxteneo.co.id/entity/7057
-- Author : Azka
-- Date	: 2016/06/18

ALTER PROCEDURE [dbo].[TPOProductionEntryVerificationCancelReport]
	@LocationCode varchar(8),
	@BrandCode varchar(11),
	@KPSYear int,
	@KPSWeek int,
	@ProductionDate date
AS
BEGIN
	DECLARE @TPOFeeCode varchar(64)
	DECLARE @TransactionLogCode varchar(64)
	DECLARE @TPOFeeHdrExists int
	DECLARE @DayOfWeek int

	-- GLOBAL DATA
	-- Master General Brand Group	
	DECLARE @BrandGroupCode varchar(20)
	SELECT
		@BrandGroupCode = bg.BrandGroupCode
	FROM MstGenBrandGroup bg
	INNER JOIN MstGenBrand b on b.BrandGroupCode = bg.BrandGroupCode 
	WHERE b.BrandCode = @BrandCode

	SET @DayOfWeek = CASE WHEN datepart(dw, @ProductionDate) = 1 THEN 7 ELSE datepart(dw, @ProductionDate) - 1 END
	SET @TPOFeeCode = 'FEE/' + @LocationCode + '/' + @BrandGroupCode + '/' + CAST(@KPSYear AS varchar) + '/' + CAST(@KPSWeek AS varchar)
	SET @TransactionLogCode = 'EBL/' + @LocationCode + '/%/' + @BrandCode + '/' + CAST(@KPSYear AS varchar) + '/' + CAST(@KPSWeek AS varchar) + '/' + CAST(@DayOfWeek AS varchar)

	--DELETE FROM dbo.UtilTransactionLogs WHERE TransactionCode like @TransactionLogCode
	
	SELECT @TPOFeeHdrExists = COUNT(*) FROM TPOFeeHdr WHERE TPOFeeCode = @TPOFeeCode AND Status = 'DRAFT';

	IF @TPOFeeHdrExists > 0
		BEGIN
		-- REMOVE DAILY FEE
		DELETE FROM TPOFeeProductionDaily where TPOFeeCode = @TPOFeeCode AND FeeDate = @ProductionDate

		-- RECALCULATE TPO Fee Calculation
		DECLARE @TPOFeeCalculationExist INT
		SELECT @TPOFeeCalculationExist = COUNT(*) FROM TPOFeeCalculation
		WHERE TPOFeeCode = @TPOFeeCode

			DECLARE @HDRTotalProdBox float
			DECLARE @HDRTotalProdJl1 float
			DECLARE @HDRTotalProdJl2 float
			DECLARE @HDRTotalProdJl3 float
			DECLARE @HDRTotalProdJl4 float
	
			SELECT @HDRTotalProdBox = TotalProdBox, @HDRTotalProdJl1 = TotalProdJl1, @HDRTotalProdJl2 = TotalProdJl2, @HDRTotalProdJl3 = TotalProdJl3, @HDRTotalProdJl4 = TotalProdJl4 FROM TPOFeeHdr
			WHERE TPOFeeCode = @TPOFeeCode AND LocationCode = @LocationCode AND BrandGroupCode = @BrandGroupCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND BrandGroupCode = @BrandGroupCode
	
			--DECLARE @DailyTotalJKNJam float
			--DECLARE @DailyTotalJL1Jam float
			--DECLARE @DailyTotalJL2Jam float
			--DECLARE @DailyTotalJL3Jam float
			--DECLARE @DailyTotalJL4Jam float

			--SELECT @DailyTotalJKNJam = SUM(COALESCE(JKNJam,0)), @DailyTotalJL1Jam = SUM(COALESCE(JL1Jam,0)), @DailyTotalJL2Jam = SUM(COALESCE(JL2Jam,0)), @DailyTotalJL3Jam = SUM(COALESCE(JL3Jam,0)), @DailyTotalJL4Jam = SUM(COALESCE(JL4Jam,0)) FROM TPOFeeProductionDaily
			--WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear and KPSWeek = @KPSWeek

			DECLARE @TPORateJKN float
			DECLARE @TPORateJl1 float
			DECLARE @TPORateJl2 float
			DECLARE @TPORateJl3 float
			DECLARE @TPORateJl4 float
			DECLARE @ManagementFee float
			DECLARE @ProductivityIncentives float

			SELECT @TPORateJKN = JKN, @TPORateJl1 = Jl1, @TPORateJl2 = Jl2, @TPORateJl3 = Jl3, @TPORateJl4 = Jl4, @ManagementFee = ManagementFee, @ProductivityIncentives = ProductivityIncentives FROM MstTPOFeeRate
			WHERE LocationCode= @LocationCode AND BrandGroupCode = @BrandGroupCode AND EffectiveDate = @ProductionDate

			--DECLARE @OutputBiaya1 float = (@DailyTotalJKNJam * @TPORateJKN)
			--DECLARE @OutputBiaya2 float = (@DailyTotalJL1Jam * @TPORateJl1)
			--DECLARE @OutputBiaya3 float = (@DailyTotalJL2Jam * @TPORateJl2)
			--DECLARE @OutputBiaya4 float = (@DailyTotalJL3Jam * @TPORateJl3)
			--DECLARE @OutputBiaya5 float = (@DailyTotalJL4Jam * @TPORateJl4)

			DECLARE @OutputBiaya1 float = (@TPORateJKN)
			DECLARE @OutputBiaya2 float = (@TPORateJl1)
			DECLARE @OutputBiaya3 float = (@TPORateJl2)
			DECLARE @OutputBiaya4 float = (@TPORateJl3)
			DECLARE @OutputBiaya5 float = (@TPORateJl4)


		IF @TPOFeeCalculationExist > 0 	
		BEGIN
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdBox,
				OutputBiaya = @OutputBiaya1,
				Calculate = @HDRTotalProdBox*@OutputBiaya1
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 1 AND ProductionFeeType = 'JKN'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdJl1,
				OutputBiaya = @OutputBiaya2,
				Calculate = @HDRTotalProdJl1*@OutputBiaya2
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 2 AND ProductionFeeType = 'JL1'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdJl2,
				OutputBiaya = @OutputBiaya3,
				Calculate = @HDRTotalProdJl2*@OutputBiaya3
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 3 AND ProductionFeeType = 'JL2'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdJl3,
				OutputBiaya = @OutputBiaya4,
				Calculate = @HDRTotalProdJl3*@OutputBiaya4
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 4 AND ProductionFeeType = 'JL3'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdJl4,
				OutputBiaya = @OutputBiaya5,
				Calculate = @HDRTotalProdJl4*@OutputBiaya5
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 5 AND ProductionFeeType = 'JL4'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdBox,		
				Calculate = @HDRTotalProdBox*@OutputBiaya1
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 6 AND ProductionFeeType = 'Biaya Produksi'	
			UPDATE TPOFeeCalculation SET
				OutputProduction = @HDRTotalProdBox,
				OutputBiaya = @ManagementFee,		
				Calculate = @HDRTotalProdBox*@ManagementFee
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 7 AND ProductionFeeType = 'Jasa Manajemen'
			UPDATE TPOFeeCalculation SET		
				Calculate = ((@HDRTotalProdBox*@OutputBiaya1)+(@HDRTotalProdBox*@ManagementFee)-(@HDRTotalProdBox*@ProductivityIncentives))
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 8 AND ProductionFeeType = 'Total Biaya Produksi & Jasa Manajemen'
			UPDATE TPOFeeCalculation SET				
				Calculate = (((@HDRTotalProdBox*@ManagementFee) * 2)/100)
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 9 AND ProductionFeeType = 'Pajak Jasa Manajemen Sebesar 2%'		
			UPDATE TPOFeeCalculation SET				
				Calculate = (((@HDRTotalProdBox*@OutputBiaya1)+(@HDRTotalProdBox*@ManagementFee)-(@HDRTotalProdBox*@ProductivityIncentives)) - (((@HDRTotalProdBox*@ManagementFee) * 2)/100))
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 10 AND ProductionFeeType = 'Total Biaya Yang Harus Dibayarkan Ke MPS'		
			UPDATE TPOFeeCalculation SET				
				Calculate = (((@HDRTotalProdBox*@OutputBiaya1)+(@HDRTotalProdBox*@ManagementFee)-(@HDRTotalProdBox*@ProductivityIncentives)) - (((@HDRTotalProdBox*@ManagementFee) * 2)/100))
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 11 AND ProductionFeeType = 'Pembayaran'
			UPDATE TPOFeeCalculation SET				
				Calculate = 0 --???
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 12 AND ProductionFeeType = 'Sisa yang harus dibayar'		
			UPDATE TPOFeeCalculation SET				
				Calculate = (((@HDRTotalProdBox*@ManagementFee) * 10)/100)
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 13 AND ProductionFeeType = 'PPN Biaya Produksi 10%'
			UPDATE TPOFeeCalculation SET				
				Calculate = (((@HDRTotalProdBox*@OutputBiaya1) * 10)/100)
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 14 AND ProductionFeeType = 'PPN Jasa Manajemen 10%'
			UPDATE TPOFeeCalculation SET				
				Calculate = ((((@HDRTotalProdBox*@OutputBiaya1)+(@HDRTotalProdBox*@ManagementFee)-(@HDRTotalProdBox*@ProductivityIncentives)) - (((@HDRTotalProdBox*@ManagementFee) * 2)/100)) + (((@HDRTotalProdBox*@ManagementFee) * 10)/100) + (((@HDRTotalProdBox*@OutputBiaya1) * 10)/100))
			WHERE TPOFeeCode = @TPOFeeCode AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND OrderFeeType = 15 AND ProductionFeeType = 'Total Bayar'

		END
	END

	delete from dbo.ExeReportByGroups where LocationCode = @LocationCode and KPSYear = @KPSYear and KPSWeek = @KPSWeek and BrandCode = @BrandCode and ProductionDate = @ProductionDate
	delete from dbo.ExeReportByProcess where LocationCode = @LocationCode and KPSYear = @KPSYear and KPSWeek = @KPSWeek and BrandCode = @BrandCode and ProductionDate = @ProductionDate

END



