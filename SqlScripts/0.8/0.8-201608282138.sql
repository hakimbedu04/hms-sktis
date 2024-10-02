/****** Object:  UserDefinedFunction [dbo].[GetTransactionHistoryWagesProdcardCorrection]    Script Date: 8/28/2016 9:37:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description: Get TransactionHistoryWagesProdcardCorrection
-- Ticket: http://tp.voxteneo.co.id/entity/8395
-- Author: wahyu
-- Update: 27/07/2016
-- =============================================
/* Transaction History */
ALTER FUNCTION [dbo].[GetTransactionHistoryWagesProdcardCorrection]
(	
	@transactionCode VARCHAR(128)
)
RETURNS TABLE 
AS
RETURN 
(
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
	INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
	INNER JOIN UtilResponsibility urp ON urp.IDResponsibility = uur.IDResponsibility
	INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
	WHERE utl.TransactionCode LIKE '%'+@transactionCode+'%' and utl.TransactionCode not LIKE '%'+@transactionCode+'/0%'
	GROUP BY ufn.UserAD,ufn.FunctionName
	ORDER BY max(utl.TransactionDate) DESC
)