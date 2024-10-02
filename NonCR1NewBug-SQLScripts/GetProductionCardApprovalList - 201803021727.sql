IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductionCardApprovalList]') AND type in (N'P', N'PC'))
DROP FUNCTION [dbo].[GetProductionCardApprovalList]
GO

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
	--SET @ParamCurrentUser = 'PMI\ssarinah1';
	--SET @ParamStartDate = '2017-01-05';
	--SET @ParamTransactionStatus = 'inprogress';
	--SET @ParamIDResponsibility = 190;

	--SET @ParamCurrentUser = 'PMI\bkristom';
	--SET @ParamStartDate = '2017-02-02';
	--SET @ParamTransactionStatus = 'completed';
	--SET @ParamIDResponsibility = 331;

	--SET @ParamCurrentUser = 'PMI\fristant';
	--SET @ParamStartDate = '2017-01-10';
	--SET @ParamTransactionStatus = 'inprogress';
	--SET @ParamIDResponsibility = 1331;
	
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

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/12904
	-- desc : change how to define revisiontype
	-- date : 2017-01-12 1153

	-- author : hakim
	-- ticket : hot fix
	-- desc : hide release data appear on another day
	-- date : 2017-02-03 1407
	
	DECLARE @CurrentUser varchar(50);
	DECLARE @StartDate DATETIME;
	DECLARE @TransactionStatus varchar(50);
	DECLARE @IDResponsibility INT;

	SET @CurrentUser = @ParamCurrentUser;
	SET @StartDate = @ParamStartDate;
	SET @TransactionStatus = @ParamTransactionStatus;
	SET @IDResponsibility = @ParamIDResponsibility;

	DECLARE @TempTable1 TABLE (LocationCode VARCHAR(8), UnitCode VARCHAR(8),BrandGroupCode VARCHAR(20),KpsWeek VARCHAR(2),KpsYear VARCHAR(4),KpsDay VARCHAR(4), RevisionType VARCHAR(1), Shift VARCHAR(20));
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
	DECLARE @RevisionTypeCursor VARCHAR(1);
	DECLARE @ShiftCursor VARCHAR(1);
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
	DECLARE @PRODUCTIONDATE DATE
	DECLARE @WagesApprovalListTemp TABLE (LocationCode VARCHAR(10), UnitCode VARCHAR(10), BrandCode VARCHAR(11), ProductionDate DATETIME,Shift INT,Status VARCHAR(20),IDRole INT,RolesName VARCHAR(100),RevisionType INT)

	SET @DAY = (SELECT Case (SELECT DATEPART( DW, @StartDate)-1) WHEN 0 THEN 7 ELSE (SELECT DATEPART( DW, @StartDate)-1) END)
	SET @WEEK = (SELECT [Week] FROM [dbo].[MstGenWeek] WHERE StartDate <= @StartDate AND EndDate >= @StartDate ) -- (CONVERT(VARCHAR(2), @StartDate, 105));
	SET @YEAR = (CONVERT(varchar(4),(SELECT DATEPART( YY, @StartDate))));

	INSERT INTO @TempTable1
	SELECT d.Location, d.Unit,'','','','','','' FROM [dbo].[UtilUsersResponsibility] a
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

	declare @payrollstartdate datetime, @payrollenddate datetime

	SET @payrollenddate = (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) < @StartDate order by ClosingDate desc)
	SET @payrollstartdate = (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) < @payrollenddate order by ClosingDate desc)

	SELECT @RevWeek = (select Week from MstGenWeek where (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc) between StartDate and EndDate)
	SELECT @RevYear = (select Year from MstGenWeek where (select top 1 ClosingDate from MstClosingPayroll where convert(date,ClosingDate) <= @StartDate order by ClosingDate desc) between StartDate and EndDate)

	--get valid userlocationcode between ID2. and SKT(all)
	IF(@LocationCode != 'SKT' AND @UnitCode > 0)
	BEGIN
		DELETE FROM @TempTable1
		INSERT INTO @TempTable1
		select distinct 
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where 
		LocationCode = @LocationCode and
		UnitCode = @UnitCode and
		(convert(date,ProductionDate) >= DATEADD(dd,1,@payrollstartdate) and convert(date,ProductionDate) <= @payrollenddate) and
		convert(date,UpdatedDate) = @ParamStartDate and
		RevisionType = 1

		INSERT INTO @TempTable1
		select distinct
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where 
		LocationCode = @LocationCode and
		UnitCode = @UnitCode and
		(convert(date,ProductionDate) = @StartDate) and
		RevisionType = 0
	END
	ELSE IF(@LocationCode != 'SKT' AND @UnitCode = 0)
	BEGIN
		DELETE FROM @TempTable1
		INSERT INTO @TempTable1
		select distinct 
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where 
		LocationCode = @LocationCode and
		(convert(date,ProductionDate) >= DATEADD(dd,1,@payrollstartdate) and convert(date,ProductionDate) <= @payrollenddate) and
		convert(date,UpdatedDate) = @ParamStartDate and
		RevisionType = 1

		INSERT INTO @TempTable1
		select distinct
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where 
		LocationCode = @LocationCode and
		(convert(date,ProductionDate) = @StartDate) and
		RevisionType = 0
	END
	ELSE IF(@LocationCode = 'SKT')
	BEGIN
		DELETE FROM @TempTable1
		INSERT INTO @TempTable1
		select distinct 
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where
		(convert(date,ProductionDate) >= DATEADD(dd,1,@payrollstartdate) and convert(date,ProductionDate) <= @payrollenddate) and
		convert(date,UpdatedDate) = @ParamStartDate and
		RevisionType = 1

		INSERT INTO @TempTable1
		select distinct
		SUBSTRING(ProductionCardCode,5,4) AS Location
		,SUBSTRING(ProductionCardCode,12,4) AS UnitCode
		,SUBSTRING(ProductionCardCode,22,11) AS BrandGroupCode
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,39,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,39,2)) END as KpsWeek 
		,SUBSTRING(ProductionCardCode,34,4) as KpsYear
		,CASE WHEN SUBSTRING(ProductionCardCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(ProductionCardCode,41,1)) ELSE CONVERT(INT,SUBSTRING(ProductionCardCode,42,1)) END as KpsDay
		,RevisionType
		,SUBSTRING(ProductionCardCode,10,1) AS Shift
		from ProductionCard 
		where
		(convert(date,ProductionDate) = @StartDate) and
		RevisionType = 0
	END;

	--select * from @TempTable1

	DECLARE Cursor_ValidLocationUnitCode CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
	SELECT LocationCode, UnitCode, BrandGroupCode, KpsWeek, KpsYear, KpsDay,RevisionType,Shift FROM @TempTable1 ORDER BY LocationCode, UnitCode, BrandGroupCode,KpsWeek , KpsYear ASC
	OPEN Cursor_ValidLocationUnitCode
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor, @KpsDayCursor, @RevisionTypeCursor,@ShiftCursor
	WHILE @@FETCH_STATUS = 0 BEGIN
		SELECT @passingValue = ISNULL(@UnitCodeCursor,0);
		IF (@passingValue != 0)
		BEGIN
			-- delete soon when revision ok
			--SET @status = 'DRAFT';
			--SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole21 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21) END;
			-- delete soon when revision ok
			--select 'WPC/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/'+@RevisionTypeCursor

			SELECT @LastTransLogs = ISNULL((SELECT TOP 1 IDFlow FROM UtilTransactionLogs 
			WHERE TransactionCode LIKE 'WPC/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/'+@RevisionTypeCursor
			ORDER BY UpdatedDate DESC),0);

			--select @LastTransLogs

			IF((@LastTransLogs = 69) OR (@LastTransLogs = 23) OR (@LastTransLogs = 22) OR (@LastTransLogs = 24) OR (@LastTransLogs = 0))
			BEGIN
				-- BEGIN : additional logic for BrandGroup and BrandGroupCode
				--DELETE FROM @TempTableTotalTransactionlogSubmit
				--INSERT INTO @TempTableTotalTransactionlogSubmit
				--SELECT substring(x.TransactionCode,5,4) plant
				--,SUBSTRING(x.TransactionCode,12,4) unit
				--,SUBSTRING(x.TransactionCode,22,11) Brandgrupcode
				--,COUNT(*) HRASubmit
				--,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN SUBSTRING(x.TransactionCode,43,1) ELSE SUBSTRING(x.TransactionCode,44,1) END as revisiontype
				--,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,39,2)) END as KpsWeek 
				--,SUBSTRING(x.TransactionCode,34,4) as KpsYear
				--,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(x.TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(x.TransactionCode,42,1)) END as KpsDay
				--from (
				--		SELECT [TransactionCode]
				--		,max([IDFlow]) IDFlow
				--		FROM [UtilTransactionLogs] a
				--		where TransactionCode like 'WPC/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/'+@RevisionTypeCursor
				--		and a.CreatedDate=(select MAX(CreatedDate) from
				--		[dbo].[UtilTransactionLogs]
				--		where TransactionCode=a.TransactionCode)
				--		group by a.TransactionCode
				--	) x
				--where x.IDFlow in (69,23,22,24)
				----and SUBSTRING(x.TransactionCode,34,9) like '%2016/?31/%'
				--group by substring(x.TransactionCode,5,4)
				--,SUBSTRING(x.TransactionCode,12,4) 
				--,SUBSTRING(x.TransactionCode,22,11)
				--,CASE WHEN SUBSTRING(x.TransactionCode,39,2) LIKE '%/' THEN SUBSTRING(x.TransactionCode,43,1) ELSE SUBSTRING(x.TransactionCode,44,1) END
				--,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,39,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,39,2)) END
				--,SUBSTRING(TransactionCode,34,4) 
				--,CASE WHEN SUBSTRING(TransactionCode,39,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(TransactionCode,41,1)) ELSE CONVERT(INT,SUBSTRING(TransactionCode,42,1)) END
				--ORDER BY substring(x.TransactionCode,5,4),SUBSTRING(x.TransactionCode,12,4) ,SUBSTRING(x.TransactionCode,22,11) ASC;

				--DELETE FROM @TempTableTotalTransactionlogSubmit
				--INSERT INTO @TempTableTotalTransactionlogSubmit
				

				--select * from @TempTableTotalTransactionlogSubmit

				-- END : additional logic for BrandGroup and BrandGroupCode

				IF(@RevisionTypeCursor != 0)
				BEGIN
					SELECT @totalEntryVerification = (SELECT COUNT(*) FROM ExePlantProductionEntryVerificationView
														WHERE
														(
														convert(date,UpdatedDate) = @StartDate
														AND 
														(
															(convert(date,ProductionDate) >= @payrollstartdate)
															AND 
															(convert(date,ProductionDate) <= @payrollenddate)
														)
													)AND 
													SUBSTRING(GroupCode,2,1) != '5' AND
													ProductionEntryCode like 'EBL/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor);
				END
				ELSE
				BEGIN
					SELECT @totalEntryVerification = (SELECT COUNT(*) FROM ExePlantProductionEntryVerificationView
					WHERE 
					(convert(date,ProductionDate) = @StartDate) AND 
					SUBSTRING(GroupCode,2,1) != '5' AND 
					ProductionEntryCode like 'EBL/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor);
				END

				--SELECT @totalTransactionlogSubmit = ISNULL((SELECT Total FROM @TempTableTotalTransactionlogSubmit
				--WHERE Plant = @LocationCodeCursor AND Unit = @UnitCodeCursor AND BrandGroupCode = @BrandGroupCodeCursor AND revisiontype = @RevisionTypeCursor),0);

				SELECT @totalTransactionlogSubmit = (SELECT COUNT(*) HRASubmit
						from (
								SELECT [TransactionCode]
								,max([IDFlow]) IDFlow
								FROM [UtilTransactionLogs] a
								where TransactionCode like 'WPC/'+@LocationCodeCursor+'/'+@ShiftCursor+'/'+@UnitCodeCursor+'/%/'+@BrandGroupCodeCursor+'/'+@KpsYearCursor+'/'+@KpsWeekCursor+'/'+@KpsDayCursor+'/'+@RevisionTypeCursor
								and a.CreatedDate=(select MAX(CreatedDate) from
								[dbo].[UtilTransactionLogs]
								where TransactionCode=a.TransactionCode)
								group by a.TransactionCode
							) x
						where x.IDFlow in (69,23,22,24));

				--select @totalEntryVerification,@totalTransactionlogSubmit

				IF(@totalEntryVerification - @totalTransactionlogSubmit = 0)
				BEGIN
					SET @status = 'SUBMITTED';
					SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole22 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 22) END;
				END
				ELSE IF((@totalEntryVerification - @totalTransactionlogSubmit > 0) OR (@LastTransLogs = 0))
				BEGIN
					SET @status = 'DRAFT';
					SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole21 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21) END;
				END
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

			IF(@LastTransLogs = 57)
			BEGIN
				SET @status = 'DRAFT';
				SET @idNextRole = CASE WHEN @MarkUnitCode = 0 THEN @idNextRole21 ELSE (SELECT DestinationRole FROM UtilFlows WHERE IDFlow = 21) END;
			END

			SELECT @PRODUCTIONDATE = (SELECT DateADD(dd,@KpsDayCursor-1,StartDate) FROM MstGenWeek WHERE YEAR = @KpsYearCursor AND Week =@KpsWeekCursor)
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
				FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor,@RevisionTypeCursor,@ShiftCursor;
				CONTINUE;
			END
			ELSE IF(@TransactionStatus = 'inprogress' AND @status = 'COMPLETED')
			BEGIN
				FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor,@RevisionTypeCursor,@ShiftCursor;
				CONTINUE;
			END
			ELSE
			BEGIN
				IF(ISNULL(@status,'Null') != 'Null' )
				BEGIN
					INSERT INTO @WagesApprovalListTemp VALUES (
					@LocationCodeCursor
					,@UnitCodeCursor
					,(SELECT BrandGroupCode FROM MstGenBrand WHERE BrandCode = @BrandGroupCodeCursor)
					,@PRODUCTIONDATE,@ShiftCursor
					,@status
					,@idNextRole
					,(CASE WHEN @status = 'COMPLETED' THEN null ELSE @nextRole END)
					,@RevisionTypeCursor
					);
				END;
			END
		END
	FETCH NEXT FROM Cursor_ValidLocationUnitCode INTO @LocationCodeCursor, @UnitCodeCursor, @BrandGroupCodeCursor, @KpsWeekCursor, @KpsYearCursor,@KpsDayCursor,@RevisionTypeCursor,@ShiftCursor
	END
	CLOSE Cursor_ValidLocationUnitCode
	DEALLOCATE Cursor_ValidLocationUnitCode
	--SELECT * FROM @WagesApprovalListTemp
	INSERT INTO @WagesApprovalList (LocationCode, UnitCode, BrandCode, ProductionDate,Shift,Status,IDRole,RolesName,RevisionType) 
	SELECT DISTINCT LocationCode, UnitCode, BrandCode, ProductionDate,Shift,Status,IDRole,RolesName,RevisionType FROM @WagesApprovalListTemp
	RETURN;

END;
-- end : new code


