SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SUBMIT_TPO_ENTRY_VERIFICATION]
		@paramLocationCode	VARCHAR(4)
	,	@paramBrandCode		VARCHAR(20)
	,	@paramKPSYear		INT
	,	@paramKPSWeek		INT
	,	@paramProdDate		DATE
	,	@paramListProcess	VARCHAR(100)
	,	@paramUser			VARCHAR(50)
	,	@paramRole			INT
AS
BEGIN
	SET NOCOUNT ON;
	SET DATEFIRST 1;

	DECLARE @splitProcess VARCHAR(20);
	DECLARE @IsGenerateTPOFee BIT;
	DECLARE @datenow DATETIME = GETDATE();
	
	-- Insert By group
	EXEC [dbo].[InsertTPOExeReportByGroups] @paramLocationCode, @paramBrandCode, @paramKPSYear, @paramKPSWeek,
	@paramProdDate, @paramUser
	
	--- START Check Status TPO Fee ---
	DECLARE @brandGroupCode VARCHAR(20);
	SELECT @brandGroupCode = BrandGroupCode FROM MstGenBrand WHERE BrandCode = @paramBrandCode;
	
	DECLARE @statusTPOFee VARCHAR(10);
	DECLARE @tpoFeeCode VARCHAR(50);
	
	SET @tpoFeeCode = 'FEE/' + @paramLocationCode + 
						'/' + @brandGroupCode + 
						'/' + CONVERT(varchar, @paramKPSYear) + 
						'/' + CONVERT(varchar, @paramKPSWeek);
						
	IF EXISTS
	(
		SELECT * FROM TPOFeeApprovalView 
		WHERE LocationCode = @paramLocationCode AND TPOFeeCode = @tpoFeeCode
	)
	BEGIN
		SELECT @statusTPOFee = Status FROM TPOFeeApprovalView 
		WHERE LocationCode = @paramLocationCode AND TPOFeeCode = @tpoFeeCode;
	END
	ELSE
	BEGIN
		SET @statusTPOFee = '';
	END
	--- END Check Status TPO Fee ---
	
	-- SUBMIT BEGIN
	IF (@statusTPOFee = 'DRAFT' OR @statusTPOFee = 'OPEN' OR @statusTPOFee = 'REVISED' OR @statusTPOFee = '')
	BEGIN
		-- Loop process
		DECLARE cursor_process CURSOR LOCAL FOR SELECT splitdata from dbo.fnSplitString(@paramListProcess,';')
		OPEN cursor_process
		FETCH NEXT FROM cursor_process   
		INTO @splitProcess

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			IF (@splitProcess = 'CUTTIN')
			BEGIN
				SET @IsGenerateTPOFee = 1;
			END
			
			DECLARE @processIdentifier INT;
			SELECT @processIdentifier = ProcessIdentifier FROM MstGenProcess WHERE ProcessGroup = @splitProcess;
			
			DECLARE @transactionCode VARCHAR(50);
			SET @transactionCode = 'EBL/' + @paramLocationCode +
									'/' + CONVERT(varchar, @processIdentifier) +
									'/' + @paramBrandCode +
									'/' + CONVERT(varchar, @paramKPSYear) +
									'/' + CONVERT(varchar, @paramKPSWeek) +
									'/' + CONVERT(varchar,(select datepart(dw, @paramProdDate)));
			
			-- Update verify system, manual and flag
			UPDATE ExeTPOProductionEntryVerification
			SET VerifyManual = 1, VerifySystem = 1, Flag_Manual = 1
			WHERE ProductionEntryCode = @transactionCode;
			
			EXEC [dbo].[TransactionLog] '', 'TPOProductionEntryVerification', 0, 0, '', '', '', '' , '', '', '', '', '',
								 @datenow, 'Submit', @datenow, @paramUser, @transactionCode, '', @paramRole;
			
			FETCH NEXT FROM cursor_process INTO @splitProcess
		END

		CLOSE cursor_process; DEALLOCATE cursor_process;  
	END
	
	IF (@paramListProcess <> '')
	BEGIN
		DECLARE @processToGenerate VARCHAR(20);
	
		DECLARE cursor_process2 CURSOR LOCAL FOR 
		SELECT ProcessGroup FROM ProcessSettingsAndLocationView 
		WHERE LocationCode = @paramLocationCode AND BrandGroupCode = @brandGroupCode
		ORDER BY ProcessOrder DESC
		
		OPEN cursor_process2
		FETCH NEXT FROM cursor_process2   
		INTO @processToGenerate

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			EXEC [dbo].[BaseExeReportByProcess] @paramLocationCode, @paramBrandCode, @processToGenerate,
												@paramKPSYear, @paramKPSWeek, @paramUser, @paramUser,
												@paramProdDate, 'PROD'
			
			FETCH NEXT FROM cursor_process2 INTO @processToGenerate
		END

		CLOSE cursor_process2; DEALLOCATE cursor_process2;  
		
	END
	
	IF (@IsGenerateTPOFee = 1)
	BEGIN
		EXEC [dbo].[TPOProductionEntryVerificationGenerateReport] @paramLocationCode, @paramBrandCode,
																	@paramKPSYear, @paramKPSWeek, @paramProdDate,
																	@paramUser
	END
END
GO