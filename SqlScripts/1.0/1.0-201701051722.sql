IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetProductionCardApprovalList]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetProductionCardApprovalList]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- begin : old code
--ALTER FUNCTION [dbo].[GetProductionCardApprovalList]
--(	
--	@ParamCurrentUser as varchar(50),
--	@ParamStartDate as DATETIME,
--	@ParamTransactionStatus as varchar(50),
--	@ParamIDResponsibility INT
--)
--RETURNS @WagesApprovalList TABLE (LocationCode VARCHAR(10), UnitCode VARCHAR(10), BrandCode VARCHAR(11), ProductionDate DATETIME,Shift INT,Status VARCHAR(20),IDRole INT,RolesName VARCHAR(100),RevisionType INT)
--AS
--BEGIN
--	--SET @ParamCurrentUser = 'PMI\bkristom';
--	--SET @ParamStartDate = '2016-11-16';
--	--SET @ParamTransactionStatus = 'inprogress';
--	--SET @ParamIDResponsibility = 331;
	
--	-- author : hakim
--	-- ticket : http://tp.voxteneo.co.id/entity/10459
--	-- desc : create function approval list
--	-- date : 2016-11-18 1518

--	-- author : hakim
--	-- ticket : http://tp.voxteneo.co.id/entity/10459
--	-- desc : cek filter using if exists
--	-- date : 2016-11-21 1315
	
--	-- author : hakim
--	-- ticket : http://tp.voxteneo.co.id/entity/10459
--	-- desc : fix several bugs
--	-- date : 2016-11-21 1851
	
--	DECLARE @CurrentUser varchar(50);
--	DECLARE @StartDate DATETIME;
--	DECLARE @TransactionStatus varchar(50);
--	DECLARE @IDResponsibility INT;

--	SET @CurrentUser = @ParamCurrentUser;
--	SET @StartDate = @ParamStartDate;
--	SET @TransactionStatus = @ParamTransactionStatus;
--	SET @IDResponsibility = @ParamIDResponsibility;

--	DECLARE @TempTable1 TABLE (LocationCode VARCHAR(8), UnitCode VARCHAR(8),BrandGroupCode VARCHAR(20));
--	DECLARE @DAY VARCHAR(2);
--	DECLARE @WEEK VARCHAR(2);
--	DECLARE @YEAR VARCHAR(4);

--	SET @DAY = (SELECT Case (SELECT DATEPART( DW, @StartDate)-1) WHEN 0 THEN 7 ELSE (SELECT DATEPART( DW, @StartDate)-1) END)
--	SET @WEEK = (SELECT [Week] FROM [dbo].[MstGenWeek] WHERE StartDate <= @StartDate AND EndDate >= @StartDate ) -- (CONVERT(VARCHAR(2), @StartDate, 105));
--	SET @YEAR = (CONVERT(varchar(4),(SELECT DATEPART( YY, @StartDate))));
--	--select @WEEK,@DAY
--	INSERT INTO @TempTable1
--	SELECT d.Location, d.Unit,'' FROM [dbo].[UtilUsersResponsibility] a
--	INNER JOIN  UtilResponsibilityRules b
--	ON a.IDResponsibility=b.IDResponsibility
--	INNER JOIN UtilResponsibility c
--	ON b.IDResponsibility = c.IDResponsibility
--	INNER JOIN UtilRules d
--	ON b.IDRule=d.IDRule
--	WHERE a.UserAD = @CurrentUser
--	and a.IDResponsibility = @IDResponsibility
--	--select * from @TempTable1
--	--get user locationcode
--	DECLARE @LocationCode VARCHAR(8);
--	DECLARE @UnitCode VARCHAR(8);
--	DECLARE @MarkUnitCode BIT;
--	DECLARE @idNextRole21 INT;
--	DECLARE @idNextRole22 INT;
--	DECLARE @idNextRole25 INT;
--	DECLARE @idNextRole56 INT;

--	SET @idNextRole21 = 0;
--	SET @idNextRole22 = 0;
--	SET @idNextRole25 = 0;
--	SET @idNextRole56 = 0;

