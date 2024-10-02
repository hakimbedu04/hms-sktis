-- =============================================
-- Description: give condition for flow tpo fee
-- Ticket: http://tp.voxteneo.co.id/entity/10371
-- Author: HAKIM
-- Updated: 2 - 2016/10/10
-- =============================================

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTransactionHistory]'))
	DROP FUNCTION [dbo].[GetTransactionHistory]
GO
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
			GROUP BY ufn.UserAD,ufn.FunctionName
			ORDER BY max(utl.TransactionDate) DESC
		)
		INSERT INTO @TempTable SELECT * FROM X;
	END;
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
	END;
	RETURN;
END;