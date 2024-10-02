IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleButtonWagesApprovalDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RoleButtonWagesApprovalDetail]
GO


CREATE PROC [dbo].[RoleButtonWagesApprovalDetail] 
-- Add the parameters for the stored procedure here
@locationCode		VARCHAR(250),
@unitCode			VARCHAR(250),
@shift				VARCHAR(250),
@Date				VARCHAR(250),
@brandGroupCode     VARCHAR(250),
@brandCode     VARCHAR(250),
@RoleId INT
       
AS
BEGIN
	DECLARE @TempTableTotalTransactionlogSubmit TABLE (Location VARCHAR(4), Unit VARCHAR(4), BrandCode VARCHAR(20), Total INT);
	DECLARE @LastTransLogs INT;
	DECLARE @DAY VARCHAR(2);
	DECLARE @WEEK VARCHAR(2);
	DECLARE @YEAR VARCHAR(4);
	DECLARE @totalEntryVerification INT;
	DECLARE @totalTransactionlogSubmit INT;
	DECLARE @idNextRole INT;
	DECLARE @Result TABLE (Result1 BIT,Result2 BIT,Result3 BIT); --approve,return,complete

	--SET @locationCode = 'ID21'
	--SET @unitCode = '2025'
	--SET @shift = '1'
	--SET @Date = CONVERT(DATETIME,'2016-12-05')
	--SET @brandGroupCode = 'SAO12SR-20'  --'SAO12SR-20'--
	--SET @brandCode = 'SAO12SR-20' --'FA044773.05' --
	--SET @RoleId = 14
	SET @DAY = (SELECT Case (SELECT DATEPART( DW, @Date)-1) WHEN 0 THEN 7 ELSE (SELECT DATEPART( DW, @Date)-1) END)
	SET @WEEK = (SELECT [Week] FROM [dbo].[MstGenWeek] WHERE StartDate <= @Date AND EndDate >= @Date ) -- (CONVERT(VARCHAR(2), @StartDate, 105));
	SET @YEAR = (CONVERT(varchar(4),(SELECT DATEPART( YY, @Date))));

	IF(@brandCode != @brandGroupCode)
	BEGIN
		 INSERT INTO @TempTableTotalTransactionlogSubmit
		 SELECT substring(x.TransactionCode,5,4) Location
		,SUBSTRING(x.TransactionCode,12,4) Unit
		,SUBSTRING(x.TransactionCode,22,11) BrandCode
		,COUNT(*) HRASubmit
		from (
				SELECT [TransactionCode]
				,max([IDFlow]) IDFlow
				FROM [UtilTransactionLogs] a
				where TransactionCode like 'WPC/'+@locationCode+'/1/'+@unitCode+'/%/'+@brandCode+'/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'
				and a.CreatedDate=(select MAX(CreatedDate) from
				[dbo].[UtilTransactionLogs]
				where TransactionCode=a.TransactionCode)
				group by a.TransactionCode
			) x
		where x.IDFlow in (69,23,21,22,24)
		group by substring(x.TransactionCode,5,4),
		SUBSTRING(x.TransactionCode,12,4) ,
		SUBSTRING(x.TransactionCode,22,11)
		ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;

		-- SELECT CONDITIONAL
			SELECT @LastTransLogs = (SELECT TOP 1 IDFlow FROM UtilTransactionLogs
			WHERE TransactionCode LIKE 'WPC/'+@locationCode+'/1/'+@unitCode+'/%/'+@brandCode+'/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%' ORDER BY UpdatedDate DESC);
	
			SELECT @totalEntryVerification = (SELECT COUNT(*) FROM ExePlantProductionEntryVerificationView
			WHERE LocationCode = @locationCode
			AND UnitCode = @unitCode
			AND BrandCode = @brandCode
			AND ProductionDate = @Date
			AND SUBSTRING(GroupCode,2,1) != '5'
			GROUP BY BrandCode);

			SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
			WHERE Location = @locationCode AND Unit = @unitCode AND BrandCode = @brandCode),0);
		-- SELECT CONDITIONAL

		IF(@totalEntryVerification - @totalTransactionlogSubmit = 0)
		BEGIN
			-- STATUS SUBMITTED
			-- allowed role PPC 14
			SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 22;
			IF(@RoleId = @idNextRole)
			BEGIN
				INSERT INTO @Result (Result1,Result2,Result3) values (1,0,0);
			END
		END
		ELSE IF(@totalEntryVerification - @totalTransactionlogSubmit > 0)
		BEGIN
			-- STATUS DRAFT
			-- allowed role paaprd 7
			SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 21;
			IF(@RoleId = @idNextRole)
			BEGIN
				INSERT INTO @Result (Result1,Result2,Result3) values (0,0,0);
			END
		END
		IF(@LastTransLogs = 56)
		BEGIN
			-- STATUS APPROVED
			-- allowed role pgsc 6
			SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 56;
			IF(@RoleId = @idNextRole)
			BEGIN
				INSERT INTO @Result (Result1,Result2,Result3) values (1,1,0);
			END
		END

		IF((@LastTransLogs = 25) OR (@LastTransLogs = 70)) -- status completed
		BEGIN
			-- allowed role payroll 15
			BEGIN
				INSERT INTO @Result (Result1,Result2,Result3) values (0,1,1);
			END
		END
	END
	ELSE
	BEGIN
	
		DECLARE @TempTable3 TABLE (ApproveBtn INT,ReturnBtn INT,CompleteBtn INT)
		DECLARE @BrandCodeCursor VARCHAR(20);
		DECLARE @var1 INT
		DECLARE @var2 INT
		DECLARE @var3 INT
		
		DECLARE Cursor_RoleBtn CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
			SELECT BrandCode from ProductionCard 
			WHERE LocationCode = @locationCode
			and UnitCode = @unitCode
			and ProductionDate = @Date
			and BrandGroupCode = @brandGroupCode
			GROUP BY BrandCode,BrandGroupCode
			
		OPEN Cursor_RoleBtn
		FETCH NEXT FROM Cursor_RoleBtn INTO @BrandCodeCursor
		WHILE @@FETCH_STATUS = 0 BEGIN 
	
			INSERT INTO @TempTableTotalTransactionlogSubmit
			 SELECT substring(x.TransactionCode,5,4) Location
			,SUBSTRING(x.TransactionCode,12,4) Unit
			,SUBSTRING(x.TransactionCode,22,11) BrandCode
			,COUNT(*) HRASubmit
			from (
					SELECT [TransactionCode]
					,max([IDFlow]) IDFlow
					FROM [UtilTransactionLogs] a
					where TransactionCode like 'WPC/'+@locationCode+'/1/'+@unitCode+'/%/'+@BrandCodeCursor+'/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'
					and a.CreatedDate=(select MAX(CreatedDate) from
					[dbo].[UtilTransactionLogs]
					where TransactionCode=a.TransactionCode)
					group by a.TransactionCode
				) x
			where x.IDFlow in (69,23,21,22,24)
			group by substring(x.TransactionCode,5,4),
			SUBSTRING(x.TransactionCode,12,4) ,
			SUBSTRING(x.TransactionCode,22,11)
			ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;
			
			-- SELECT CONDITIONAL
				SELECT @LastTransLogs = (SELECT TOP 1 IDFlow FROM UtilTransactionLogs
				WHERE TransactionCode LIKE 'WPC/'+@locationCode+'/1/'+@unitCode+'/%/'+@BrandCodeCursor+'/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%' ORDER BY UpdatedDate DESC);
	
				SELECT @totalEntryVerification = (SELECT COUNT(*) FROM ExePlantProductionEntryVerificationView
				WHERE LocationCode = @locationCode
				AND UnitCode = @unitCode
				AND BrandCode = @BrandCodeCursor
				AND ProductionDate = @Date
				AND SUBSTRING(GroupCode,2,1) != '5'
				GROUP BY BrandCode);

				SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
				WHERE Location = @locationCode AND Unit = @unitCode AND BrandCode = @BrandCodeCursor),0);
			-- SELECT CONDITIONAL
		
			IF(@totalEntryVerification - @totalTransactionlogSubmit = 0)
			BEGIN
				-- STATUS SUBMITTED
				-- allowed role PPC 14
				SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 22;
				IF(@RoleId = @idNextRole)
				BEGIN
					INSERT INTO @Result (Result1,Result2,Result3) values (1,0,0);
				END
			END
			ELSE IF(@totalEntryVerification - @totalTransactionlogSubmit > 0)
			BEGIN
				-- STATUS DRAFT
				-- allowed role paaprd 7
				SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 21;
				IF(@RoleId = @idNextRole)
				BEGIN
					INSERT INTO @Result (Result1,Result2,Result3) values (0,0,0);
				END
			END
			IF(@LastTransLogs = 56)
			BEGIN
				-- STATUS APPROVED
				-- allowed role pgsc 6
				SELECT @idNextRole = DestinationRole FROM UtilFlows WHERE IDFlow = 56;
				IF(@RoleId = @idNextRole)
				BEGIN
					INSERT INTO @Result (Result1,Result2,Result3) values (1,1,0);
				END
			END

			IF((@LastTransLogs = 25) OR (@LastTransLogs = 70)) -- status completed
			BEGIN
				-- allowed role payroll 15
				BEGIN
					INSERT INTO @Result (Result1,Result2,Result3) values (0,1,1);
				END
			END
		
		FETCH NEXT FROM Cursor_RoleBtn INTO @BrandCodeCursor
		END
		CLOSE Cursor_RoleBtn
		DEALLOCATE Cursor_RoleBtn
		
		SELECT @var1 = Result1 * COALESCE(@var1, 1) FROM @Result
		SELECT @var2 = Result2 * COALESCE(@var2, 1) FROM @Result
		SELECT @var3 = Result3 * COALESCE(@var3, 1) FROM @Result
	
		DELETE FROM @Result
		INSERT INTO @Result (Result1,Result2,Result3) values (ISNULL(@var1,0),ISNULL(@var2,0),ISNULL(@var3,0));
	
	END

	select ISNULL(Result1,0) as Result1,ISNULL(Result2,0) as Result2,ISNULL(Result3,0) as Result3 from @Result;
END;