--	SELECT @LocationCode =(SELECT TOP 1 LocationCode FROM @TempTable1);
--	SELECT @UnitCode = ISNULL((SELECT TOP 1 UnitCode FROM @TempTable1),0);
--	SELECT @MarkUnitCode = (SELECT ISNULL(@UnitCode,0));

--	IF (@MarkUnitCode = 0)
--	BEGIN
--		SELECT @idNextRole21 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21);
--		SELECT @idNextRole22 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22);
--		SELECT @idNextRole25 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 25);
--		SELECT @idNextRole56 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 56);
--	END

--	--get valid userlocationcode between ID2. and SKT(all)
--	IF(@LocationCode != 'SKT' AND @UnitCode > 0)
--	BEGIN
--		DELETE FROM @TempTable1
--		INSERT INTO @TempTable1
--		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode FROM UtilTransactionLogs WHERE IDFlow = 17
--		AND TransactionCode LIKE 'EBL/'+@LocationCode+'/1/'+@UnitCode+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
--	END
--	ELSE IF(@LocationCode != 'SKT' AND @UnitCode = 0)
--	BEGIN
--		DELETE FROM @TempTable1
--		INSERT INTO @TempTable1
--		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode FROM UtilTransactionLogs WHERE IDFlow = 17
--		AND TransactionCode LIKE 'EBL/'+@LocationCode+'/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
--	END
--	ELSE IF(@LocationCode = 'SKT')
--	BEGIN
--		DELETE FROM @TempTable1
--		INSERT INTO @TempTable1
--		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode FROM UtilTransactionLogs WHERE IDFlow = 17
--		AND TransactionCode LIKE 'EBL/%/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
--	END;
--	--select * from @TempTable1
--	DECLARE @LocationCodeCursor VARCHAR(8);
--	DECLARE @UnitCodeCursor VARCHAR(8);
--	DECLARE @BrandGroupCodeCursor VARCHAR(20);
--	DECLARE @totalEntryVerification INT;
--	DECLARE @totalTransactionlogSubmit INT;
--	DECLARE @totalTransactionlogCancelSubmit INT;
--	DECLARE @TempTableEntryVerification TABLE (LocationCode VARCHAR(4), UnitCode VARCHAR(4), BrandCode VARCHAR(20), ProductionDate DATETIME, [Shift] INT);
--	DECLARE @TempTableTotalTransactionlogSubmit TABLE (Plant VARCHAR(4), Unit VARCHAR(4), BrandGroupCode VARCHAR(20), Total INT);
--	DECLARE @TempTableTotalTransactionlogCancelSubmit TABLE (Plant VARCHAR(4), Unit VARCHAR(4), BrandGroupCode VARCHAR(20), Total INT);
--	DECLARE @maxRevisionType INT;
--	DECLARE @status VARCHAR(20);
--    DECLARE @nextRole VARCHAR(256);
--    DECLARE @idNextRole INT;
--	DECLARE @validation BIT;
--	DECLARE @passingValue BIT;
--	DECLARE @LastTransLogs INT;
--	DECLARE @Mark INT;
	

--	INSERT INTO @TempTableEntryVerification
--	SELECT LocationCode, UnitCode, BrandCode, ProductionDate, Shift FROM ExePlantProductionEntryVerificationView
--	WHERE ProductionDate = @StartDate
--	AND SUBSTRING(GroupCode,2,1) != '5';

--	-- BEGIN : additional logic for BrandGroup and BrandGroupCode
--		INSERT INTO @TempTableTotalTransactionlogSubmit
--		SELECT substring(x.TransactionCode,5,4) plant
--		,SUBSTRING(x.TransactionCode,12,4) unit
--		,SUBSTRING(x.TransactionCode,22,11) Brandgrupcode
--		,COUNT(*) HRASubmit
--		from (
--				SELECT [TransactionCode]
--				,max([IDFlow]) IDFlow
--				FROM [UtilTransactionLogs] a
--				where TransactionCode like 'WPC/%/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'
--				and a.CreatedDate=(select MAX(CreatedDate) from
--				[dbo].[UtilTransactionLogs]
--				where TransactionCode=a.TransactionCode)
--				group by a.TransactionCode
--			) x
--		where x.IDFlow in (69,23,22,24)
--		group by substring(x.TransactionCode,5,4),
--		SUBSTRING(x.TransactionCode,12,4) ,
--		SUBSTRING(x.TransactionCode,22,11)
--		ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;

