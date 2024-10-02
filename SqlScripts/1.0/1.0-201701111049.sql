IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetWagesProductionCardApprovalView]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetWagesProductionCardApprovalView]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[GetWagesProductionCardApprovalView] 
-- Add the parameters for the stored procedure here
@locationCode		VARCHAR(250),
@unitCode			VARCHAR(250),
@revisiontype	    VARCHAR(1)

AS
BEGIN

	-- Description: CREATE SP : change view WagesProductionCardApprovalView to SP due to timeout data select
	-- Ticket: http://tp.voxteneo.co.id/entity/
	-- Author: HAKIM
	-- Date: 2017/01/11

	DECLARE @paramlocationCode VARCHAR(250);
	DECLARE @paramunitCode VARCHAR(250);
	DECLARE @paramrevisiontype VARCHAR(1);

	SET @paramlocationCode = @locationCode;
	SET @paramunitCode = @unitCode;
	SET @paramrevisiontype = @revisiontype

	SELECT top 100 ROW_NUMBER() OVER(ORDER BY card.ProductionCardCode DESC) AS Row,
		   MAX(card.RevisionType) RevisionType,
		   card.ProductionCardCode,
		   card.LocationCode,
		   card.UnitCode,
		   card.BrandGroupCode AS BrandCode,
		   card.ProductionDate,
		   trans.IDFlow,
				 CASE
					 WHEN [trans].IDFlow = 21
					 THEN 'DRAFT'
					 WHEN [trans].IDFlow = 22
					 THEN 'SUBMITTED'
					 WHEN [trans].IDFlow = 25
					 THEN 'APPROVED'
					 WHEN [trans].IDFlow = 26
					 THEN 'COMPLETED'
				 END AS Status,
		   AD.UserAD,
		   role.RolesName,
		   MAX(card.UpdatedDate) UpdatedDate,
		   card.Shift
	FROM dbo.ProductionCard AS card
		 INNER JOIN dbo.UtilTransactionLogs AS trans ON trans.TransactionCode = (card.ProductionCardCode + '/' + CONVERT(VARCHAR(1), card.RevisionType))
		 LEFT JOIN dbo.MstADTemp AS AD ON AD.UserAD = trans.CreatedBy
		 LEFT JOIN dbo.UtilUsersResponsibility AS userresponse ON userresponse.UserAD = AD.UserAD
		 LEFT JOIN dbo.UtilResponsibility AS response ON response.IDResponsibility = userresponse.IDResponsibility
		 LEFT JOIN dbo.UtilRoles AS role ON role.IDRole = response.IDRole
	WHERE card.LocationCode = @paramlocationCode
	and UnitCode = @paramunitCode
	and RevisionType = @paramrevisiontype 
	and ( trans.IDFlow IN( 21, 22, 25, 26 ) )
	GROUP BY card.ProductionCardCode, card.LocationCode, card.UnitCode, card.BrandGroupCode, card.ProductionDate, AD.UserAD , role.RolesName, 
	trans.IDFlow,  
	card.Shift

END;