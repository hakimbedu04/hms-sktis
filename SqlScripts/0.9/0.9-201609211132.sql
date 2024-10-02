CREATE PROCEDURE [dbo].[RoleButtonWagesApprovalDetail] 
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
		DECLARE @TempTable1 TABLE (brandCodeTemp VARCHAR(250))
		DECLARE @TempTable2 TABLE (groupCodeTemp VARCHAR(250))
		DECLARE @TempTable3 TABLE (ApproveBtn INT,ReturnBtn INT,CompleteBtn INT)
		DECLARE @TempTable4 TABLE (Result1 BIT,Result2 BIT,Result3 BIT)
		DECLARE @TransactionCode VARCHAR(250)
		DECLARE @brandCodeTemp VARCHAR(250)
		DECLARE @groupCodeTemp VARCHAR(250)
		DECLARE @day   VARCHAR(4)
		DECLARE @week  VARCHAR(4)
		DECLARE @year  VARCHAR(4)
		DECLARE @var1 INT
		DECLARE @var2 INT
		DECLARE @var3 INT
		
		--SET @locationCode = 'ID22'
		--SET @unitCode = '2024'
		--SET @shift = '1'
		--SET @Date = '2016-09-14'
		--SET @brandGroupCode = 'DSS12SR-20'
		--SET @RoleId = 14

		SET @day = (SELECT DATEPART(DD, @Date));
		SET @week = (SELECT DATEPART( WK, @week)-1);
		SET @year = (SELECT DATEPART( YY, @year));

		-- GET DETAIL BRANDCODE
		INSERT INTO @TempTable1 SELECT DISTINCT BrandCode FROM WagesProductionCardApprovalDetailView 
		WHERE LocationCode = @locationCode
		AND UnitCode = @unitCode
		AND ProductionDate = @Date

		-- GET DETAIL GROUP
		INSERT INTO @TempTable2 SELECT DISTINCT GroupCode FROM WagesProductionCardApprovalDetailViewGroup 
		WHERE LocationCode = @locationCode
		AND UnitCode = @unitCode
		AND ProductionDate = @Date
		AND BrandGroupCode = @brandGroupCode

		IF(@brandCode = @brandGroupCode) -- first condition
		BEGIN
			DECLARE MY_CURSOR1 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
			SELECT brandCodeTemp FROM @TempTable1
			OPEN MY_CURSOR1
			FETCH NEXT FROM MY_CURSOR1 INTO @brandCodeTemp
			WHILE @@FETCH_STATUS = 0 BEGIN 
			
				DECLARE MY_CURSOR2 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
				SELECT groupCodeTemp FROM @TempTable2
				OPEN MY_CURSOR2
				FETCH NEXT FROM MY_CURSOR2 INTO @groupCodeTemp
				WHILE @@FETCH_STATUS = 0 BEGIN 
					-- GENERATE transactionCode
					SET @TransactionCode = 'WPC'+'/'+@locationCode+'/'+@shift+'/'+@unitCode+'/'+@groupCodeTemp+'/'+@brandCodeTemp+'/'+@year+'/'+@week+'/'+@day+'/%';
					
					INSERT INTO @TempTable3 (ApproveBtn,ReturnBtn,CompleteBtn)
					VALUES(
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Approve')),
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Return')),
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Complete'))
					)
				FETCH NEXT FROM MY_CURSOR2 INTO @groupCodeTemp
				END
				CLOSE MY_CURSOR2
				DEALLOCATE MY_CURSOR2

			FETCH NEXT FROM MY_CURSOR1 INTO @brandCodeTemp
			END
			CLOSE MY_CURSOR1
			DEALLOCATE MY_CURSOR1
		END
		ELSE -- second condition
		BEGIN
			DECLARE MY_CURSOR2 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
			SELECT groupCodeTemp FROM @TempTable2
			OPEN MY_CURSOR2
			FETCH NEXT FROM MY_CURSOR2 INTO @groupCodeTemp
			WHILE @@FETCH_STATUS = 0 BEGIN 
				-- GENERATE transactionCode
				SET @TransactionCode = 'WPC'+'/'+@locationCode+'/'+@shift+'/'+@unitCode+'/'+@groupCodeTemp+'/'+@brandCode+'/'+@year+'/'+@week+'/'+@day+'/%';

				INSERT INTO @TempTable3 (ApproveBtn,ReturnBtn,CompleteBtn)
					VALUES(
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Approve')),
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Return')),
						(SELECT * FROM RoleButtonChecker(@TransactionCode,@RoleId,'ProductionCardApprovalDetail','Complete'))
					)

			FETCH NEXT FROM MY_CURSOR2 INTO @groupCodeTemp
			END
			CLOSE MY_CURSOR2
			DEALLOCATE MY_CURSOR2
		END

		SELECT @var1 = ApproveBtn * COALESCE(@var1, 1) FROM @TempTable3
		SELECT @var2 = ReturnBtn * COALESCE(@var2, 1) FROM @TempTable3
		SELECT @var3 = CompleteBtn * COALESCE(@var3, 1) FROM @TempTable3

		INSERT INTO @TempTable4 (Result1,Result2,Result3) values ((SELECT @var1),(SELECT @var2),(SELECT @var3))
		
		SELECT Result1,Result2,Result3 FROM @TempTable4
		
    END;