--		-- END : additional logic for BrandGroup and BrandGroupCode
--		--select * from @TempTableTotalTransactionlogSubmit
--		--select * from @TempTableTotalTransactionlogCancelSubmit
--	DECLARE Cursor_ValidLocationUnitCode CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
--	SELECT LocationCode, UnitCode, BrandGroupCode FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode ASC
--	OPEN Cursor_ValidLocationUnitCode
--	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor
--	WHILE @@FETCH_STATUS = 0 BEGIN
--		SELECT @passingValue = ISNULL(@UnitCodeCursor,0);
--		IF (@passingValue != 0)
--		BEGIN

--			SELECT @totalEntryVerification = (SELECT COUNT(*) FROM @TempTableEntryVerification
--			WHERE LocationCode = @LocationCodeCursor
--			AND UnitCode = @UnitCodeCursor
--			AND BrandCode = @BrandGroupCodeCursor);

--			SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
--			WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor),0);

--			SELECT @totalTransactionlogCancelSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogCancelSubmit
--			WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor),0);

--			SELECT @Mark = ISNULL(@totalTransactionlogSubmit,0);
			
--			SELECT @LastTransLogs = (SELECT TOP 1 IDFlow FROM UtilTransactionLogs
--			WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%' ORDER BY UpdatedDate DESC);

--			IF(@totalEntryVerification - @totalTransactionlogSubmit = 0)
--			BEGIN
--				SET @status = 'SUBMITTED';
--				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole22 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22) END;
--			END
--			ELSE IF(@totalEntryVerification - @totalTransactionlogSubmit > 0)
--			BEGIN
--				SET @status = 'DRAFT';
--				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole21 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21) END;
--			END
			
--			IF(@LastTransLogs = 56)
--			BEGIN
--				SET @status = 'APPROVED';
--				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole56 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 56) END;
--			END

--			IF(@LastTransLogs = 25)
--			BEGIN
--				SET @status = 'APPROVED';
--				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole25 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 25) END;
--			END

--			IF((@LastTransLogs = 26) OR (@LastTransLogs = 70))
--			BEGIN
--				SET @status = 'COMPLETED';
--				SET @idNextRole = 0;
--			END

--			SELECT @nextRole = ISNULL((SELECT DISTINCT TOP 1 RolesName FROM UserADResponsibilityRolesView
--			WHERE IDRole =  @idNextRole),'NULL');

