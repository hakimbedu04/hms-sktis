IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetTransactionHistory]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetTransactionHistory]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetTransactionHistory]
(	
	@transactionCode VARCHAR(128),
	@pageSource VARCHAR(128)
)
RETURNS @TempTable TABLE (name Varchar(100), role Varchar(64), action Varchar(64), date DATETIME, note Varchar(256))
AS
BEGIN
	-- author : Hakim
	-- description : create history from prodcardapprovaldetail with loop brandcode
	-- date : 2016-12-09
	-- ticket : http://tp.voxteneo.co.id/entity/12590

	-- author : Hakim
	-- description : CHANGE DATE IF WEEK ONLY 1 CHARACTER
	-- date : 2017-01-03
	-- ticket : 
	DECLARE @Trans VARCHAR(255);
	SET @Trans = @transactionCode;

	IF(@Trans like '%Fee%')
	BEGIN
		WITH X AS (
			SELECT TOP 100 MAX(mat.Name) AS name,
			   MAX(utl.UpdatedBy) AS role,
			   (MAX(ufn.FunctionName)) AS action,
			   MAX(utl.CreatedDate) AS date,
			   MAX(utl.Comments) AS note
			FROM dbo.UtilTransactionLogs AS utl
			INNER JOIN dbo.UtilFlows AS uf ON uf.IDFlow = utl.IDFlow
			INNER JOIN dbo.MstADTemp AS mat ON mat.UserAD = utl.CreatedBy
			INNER JOIN dbo.UtilFlowFunctionView AS ufn ON ufn.IDFlow = uf.IDFlow 
				AND ufn.UserAD = mat.UserAD
				AND ufn.SourceFunctionForm = CASE 
					WHEN (@pageSource IS NOT NULL) 
					THEN @pageSource
					ELSE ufn.SourceFunctionForm
				END
			INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
			INNER JOIN UtilResponsibility urp ON urp.IDResponsibility = uur.IDResponsibility
			INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
			WHERE utl.TransactionCode LIKE '%'+@transactionCode+'%' 
			AND utl.IDFlow >= 41
			AND utl.IDFlow <= 55
			GROUP BY ufn.UserAD,ufn.FunctionName,utl.UpdatedBy
			ORDER BY max(utl.TransactionDate) DESC
		)
		INSERT INTO @TempTable SELECT * FROM X;
	END;
	ELSE
	BEGIN
		IF(@pageSource = 'ProductionCardApprovalDetail')
		BEGIN
			DECLARE @LocationCode VARCHAR(4),
					@UnitCode VARCHAR(4),
					@BrandGroupCode VARCHAR(20),
					@Year INT,
					@Week INT,
					@Day INT,
					@NewTranscode VARCHAR(128),
					@BrandCodeCursor VARCHAR(20)

			SET @transactionCode = @transactionCode
			SET @LocationCode = SUBSTRING(@transactionCode,5,4)
			SET @UnitCode = SUBSTRING(@transactionCode,12,4)
			SET @BrandGroupCode = SUBSTRING(@transactionCode,19,10)
			SET @Year = CONVERT(INT,SUBSTRING(@transactionCode,30,4))
			--SET @Week = CONVERT(INT,SUBSTRING(@transactionCode,35,2))
			--SET @Day = CONVERT(INT,SUBSTRING(@transactionCode,38,1))
			SET @Week = CASE WHEN SUBSTRING(@transactionCode,35,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(@transactionCode,35,1)) ELSE CONVERT(INT,SUBSTRING(@transactionCode,35,2)) END
			SET @Day = CASE WHEN SUBSTRING(@transactionCode,35,2) LIKE '%/' THEN CONVERT(INT,SUBSTRING(@transactionCode,37,1)) ELSE CONVERT(INT,SUBSTRING(@transactionCode,38,1)) END 

			DECLARE Cursor_Brand CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
				SELECT BrandCode FROM ProductionCard
				WHERE LocationCode = @LocationCode
				AND UnitCode = @UnitCode
				AND BrandGroupCode = @BrandGroupCode
				AND ProductionDate = (SELECT DateADD(dd,@Day-1,StartDate) FROM MstGenWeek WHERE YEAR = @Year AND Week =@Week) --dateadd (week, @Week, dateadd (year, @Year-1900, 0)) + (@Day+1) - datepart(dw, dateadd (week, @Week, dateadd (year, @Year-1900, 0)) )
				GROUP BY BrandCode
			OPEN Cursor_Brand
			FETCH NEXT FROM Cursor_Brand INTO @BrandCodeCursor
			WHILE @@FETCH_STATUS = 0 BEGIN 
				SET @NewTranscode = SUBSTRING(@transactionCode,1,18)+@BrandCodeCursor+SUBSTRING(@transactionCode,29,12)
				INSERT INTO @TempTable
				SELECT TOP 100 MAX(mat.Name) AS name,
					MAX(utl.UpdatedBy) AS role,
					(MAX(ufn.FunctionName)) AS action,
					MAX(utl.CreatedDate) AS date,
					MAX(utl.Comments) AS note
				FROM dbo.UtilTransactionLogs AS utl
				INNER JOIN dbo.UtilFlows AS uf ON uf.IDFlow = utl.IDFlow
				INNER JOIN dbo.MstADTemp AS mat ON mat.UserAD = utl.CreatedBy
				INNER JOIN dbo.UtilFlowFunctionView AS ufn ON ufn.IDFlow = uf.IDFlow 
					AND ufn.UserAD = mat.UserAD
					AND ufn.SourceFunctionForm = CASE 
						WHEN (NULL IS NOT NULL) 
						THEN NULL
						ELSE ufn.SourceFunctionForm
					END
				INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
				INNER JOIN UtilResponsibility urp ON urp.IDResponsibility = uur.IDResponsibility
				INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
				WHERE utl.TransactionCode LIKE '%'+@NewTranscode+'%' 
				GROUP BY ufn.UserAD,ufn.FunctionName
				ORDER BY max(utl.TransactionDate) DESC
			FETCH NEXT FROM Cursor_Brand INTO @BrandCodeCursor
			END
			CLOSE Cursor_Brand
			DEALLOCATE Cursor_Brand
		END
		ELSE
		BEGIN
			WITH X AS (
				SELECT TOP 100 MAX(mat.Name) AS name,
					MAX(utl.UpdatedBy) AS role,
					(MAX(ufn.FunctionName)) AS action,
					MAX(utl.CreatedDate) AS date,
					MAX(utl.Comments) AS note
				FROM dbo.UtilTransactionLogs AS utl
				INNER JOIN dbo.UtilFlows AS uf ON uf.IDFlow = utl.IDFlow
				INNER JOIN dbo.MstADTemp AS mat ON mat.UserAD = utl.CreatedBy
				INNER JOIN dbo.UtilFlowFunctionView AS ufn ON ufn.IDFlow = uf.IDFlow 
					AND ufn.UserAD = mat.UserAD
					AND ufn.SourceFunctionForm = CASE 
						WHEN (@pageSource IS NOT NULL) 
						THEN @pageSource
						ELSE ufn.SourceFunctionForm
					END
				INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
				INNER JOIN UtilResponsibility urp ON urp.IDResponsibility = uur.IDResponsibility
				INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
				WHERE utl.TransactionCode LIKE '%'+@transactionCode+'%' 
				GROUP BY ufn.UserAD,ufn.FunctionName
				ORDER BY max(utl.TransactionDate) DESC
			)
			INSERT INTO @TempTable SELECT * FROM X;
		END
	END
	RETURN;
END;