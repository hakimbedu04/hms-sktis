/****** Object:  UserDefinedFunction [dbo].[GetTransactionHistory]    Script Date: 8/28/2016 4:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* Transaction History */
ALTER FUNCTION [dbo].[GetTransactionHistory]
(	
	@transactionCode VARCHAR(128),
	@pageSource VARCHAR(128)
)
RETURNS TABLE 
AS
RETURN 
(
	--SELECT TOP 100
	--	utl.CreatedBy name,
	--	ur.RolesName [role], 
	--	uf.MessageText [action],
	--	utl.CreatedDate [date],
	--	utl.Comments note 
	--FROM UtilTransactionLogs utl
	--INNER JOIN MstADTemp mat ON mat.UserAD = utl.CreatedBy
	--INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
	--INNER JOIN UtilResponsibility urp on urp.IDResponsibility = uur.IDResponsibility
	--INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
	--INNER JOIN UtilFlows uf ON uf.IDFlow = utl.IDFlow
	--WHERE utl.TransactionCode like '%'+@transactionCode+'%'
	--ORDER BY utl.CreatedDate desc
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