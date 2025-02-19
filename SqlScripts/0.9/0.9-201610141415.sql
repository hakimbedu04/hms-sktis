-- =============================================
-- Author:		Robby Prima
-- Create date: 2016-08-11
-- Description:	Check if user can do button action
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-23
-- Description:	Check if user can do button action / part WPC(wages)
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-25
-- Description:	add EBL code
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-29
-- Description:	modify WPC
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-29
-- Description:	change WPC algorith
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-09-01
-- Description:	conform WPC algorith (ProdcardApprovalDetail And EblekReleaseApproval)
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-09-01
-- Description:	add btn for payment (return) and idflow 24 on prodcard
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-10-06
-- Description:	double check for save productioncard, select idflow(indicated as submit) except save, than enable btn save
-- http://tp.voxteneo.co.id/entity/10348
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-10-14
-- Description:	add if condition productioncard btn save and wpc translog
-- http://tp.voxteneo.co.id/entity/10348
-- =============================================

ALTER FUNCTION [dbo].[RoleButtonChecker]
(
--	@TPOFeeCode VARCHAR(250),
	@TransactionCode VARCHAR(250),
	@IDRole INT,
	@Page VARCHAR(250),
	@Button VARCHAR(250)
)
RETURNS @ButtonState TABLE (IsEnabled INT)
AS
BEGIN
	DECLARE @DestinationRole INT
	DECLARE @Ret INT
	DECLARE @RC INT
	DECLARE @RoleCode VARCHAR(10)
	DECLARE @LatestIDFlow INT
	DECLARE @IDFlows TABLE (idFlow INT)
	DECLARE @Temp INT

	SET @Ret = 0;
	
	if(@TransactionCode like '%FEE%')
	BEGIN
		SET @DestinationRole = (SELECT DestinationRole FROM TPOFeeApprovalView WHERE TPOFeeCode = @TransactionCode)
		SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole

		if (@DestinationRole = @IDRole)
		BEGIN
			SELECT @RC = COUNT(*) FROM UtilFlowFunctionView WHERE SourceRolesCodes = @RoleCode AND SourceFunctionForm = @Page AND FunctionName = @Button

			if (@RC > 0)
			BEGIN
				SET @Ret = 1
			END
		END
	END
	ELSE IF((@TransactionCode like '%WPC%') OR (@TransactionCode like '%EBL%'))
	BEGIN
		
		SELECT TOP 1 @LatestIDFlow = IDFlow 
		FROM [dbo].[UtilTransactionLogs] 
		WHERE TransactionCode like '%' + @TransactionCode + '%'
		ORDER BY UpdatedDate DESC

		SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole

		-- Production Card checker
		IF(@Page = 'ProductionCard')
		BEGIN
			IF(@Button = 'Save' AND @TransactionCode like '%EBL%')
			BEGIN
				SELECT TOP 1 @LatestIDFlow = ax.IDFlow 
				FROM [dbo].[UtilTransactionLogs] ax
				INNER JOIN UtilFlows a
				ON ax.IDFlow=a.IDFlow
				INNER JOIN UtilFunctions b 
				ON a.ActionButton = b.IDFunction
				WHERE ax.TransactionCode LIKE @TransactionCode
				AND b.FunctionName != @Button
				ORDER BY ax.UpdatedDate DESC

				IF(@LatestIDFlow in(17,18,19,21,24,57))
				BEGIN
					INSERT INTO @IDFlows VALUES (17)
					INSERT INTO @IDFlows VALUES (18)
					INSERT INTO @IDFlows VALUES (19)
					INSERT INTO @IDFlows VALUES (21)
					INSERT INTO @IDFlows VALUES (24)
					INSERT INTO @IDFlows VALUES (57)
				END
			END
			ELSE IF(@Button = 'Save' AND @TransactionCode like '%WPC%')
			BEGIN
				IF(@LatestIDFlow in(21,24,57))
				BEGIN
					INSERT INTO @IDFlows VALUES (21)
					INSERT INTO @IDFlows VALUES (24)
					INSERT INTO @IDFlows VALUES (57)
				END
			END
			ELSE IF(@Button = 'Submit' AND @LatestIDFlow = 21)
			BEGIN
				INSERT INTO @IDFlows VALUES (21)
			END
			ELSE IF(@Button = 'Cancel Submit' AND @LatestIDFlow in (22,23,69) )
			BEGIN
				INSERT INTO @IDFlows VALUES (22)
				INSERT INTO @IDFlows VALUES (23)
				INSERT INTO @IDFlows VALUES (69)
			END
		END

		ELSE IF(@Page = 'ProductionCardApprovalDetail')
		BEGIN
			IF(@RoleCode = 'PPC')
			BEGIN
				--IF((@Button = 'Approve' OR @Button = 'Return' )AND @LatestIDFlow in (22,23,24,69) )
				IF((@Button = 'Approve' OR @Button = 'Return' )AND @LatestIDFlow in (22,23,69) )
				BEGIN
					INSERT INTO @IDFlows VALUES (22)
					INSERT INTO @IDFlows VALUES (23)
					--INSERT INTO @IDFlows VALUES (24)
					INSERT INTO @IDFlows VALUES (69)
				END
			END
			ELSE IF(@RoleCode = 'PGSC')
			BEGIN
				IF((@Button = 'Approve' OR @Button = 'Return') AND @LatestIDFlow in (56) )
				BEGIN
					INSERT INTO @IDFlows VALUES (56)
				END
			END
			ELSE IF(@RoleCode = 'PAY')
			BEGIN
				--IF((@Button = 'Complete' OR @Button = 'Return') AND @LatestIDFlow in (25,56))
				IF((@Button = 'Complete' OR @Button = 'Return') AND @LatestIDFlow in (25))
				BEGIN
					INSERT INTO @IDFlows VALUES (25)
					--INSERT INTO @IDFlows VALUES (56)
				END
			END
		END

		ELSE IF(@Page = 'EblekRelease')
		BEGIN
		
			IF(@Button = 'SendApproval' AND @LatestIDFlow in (26,70))
			BEGIN
				INSERT INTO @IDFlows VALUES (26)
				INSERT INTO @IDFlows VALUES (70)
			END
		END

		ELSE IF(@Page = 'EblekReleaseApproval')
		BEGIN
			IF(@RoleCode = 'PGSC')
			BEGIN
				IF((@Button = 'Approve' OR @Button = 'Return') AND @LatestIDFlow in(27,28))
				BEGIN
					INSERT INTO @IDFlows VALUES (27)
					INSERT INTO @IDFlows VALUES (28)
				END
			END
			ELSE IF(@RoleCode = 'SPGS')
			BEGIN
				IF((@Button = 'Approve' OR @Button = 'Return') AND @LatestIDFlow in(29))
				BEGIN
					INSERT INTO @IDFlows VALUES (29)
				END
			END
		END

		select @Temp = count(*) from UtilFlowFunctionView where DestinationFunctionForm = @Page and DestinationRolesCodes = @RoleCode and IDFlow IN (select * from @IDFlows)
		--select @Temp = count(*) from @IDFlows
		
		IF(@Temp > 0)
		BEGIN
			--SET @Ret = @Temp
			SET @Ret = 1
		END
	END
	-- Return the result of the function
	INSERT INTO @ButtonState SELECT @Ret
	RETURN;

END;

