/****** Object:  UserDefinedFunction [dbo].[GetUserAndEmail]    Script Date: 22/07/2016 15:11:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Description: Get User Email on Return Eblek Release Approval
-- Ticket: http://tp.voxteneo.co.id/entity/3783
-- Author: AZKA
-- Updated: 1.0 - 2016/04/19
-- =============================================

-- =============================================
-- Description: Get User Email on submit TPOTPK, PlanTPU
-- Ticket: http://tp.voxteneo.co.id/entity/7179
-- Author: AZKA
-- Updated: 2.0 - 2016/06/23
-- =============================================

-- =============================================
-- Description: Get User Email on submit TPO Fee Actual Detail
-- Ticket: http://tp.voxteneo.co.id/entity/8297
-- Author: AZKA
-- Updated: 3.0 - 2016/06/30
-- =============================================

-- =============================================
-- Description: Get User Email on save Equipment Request
-- Ticket: http://tp.voxteneo.co.id/entity/8358
-- Author: HAKIM
-- Updated: 4.0 - 2016/07/14
-- =============================================

-- =============================================
-- Description: Get User Email on submit PlantTargetProductionGroup
-- Ticket: http://tp.voxteneo.co.id/entity/8368
-- Author: Yudha
-- Updated: 4.1 - 2016/07/15
-- =============================================

-- =============================================
-- Description: Get User Email on submit ProductionEntryVerification
-- Ticket: http://tp.voxteneo.co.id/entity/8357
-- Author: Yudha
-- Updated: 4.2 - 2016/07/18
-- =============================================

-- =============================================
-- Description: Get User Email on SendApproval WagesEblekRelease
-- Ticket: http://tp.voxteneo.co.id/entity/8449
-- Author: Wahyu
-- Updated: 4.2 - 2016/07/22
-- =============================================

-- =============================================
-- Description: fixing equpment fullfilment and request
-- Ticket: http://tp.voxteneo.co.id/entity/8439
-- Author: azka
-- Updated: 4.4 - 2016/07/22
-- =============================================

-- =============================================
-- Description: Get User Email on save equipment transfer
-- Ticket: http://tp.voxteneo.co.id/entity/8439
-- Author: Indra
-- Updated: 4.5 - 2016/07/22
-- =============================================

ALTER FUNCTION [dbo].[GetUserAndEmail]
(	
	@FunctionName as varchar(50),
	@ButtonName as varchar(50),
	@LocationCode as varchar(50),
	@UnitCode as varchar(50),
	@BrandCode as varchar(50),
	@Shift int,
	@KpsYear int,
	@KpsWeek int
)
RETURNS @UserAndEmail TABLE (UserAD VARCHAR(100), Name VARCHAR(100), Email VARCHAR(100), IDResponsibility INT, Location VARCHAR(10), Unit VARCHAR(10))
AS
BEGIN
	IF(@FunctionName LIKE 'EblekReleaseApproval') OR (@FunctionName LIKE 'PlantTargetProductionGroup') OR (@FunctionName LIKE 'ProductionEntryVerification') OR (@FunctionName LIKE 'ProductionCard') OR (@FunctionName LIKE 'PlantProductionEntry')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole IN 
					(
						select IDRole from UtilRoles where IDRole IN (select DestinationRole from UtilFlows where ActionButton = 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode and Unit = @UnitCode)
			)
	END;

	IF(@FunctionName LIKE 'TPOTargetProductionGroup')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole = 
					(
						select IDRole from UtilRoles where IDRole = (select DestinationRole from UtilFlows where ActionButton = 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode)
			) AND ad.StatusActive = 1
	END;

	IF(@FunctionName LIKE 'TargetProductionUnit')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
						select IDResponsibility from UtilResponsibility where IDRole = 
						(
							select IDRole from UtilRoles where IDRole = (select DestinationRole from UtilFlows where ActionButton = 
								(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
									(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
								)
							)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode and Unit IN 
								(
									select UnitCode from PlanTargetProductionUnit where KPSYear = @KpsYear and KPSWeek = @KpsWeek and LocationCode = @LocationCode and BrandCode = @BrandCode and Shift = @Shift
								)
							)
						)
	END;

	IF(@FunctionName LIKE 'TPOFeeActualDetail')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole = 
					(
						select IDRole from UtilRoles where IDRole = (select DestinationRole from UtilFlows where ActionButton = 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode)
			) AND ad.StatusActive = 1
	END;
	
	IF(@FunctionName LIKE 'EquipmentRequest')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole IN
					(
						select IDRole from UtilRoles where IDRole IN (select DestinationRole from UtilFlows where ActionButton IN 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = (SELECT ParentLocationCode FROM MstGenLocation where LocationCode = @LocationCode))
			) AND ad.StatusActive = 1
	END;
	
	IF(@FunctionName LIKE 'EblekRelease') 
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole IN 
					(
						select IDRole from UtilRoles where IDRole IN (select DestinationRole from UtilFlows where ActionButton = 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode)
			)
	END;
	
	IF(@FunctionName LIKE 'EquipmentFullfillment')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole IN
					(
						select IDRole from UtilRoles where IDRole IN (select DestinationRole from UtilFlows where ActionButton IN 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode)
			) AND ad.StatusActive = 1
	END;

	IF(@FunctionName LIKE 'EquipmentTransfer')
	BEGIN
		INSERT INTO @UserAndEmail
			SELECT ad.UserAD, ad.Name, ad.Email, ur.IDResponsibility, ru.Location, ru.Unit
			from UtilUsersResponsibility ur
			inner join MstADTemp ad on ad.UserAD = ur.UserAD
			inner join UtilResponsibilityRules urr on urr.IDResponsibility = ur.IDResponsibility
			inner join UtilRules ru on ru.IDRule = urr.IDRule
			where ur.IDResponsibility IN 
			(
				select IDResponsibility from UtilResponsibilityRules where IDResponsibility IN 
				(
					select IDResponsibility from UtilResponsibility where IDRole IN
					(
						select IDRole from UtilRoles where IDRole IN (select DestinationRole from UtilFlows where ActionButton IN 
							(select IDFunction from UtilFunctions where FunctionName = @ButtonName and ParentIDFunction = 
							(select IDFunction from UtilFunctions where FunctionName = @FunctionName)))
					)
				)and IDRule IN (select IDRule from UtilRules where Location = @LocationCode)
			) AND ad.StatusActive = 1
	END;

	RETURN;
END;

GO