--			IF(@nextRole != 'NULL')
--			BEGIN
--				SET @nextRole = @nextRole;
--			END
--			ELSE
--			BEGIN
--				SET @nextRole = NULL;
--			END
--			--select @TransactionStatus,@status
--			IF(@TransactionStatus = 'completed' and @status != 'COMPLETED')
--			BEGIN
--				--goto cont;
--				FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor;
--				CONTINUE;
--			END
--			ELSE IF(@TransactionStatus = 'inprogress' AND @status = 'COMPLETED')
--			BEGIN
--				--goto cont;
--				FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor;
--				CONTINUE;
--			END
--			ELSE
--			BEGIN
--				SELECT @maxRevisionType =  ISNULL((SELECT MAX(substring(TransactionCode,len(TransactionCode),1)) FROM UtilTransactionLogs WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'),0);
--				INSERT INTO @WagesApprovalList VALUES (
--				@LocationCodeCursor
--				,@UnitCodeCursor
--				,(SELECT BrandGroupCode FROM MstGenBrand WHERE BrandCode =@BrandGroupCodeCursor)
--				,(SELECT TOP 1 ProductionDate FROM @TempTableEntryVerification)
--				,(SELECT TOP 1 [Shift] FROM @TempTableEntryVerification)
--				,@status
--				,@idNextRole
--				,(CASE WHEN @status = 'COMPLETED' THEN null ELSE @nextRole END)
--				,@maxRevisionType
--				);
--			END
--		END
--	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor
--	END
--	CLOSE Cursor_ValidLocationUnitCode
--	DEALLOCATE Cursor_ValidLocationUnitCode
--	--SELECT * FROM @WagesApprovalList
--	RETURN;

--END;
-- end : old code

-- begin : new code
CREATE FUNCTION [dbo].[GetProductionCardApprovalList]
(	
	@ParamCurrentUser as varchar(50),
	@ParamStartDate as DATETIME,
	@ParamTransactionStatus as varchar(50),
	@ParamIDResponsibility INT
)
RETURNS @WagesApprovalList TABLE (LocationCode VARCHAR(10), UnitCode VARCHAR(10), BrandCode VARCHAR(11), ProductionDate DATETIME,Shift INT,Status VARCHAR(20),IDRole INT,RolesName VARCHAR(100),RevisionType INT)
AS
BEGIN
	--SET @ParamCurrentUser = 'PMI\bkristom';
	--SET @ParamStartDate = '2016-11-15';
	--SET @ParamTransactionStatus = 'inprogress';
	--SET @ParamIDResponsibility = 331;
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : create function approval list
	-- date : 2016-11-18 1518

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : cek filter using if exists
	-- date : 2016-11-21 1315
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : fix several bugs
	-- date : 2016-11-21 1851
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : create function approval list
	-- date : 2016-11-18 1518

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : cek filter using if exists
	-- date : 2016-11-21 1315
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/10459
	-- desc : fix several bugs
	-- date : 2016-11-21 1851
	
	DECLARE @CurrentUser varchar(50);
	DECLARE @StartDate DATETIME;
	DECLARE @TransactionStatus varchar(50);
	DECLARE @IDResponsibility INT;

	SET @CurrentUser = @ParamCurrentUser;
	SET @StartDate = @ParamStartDate;
	SET @TransactionStatus = @ParamTransactionStatus;
	SET @IDResponsibility = @ParamIDResponsibility;

	DECLARE @TempTable1 TABLE (LocationCode VARCHAR(8), UnitCode VARCHAR(8),BrandGroupCode VARCHAR(20),KpsWeek VARCHAR(2),KpsYear VARCHAR(4),KpsDay VARCHAR(4));
	DECLARE @DAY VARCHAR(2);
	DECLARE @WEEK VARCHAR(2);
	DECLARE @YEAR VARCHAR(4);
	DECLARE @RevWeek VARCHAR(2);
	DECLARE @RevYear VARCHAR(4);

	DECLARE @LocationCodeCursor VARCHAR(8);
	DECLARE @UnitCodeCursor VARCHAR(8);
	DECLARE @BrandGroupCodeCursor VARCHAR(20);
	DECLARE @KpsWeekCursor VARCHAR(2);
	DECLARE @KpsYearCursor VARCHAR(4);
	DECLARE @KpsDayCursor VARCHAR(4);
	DECLARE @totalEntryVerification INT;
	DECLARE @totalTransactionlogSubmit INT;
	DECLARE @totalTransactionlogCancelSubmit INT;
	DECLARE @TempTableEntryVerification TABLE (ProductionEntryCode VARCHAR(128), LocationCode VARCHAR(4), UnitCode VARCHAR(4), BrandCode VARCHAR(20), ProductionDate DATETIME, [Shift] INT, KpsWeek VARCHAR(2), KpsYear VARCHAR(4));
	DECLARE @TempTableTotalTransactionlogSubmit TABLE (Plant VARCHAR(4), Unit VARCHAR(4), BrandGroupCode VARCHAR(20), Total INT, revisiontype int,KpsWeek VARCHAR(2),KpsYear VARCHAR(4),KpsDay VARCHAR(4));
	DECLARE @TempTableTotalTransactionlogCancelSubmit TABLE (Plant VARCHAR(4), Unit VARCHAR(4), BrandGroupCode VARCHAR(20), Total INT);
	DECLARE @maxRevisionType INT;
	DECLARE @status VARCHAR(20);
    DECLARE @nextRole VARCHAR(256);
    DECLARE @idNextRole INT;
	DECLARE @validation BIT;
	DECLARE @passingValue BIT;
	DECLARE @LastTransLogs INT;
	DECLARE @Mark INT;
	DECLARE @LastClosingPayrollDate DATETIME;
	DECLARE @revType varchar(1);
	DECLARE @date1 datetime
	DECLARE @date2 datetime

	SET @DAY = (SELECT Case (SELECT DATEPART( DW, @StartDate)-1) WHEN 0 THEN 7 ELSE (SELECT DATEPART( DW, @StartDate)-1) END)
	SET @WEEK = (SELECT [Week] FROM [dbo].[MstGenWeek] WHERE StartDate <= @StartDate AND EndDate >= @StartDate ) -- (CONVERT(VARCHAR(2), @StartDate, 105));
	SET @YEAR = (CONVERT(varchar(4),(SELECT DATEPART( YY, @StartDate))));

	INSERT INTO @TempTable1
	SELECT d.Location, d.Unit,'','','','' FROM [dbo].[UtilUsersResponsibility] a
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
	DECLARE @MarkUnitCode BIT;
	DECLARE @idNextRole21 INT;
	DECLARE @idNextRole22 INT;
	DECLARE @idNextRole25 INT;
	DECLARE @idNextRole56 INT;

	SET @idNextRole21 = 0;
	SET @idNextRole22 = 0;
	SET @idNextRole25 = 0;
	SET @idNextRole56 = 0;

	SELECT @LocationCode =(SELECT TOP 1 LocationCode FROM @TempTable1);
	SELECT @UnitCode = ISNULL((SELECT TOP 1 UnitCode FROM @TempTable1),0);
	SELECT @MarkUnitCode = (SELECT ISNULL(@UnitCode,0));

	IF (@MarkUnitCode = 0)
	BEGIN
		SELECT @idNextRole21 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21);
		SELECT @idNextRole22 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22);
		SELECT @idNextRole25 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 25);
		SELECT @idNextRole56 = (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 56);
	END

	SELECT @RevWeek = (select Week from MstGenWeek where (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc) between StartDate and EndDate)
	SELECT @RevYear = (select Year from MstGenWeek where (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc) between StartDate and EndDate)

	--get valid userlocationcode between ID2. and SKT(all)
	IF(@LocationCode != 'SKT' AND @UnitCode > 0)
	BEGIN
		DELETE FROM @TempTable1
		-- revision logic
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE TransactionCode LIKE 'EBL/'+@LocationCode+'/1/'+@UnitCode+'/%/%/'+@RevYear+'/'+@RevWeek+'/%' AND CONVERT(DATE,TransactionDate) = @StartDate
		-- revision logic

		
		DECLARE Cursor_1 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
		SELECT LocationCode, UnitCode, BrandGroupCode,KpsWeek, KpsYear, KpsDay FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode,KpsWeek , KpsYear ASC
		OPEN Cursor_1
		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		WHILE @@FETCH_STATUS = 0 BEGIN
			select @date1 = (select DateADD(dd,@KpsDayCursor-1,StartDate) FROM MstGenWeek WHERE YEAR = @KpsYearCursor AND Week =@KpsWeekCursor )
			select @date2 = (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc)
			IF(@date1 > @date2)
			BEGIN
				DELETE FROM @TempTable1 WHERE LocationCode = @LocationCodeCursor and UnitCode = @UnitCodeCursor and BrandGroupCode = @BrandGroupCodeCursor and KpsYear = @KpsYearCursor and KpsWeek = @KpsWeekCursor and KpsDay = @KpsDayCursor
			END

		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		END
		CLOSE Cursor_1
		DEALLOCATE Cursor_1

		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE 'EBL/'+@LocationCode+'/1/'+@UnitCode+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
	END
	ELSE IF(@LocationCode != 'SKT' AND @UnitCode = 0)
	BEGIN
		DELETE FROM @TempTable1
		-- revision logic
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE TransactionCode LIKE 'EBL/'+@LocationCode+'/1/%/%/%/'+@RevYear+'/'+@RevWeek+'/%' AND CONVERT(DATE,TransactionDate) = @StartDate
		-- revision logic

		DECLARE Cursor_1 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
		SELECT LocationCode, UnitCode, BrandGroupCode,KpsWeek, KpsYear, KpsDay FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode,KpsWeek , KpsYear ASC
		OPEN Cursor_1
		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		WHILE @@FETCH_STATUS = 0 BEGIN
			select @date1 = (select DateADD(dd,@KpsDayCursor-1,StartDate) FROM MstGenWeek WHERE YEAR = @KpsYearCursor AND Week =@KpsWeekCursor )
			select @date2 = (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc)
			IF(@date1 > @date2)
			BEGIN
				DELETE FROM @TempTable1 WHERE LocationCode = @LocationCodeCursor and UnitCode = @UnitCodeCursor and BrandGroupCode = @BrandGroupCodeCursor and KpsYear = @KpsYearCursor and KpsWeek = @KpsWeekCursor and KpsDay = @KpsDayCursor
			END

		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		END
		CLOSE Cursor_1
		DEALLOCATE Cursor_1

		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE 'EBL/'+@LocationCode+'/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
	END
	ELSE IF(@LocationCode = 'SKT')
	BEGIN

		declare @rDay VARCHAR(1);

		DELETE FROM @TempTable1
		-- revision logic
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE TransactionCode LIKE 'EBL/%/1/%/%/%/'+@RevYear+'/'+@RevWeek+'/%' AND IDFlow = 17 AND CONVERT(DATE,TransactionDate) = @StartDate 
		
		DECLARE Cursor_1 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
		SELECT LocationCode, UnitCode, BrandGroupCode,KpsWeek, KpsYear, KpsDay FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode,KpsWeek , KpsYear ASC
		OPEN Cursor_1
		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		WHILE @@FETCH_STATUS = 0 BEGIN
			select @date1 = (select DateADD(dd,@KpsDayCursor-1,StartDate) FROM MstGenWeek WHERE YEAR = @KpsYearCursor AND Week =@KpsWeekCursor )
			select @date2 = (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc)
			
			IF(@date1 > @date2)
			BEGIN
				DELETE FROM @TempTable1 WHERE LocationCode = @LocationCodeCursor and UnitCode = @UnitCodeCursor and BrandGroupCode = @BrandGroupCodeCursor and KpsYear = @KpsYearCursor and KpsWeek = @KpsWeekCursor and KpsDay = @KpsDayCursor
			END

		FETCH NEXT FROM Cursor_1 INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		END
		CLOSE Cursor_1
		DEALLOCATE Cursor_1
		
		INSERT INTO @TempTable1
		SELECT DISTINCT SUBSTRING(TransactionCode,5,4) AS Location, SUBSTRING(TransactionCode,12,4) AS UnitCode, SUBSTRING(TransactionCode,22,11) AS BrandGroupCode 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END as KpsDay
		FROM UtilTransactionLogs WHERE IDFlow = 17
		AND TransactionCode LIKE 'EBL/%/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'%'
	END;

	--select * from @TempTable1

	-- BEGIN : additional logic for BrandGroup and BrandGroupCode
		INSERT INTO @TempTableTotalTransactionlogSubmit
		SELECT substring(x.TransactionCode,5,4) plant
		,SUBSTRING(x.TransactionCode,12,4) unit
		,SUBSTRING(x.TransactionCode,22,11) Brandgrupcode
		,COUNT(*) HRASubmit
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN SUBSTRING(x.TransactionCode,43,1) ELSE SUBSTRING(x.TransactionCode,44,1) END as revisiontype
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(x.TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,42,1)) END as KpsDay
		from (
				SELECT [TransactionCode]
				,max([IDFlow]) IDFlow
				FROM [UtilTransactionLogs] a
				where TransactionCode like 'WPC/%/1/%/%/%/'+@RevYear+'/'+@RevWeek+'/%/1'
				and a.CreatedDate=(select MAX(CreatedDate) from
				[dbo].[UtilTransactionLogs]
				where TransactionCode=a.TransactionCode)
				group by a.TransactionCode
			) x
		where x.IDFlow in (69,23,22,24)
		--and SUBSTRING(x.TransactionCode,34,9) like '%2016/?31/%'
		group by substring(x.TransactionCode,5,4)
		,SUBSTRING(x.TransactionCode,12,4) 
		,SUBSTRING(x.TransactionCode,22,11)
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN SUBSTRING(x.TransactionCode,43,1) ELSE SUBSTRING(x.TransactionCode,44,1) END
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END
		,SUBSTRING(TransactionCode,34,4) 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END
		ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;

		INSERT INTO @TempTableTotalTransactionlogSubmit
		SELECT substring(x.TransactionCode,5,4) plant
		,SUBSTRING(x.TransactionCode,12,4) unit
		,SUBSTRING(x.TransactionCode,22,11) Brandgrupcode
		,COUNT(*) HRASubmit
		,SUBSTRING(x.TransactionCode,44,1) as revisiontype
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,39,2)) END as KpsWeek 
		,SUBSTRING(x.TransactionCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,42,1)) END as KpsDay
		from (
				SELECT [TransactionCode]
				,max([IDFlow]) IDFlow
				FROM [UtilTransactionLogs] a
				where TransactionCode like 'WPC/%/1/%/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/0'
				and a.CreatedDate=(select MAX(CreatedDate) from
				[dbo].[UtilTransactionLogs]
				where TransactionCode=a.TransactionCode)
				group by a.TransactionCode
			) x
		where x.IDFlow in (69,23,22,24)
		--and SUBSTRING(x.TransactionCode,34,9) like '%2016/?31/%'
		group by substring(x.TransactionCode,5,4),
		SUBSTRING(x.TransactionCode,12,4) ,
		SUBSTRING(x.TransactionCode,22,11),SUBSTRING(x.TransactionCode,44,1)
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END
		,SUBSTRING(TransactionCode,34,4) 
		,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END
		ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;

		--select * from @TempTableTotalTransactionlogSubmit

		-- END : additional logic for BrandGroup and BrandGroupCode
		DECLARE @PRODUCTIONDATE DATE
		DECLARE @SHIFT INT
	DECLARE Cursor_ValidLocationUnitCode CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
	SELECT LocationCode, UnitCode, BrandGroupCode, KpsWeek, KpsYear, KpsDay FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode,KpsWeek , KpsYear ASC
	OPEN Cursor_ValidLocationUnitCode
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
	WHILE @@FETCH_STATUS = 0 BEGIN
		SELECT @passingValue = ISNULL(@UnitCodeCursor,0);
		IF (@passingValue != 0)
		BEGIN
		--select @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor
		SELECT @totalEntryVerification = (SELECT COUNT(*) FROM ExePlantProductionEntryVerificationView
		WHERE (convert(date,UpdatedDate) = @StartDate or ((convert(date,ProductionDate) = @StartDate) and (ProductionDate <= (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc))))
		AND SUBSTRING(GroupCode,2,1) != '5'
		AND ProductionEntryCode like 'EBL/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor);

		SELECT @PRODUCTIONDATE = (SELECT DISTINCT ProductionDate FROM ExePlantProductionEntryVerificationView
		WHERE (convert(date,UpdatedDate) = @StartDate or convert(date,ProductionDate) = @StartDate)
		AND SUBSTRING(GroupCode,2,1) != '5'
		AND ProductionEntryCode like 'EBL/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor);

		SELECT @SHIFT = (SELECT DISTINCT Shift FROM ExePlantProductionEntryVerificationView
		WHERE (convert(date,UpdatedDate) = @StartDate or convert(date,ProductionDate) = @StartDate)
		AND SUBSTRING(GroupCode,2,1) != '5'
		AND ProductionEntryCode like 'EBL/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor);
		
		IF ((SELECT TOP 1 revisiontype FROM @TempTableTotalTransactionlogSubmit
		WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor AND KpsYear = @KpsYearCursor AND KpsWeek = @KpsWeekCursor AND KpsDay = @KpsDayCursor) != 0)
		BEGIN
			SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
			WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor AND revisiontype = 1),0);
		END
		ELSE
		BEGIN
			SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
			WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor AND revisiontype = 0),0);
		END 

		SELECT @Mark = ISNULL(@totalTransactionlogSubmit,0);
			
			SELECT @revType = (select max(CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN SUBSTRING(TransactionCode,43,1) ELSE SUBSTRING(TransactionCode,44,1) END) FROM UtilTransactionLogs
			WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/%'
			AND convert(date,TransactionDate) = @StartDate
			);

			IF(ISNULL(@revType,0) = 1)
			BEGIN
				SELECT @LastTransLogs = (SELECT TOP 1 IDFlow FROM UtilTransactionLogs 
				WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/1'
				ORDER BY UpdatedDate DESC);
			END
			ELSE
			BEGIN
				SELECT @LastTransLogs = (SELECT TOP 1 IDFlow FROM UtilTransactionLogs 
				WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/0'
				ORDER BY UpdatedDate DESC);
			END
			
			IF(@totalEntryVerification - @totalTransactionlogSubmit = 0)
			BEGIN
				SET @status = 'SUBMITTED';
				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole22 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22) END;
			END
			ELSE IF(@totalEntryVerification - @totalTransactionlogSubmit > 0)
			BEGIN
				SET @status = 'DRAFT';
				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole21 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21) END;
			END
			
			IF(@LastTransLogs = 56)
			BEGIN
				SET @status = 'APPROVED';
				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole56 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 56) END;
			END

			IF(@LastTransLogs = 25)
			BEGIN
				SET @status = 'APPROVED';
				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole25 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 25) END;
			END

			IF((@LastTransLogs = 26) OR (@LastTransLogs = 70))
			BEGIN
				SET @status = 'COMPLETED';
				SET @idNextRole = 0;
			END

		SELECT @nextRole = ISNULL((SELECT DISTINCT TOP 1 RolesName FROM UserADResponsibilityRolesView
		WHERE IDRole =  @idNextRole),'NULL');

		IF(@nextRole != 'NULL')
		BEGIN
			SET @nextRole = @nextRole;
		END
		ELSE
		BEGIN
			SET @nextRole = NULL;
		END
		
		IF(@TransactionStatus = 'completed' and @status != 'COMPLETED')
		BEGIN
			FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor;
			CONTINUE;
		END
		ELSE IF(@TransactionStatus = 'inprogress' AND @status = 'COMPLETED')
		BEGIN
			FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor;
			CONTINUE;
		END
		ELSE
		BEGIN
			--DECLARE @Filter BIT;
			--SELECT @Filter = ISNULL((SELECT COUNT(ProductionDate) FROM @TempTableEntryVerificationGroup),0)
			--IF (@Filter != 0)
			--BEGIN
				SELECT @maxRevisionType =  ISNULL((SELECT MAX(substring(TransactionCode,len(TransactionCode),1)) FROM UtilTransactionLogs WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/1/'+@UnitCodeCursor+'/%/%/'+@YEAR+'/'+@WEEK+'/'+@DAY+'/%'),0);
				INSERT INTO @WagesApprovalList VALUES (
				@LocationCodeCursor
				,@UnitCodeCursor
				,(SELECT BrandGroupCode FROM MstGenBrand WHERE BrandCode = @BrandGroupCodeCursor)
				,@PRODUCTIONDATE,@SHIFT
				,@status
				,@idNextRole
				,(CASE WHEN @status = 'COMPLETED' THEN null ELSE @nextRole END)
				--,@maxRevisionType
				,ISNULL((SELECT TOP 1 revisiontype FROM @TempTableTotalTransactionlogSubmit
				WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor AND KpsYear = @KpsYearCursor AND KpsWeek = @KpsWeekCursor AND KpsDay = @KpsDayCursor),0)
				);
			--END
		END
		END
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor
	END
	CLOSE Cursor_ValidLocationUnitCode
	DEALLOCATE Cursor_ValidLocationUnitCode
	--SELECT * FROM @WagesApprovalList
	RETURN;

END;
-- end : new code