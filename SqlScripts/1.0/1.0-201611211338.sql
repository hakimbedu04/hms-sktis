IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetProductionCardApprovalList]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetProductionCardApprovalList]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetProductionCardApprovalList]
(	
	@CurrentUser as varchar(50),
	@StartDate as DATETIME,
	@TransactionStatus as varchar(50),
	@IDResponsibility INT
)
RETURNS @WagesApprovalList TABLE (LocationCode VARCHAR(10), UnitCode VARCHAR(10), BrandCode VARCHAR(11), ProductionDate DATETIME,Shift INT,Status VARCHAR(20),IDRole INT,RolesName VARCHAR(100),RevisionType INT)
AS
BEGIN
	--SET @CurrentUser = 'PMI\bkristom';
	--SET @StartDate = '2016-11-09';
	--SET @TransactionStatus = 'inprogress';
	--SET @IDResponsibility = 331;
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : create function approval list
	-- date : 2016-11-18 1518

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : cek filter using if exists
	-- date : 2016-11-21 1315
	
	DECLARE @TempTable1 TABLE (LocationCode VARCHAR(8), UnitCode VARCHAR(8));
	DECLARE @DAY VARCHAR(2);
	DECLARE @WEEK VARCHAR(2);
	DECLARE @YEAR VARCHAR(4);

	SET @DAY = (SELECT Case (SELECT DATEPART( DW, @StartDate)-1) WHEN 0 THEN 7 ELSE (SELECT DATEPART( DW, @StartDate)-1) END)
	SET @WEEK = (SELECT [Week] FROM [dbo].[MstGenWeek] WHERE StartDate <= @StartDate AND EndDate >= @StartDate ) -- (CONVERT(VARCHAR(2), @StartDate, 105));
	SET @YEAR = (CONVERT(varchar(4),(SELECT DATEPART( YY, @StartDate))));

	INSERT INTO @TempTable1
	SELECT d.Location, d.Unit FROM [dbo].[UtilUsersResponsibility] a
	INNER JOIN  UtilResponsibilityRules b
	ON a.IDResponsibility=b.IDResponsibility
	INNER JOIN UtilResponsibility c
	ON b.IDResponsibility = c.IDResponsibility
	INNER JOIN UtilRules d
	ON b.IDRule=d.IDRule
	WHERE a.UserAD = @CurrentUser
	and a.IDResponsibility = @IDResponsibility
	
	--get user locationcode
	DECLARE @LocationCode VARCHAR(8);
	DECLARE @UnitCode VARCHAR(8);
	SELECT @LocationCode =(SELECT TOP 1 LocationCode FROM @TempTable1);
	SELECT @UnitCode = ISNULL((SELECT TOP 1 UnitCode FROM @TempTable1),0);
	
	--get valid userlocationcode between ID2. and SKT(all)
	IF(@LocationCode != 'SKT' AND @UnitCode > 0)
	BEGIN
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE '%EBL/'+@LocationCode+'/1/'+@UnitCode+'/2016/45/2%'
	END
	ELSE IF(@LocationCode != 'SKT' AND @UnitCode = 0)
	BEGIN
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE '%EBL/'+@LocationCode+'/1/%/2016/45/2%'
	END
	ELSE IF(@LocationCode = 'SKT')
	BEGIN
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE '%EBL/%/1/%/2016/45/2%'
	END;

	DECLARE @LocationCodeCursor VARCHAR(8);
	DECLARE @UnitCodeCursor VARCHAR(8);
	DECLARE @totalEntryVerification INT;
	DECLARE @totalTransactionlogSubmit INT;
	DECLARE @totalTransactionlogCancelSubmit INT;
	DECLARE @TempTableEntryVerification TABLE (LocationCode VARCHAR(4), UnitCode VARCHAR(4), BrandCode VARCHAR(20), ProductionDate DATETIME, [Shift] INT);
	DECLARE @TempTableEntryVerificationGroup TABLE (LocationCode VARCHAR(4), UnitCode VARCHAR(4), BrandCode VARCHAR(20), ProductionDate DATETIME, [Shift] INT);
	DECLARE @TempTableLastTransactionLogs TABLE (IDFlow INT);
	DECLARE @TempTableGetAllTotalTransactionlog TABLE (TransactionCode VARCHAR(128), IDFlow INT);
	DECLARE @maxRevisionType INT;
	DECLARE @status VARCHAR(20);
    DECLARE @nextRole VARCHAR(256);
    DECLARE @idNextRole INT;
	DECLARE @userAdView VARCHAR(100);
	DECLARE @validation BIT;
	DECLARE @passingValue BIT;
	

	INSERT INTO @TempTableEntryVerification
	SELECT LocationCode, UnitCode, BrandCode, ProductionDate, Shift FROM ExePlantProductionEntryVerification
	WHERE ProductionDate = @StartDate
	AND SUBSTRING(GroupCode,2,1) != '5';

	INSERT INTO @TempTableGetAllTotalTransactionlog
	SELECT TransactionCode,IDFlow FROM UtilTransactionLogs WHERE TransactionCode LIKE '%WPC/%/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%';

	DECLARE Cursor_ValidLocationUnitCode CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
	SELECT LocationCode, UnitCode FROM @TempTable1
	OPEN Cursor_ValidLocationUnitCode
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor
	WHILE @@FETCH_STATUS = 0 BEGIN
		SELECT @passingValue = ISNULL(@UnitCodeCursor,0);
		IF (@passingValue != 0)
		BEGIN

		SELECT @totalEntryVerification = (SELECT COUNT(*) FROM @TempTableEntryVerification
		WHERE LocationCode = @LocationCodeCursor
		AND UnitCode = @UnitCodeCursor);

		SELECT @totalTransactionlogSubmit = (SELECT COUNT(*) FROM @TempTableGetAllTotalTransactionlog 
		WHERE IDFlow=22 AND TransactionCode LIKE '%WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%');

		SELECT @totalTransactionlogCancelSubmit = (SELECT COUNT(*) FROM @TempTableGetAllTotalTransactionlog 
		WHERE IDFlow=57 AND TransactionCode LIKE '%WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%');

		DELETE FROM @TempTableEntryVerificationGroup
		INSERT INTO @TempTableEntryVerificationGroup
		SELECT LocationCode,UnitCode,BrandCode,ProductionDate,[Shift]  FROM @TempTableEntryVerification
		WHERE ProductionDate = @StartDate
		AND LocationCode = @LocationCodeCursor
		AND UnitCode = @UnitCodeCursor
		GROUP BY LocationCode,UnitCode,BrandCode,ProductionDate,[Shift];

		IF(@totalTransactionlogSubmit > 0)
		BEGIN
			DELETE FROM @TempTableLastTransactionLogs
			INSERT INTO @TempTableLastTransactionLogs
			SELECT IDFlow FROM UtilTransactionLogs
			WHERE UpdatedDate = (SELECT MAX(UpdatedDate) FROM UtilTransactionLogs WHERE TransactionCode LIKE '%WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%');

			IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (21,22,23,69))
			BEGIN
				IF(@totalEntryVerification - (@totalTransactionlogSubmit - @totalTransactionlogCancelSubmit) = 0)
				BEGIN
					SET @status = 'SUBMITTED';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22);
				END
				ELSE IF(@totalEntryVerification - (@totalTransactionlogSubmit - @totalTransactionlogCancelSubmit) > 0)
				BEGIN
					SET @status = 'DRAFT';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21);
				END
			END
			ELSE
			BEGIN
				IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (56))
				BEGIN
					SET @status = 'APPROVED';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 56);
				END

				IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (25))
				BEGIN
					SET @status = 'APPROVED';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 25);
				END

				IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (26))
				BEGIN
					SET @status = 'COMPLETED';
					SET @idNextRole = 0;
				END

				IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (24))
				BEGIN
					SET @status = 'SUBMITTED';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 24);
				END

				IF EXISTS(SELECT IDFlow FROM @TempTableLastTransactionLogs WHERE IDFlow in (57))
				BEGIN
					SET @status = 'DRAFT';
					SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21);
				END
			END

		END
		ELSE
		BEGIN
			SET @status = 'DRAFT';
			SET @idNextRole = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21);
		END

		SELECT @userAdView = ISNULL((SELECT DISTINCT TOP 1 RolesName FROM UserADResponsibilityRolesView
		WHERE IDRole =  @idNextRole),'NULL');

		IF(@userAdView != 'NULL')
		BEGIN
			SET @nextRole = @userAdView;
		END
		ELSE
		BEGIN
			SET @nextRole = NULL;
		END

		IF(@TransactionStatus = 'completed' and @status != 'COMPLETED')
		BEGIN
			DELETE FROM @WagesApprovalList;
		END
		ELSE IF(@TransactionStatus = 'inprogress' AND @status = 'COMPLETED')
		BEGIN
			DELETE FROM @WagesApprovalList;
		END
		ELSE
		BEGIN
			DECLARE @Filter BIT;
			SELECT @Filter = ISNULL((SELECT COUNT(ProductionDate) FROM @TempTableEntryVerification),0)
			IF (@Filter != 0)
			BEGIN
				SELECT @maxRevisionType =  ISNULL((SELECT MAX(substring(TransactionCode,len(TransactionCode),1)) FROM UtilTransactionLogs WHERE TransactionCode LIKE '%WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'),0);
				INSERT INTO @WagesApprovalList VALUES (
				@LocationCodeCursor
				,@UnitCodeCursor
				,(SELECT BrandGroupCode FROM MstGenBrand WHERE BrandCode =( SELECT TOP 1 BrandCode FROM @TempTableEntryVerification))
				,(SELECT TOP 1 ProductionDate FROM @TempTableEntryVerification)
				,(SELECT TOP 1 [Shift] FROM @TempTableEntryVerification)
				,@status
				,@idNextRole
				,@nextRole
				,@maxRevisionType
				);
			END
		END
		END
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor
	END
	CLOSE Cursor_ValidLocationUnitCode
	DEALLOCATE Cursor_ValidLocationUnitCode
	--SELECT * FROM @WagesApprovalList
	RETURN;

END;
