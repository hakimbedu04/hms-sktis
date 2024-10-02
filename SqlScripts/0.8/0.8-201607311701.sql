/****** Object:  StoredProcedure [dbo].[TransactionLog]    Script Date: 7/31/2016 11:21:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Author: Jejen Suhendar

-- Author: Whisnu Sucitanuary
-- Changes: Add Page Condition for ProductionCardApprovalDetail


-- Versi 1.2
-- Author: Harizal
-- Changes: Add Condition @Page LIKE 'ProductionCard', Source Get from DestinationForm  = 143

-- =============================================
-- Description: edit eblek release approval
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 05/04/2016
-- =============================================

-- =============================================
-- Description: edit eblek release approval
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 05/04/2016 - part 2
-- =============================================

-- =============================================
-- Description: edit eblek release approval
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 05/04/2016 - part 3
-- =============================================

-- =============================================
-- Description: Edit Prodcard Approval detail
-- Ticket: http://tp.voxteneo.co.id/entity/5096
-- Author: Indra
-- Update: 20/04/2016
-- =============================================

-- =============================================
-- Description: Add new page TPOFeeActualDetail
-- Ticket: http://tp.voxteneo.co.id/entity/6316
-- Author: Yudha
-- Update: 28/04/2016
-- =============================================

-- =============================================
-- Description: fixing ProductionCardApprovalDetail
-- Ticket: http://tp.voxteneo.co.id/entity/8438
-- Author: Azka
-- Updated: 2016/07/31
-- =============================================

ALTER PROCEDURE [dbo].[TransactionLog] 
-- Add the parameters for the stored procedure here
      @Separator        VARCHAR(20),
      @Page             VARCHAR(30),
      @Year             INT,
      @Week             INT,
      @code_1           VARCHAR(16),
      @code_2           VARCHAR(16),
      @code_3           VARCHAR(16),
      @code_4           VARCHAR(16),
      @code_5           VARCHAR(16),
      @code_6           VARCHAR(16),
      @code_7           VARCHAR(16),
      @code_8           VARCHAR(16),
      @code_9           VARCHAR(16),
      @Transaction_Date DATETIME,
      @ActionButton     VARCHAR(16),
      @ActionTime       DATETIME,
      @UserName         VARCHAR(32),
	  @TransactionCode  VARCHAR(128) = NULL,
	  @Message VARCHAR(16)
AS
    BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
        DECLARE @IDFunct         INT,
                @IDButton        INT,
                @IDFlow          INT,
                @Counter         INT,
                @Transaction_Date_New DATETIME,
                @RolesCodes      VARCHAR(128),
                --@TransactionCode VARCHAR(128),
				@Count	INT,
                @MessageText     VARCHAR(128) = @Message;

				
        DECLARE @FlowLists       TABLE (id int IDENTITY (1,1), IDFlow INT, MessageText VARCHAR(128));
        -- checking the  page
        -- PLANNING
        IF( @Page LIKE 'WPP%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN				
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'Weekly%';
            END;
        IF( @Page LIKE 'TPU%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN				
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END

			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TargetProductionUnit%';
            END;
        IF( @Page LIKE 'TPKPLANT%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7);
				END

			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'PlantTargetProductionGroup%';
            END;
        IF( @Page LIKE 'TPKTPO%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOTargetProductionGroup%';
            END;
        -- MAINTENANCE
        IF( @Page LIKE 'MTNCREQ%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentRequest';
            END;
        IF( @Page LIKE 'MTNCFULL%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentFullfillment';
            END;
        IF( @Page LIKE 'MTNCFULL%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentFullfillment';
            END;
        IF( @Page LIKE 'MTNCTRANS%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentTransfer';
            END;
        IF( @Page LIKE 'MTNCRECV%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentReceive';
            END;
        IF( @Page LIKE 'MTNCREPPLNT%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentRepairPlant';
            END;
        IF( @Page LIKE 'MTNCREPTPO%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EquipmentRepairTPO';
            END;
        IF( @Page LIKE 'MTNCQI%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'QualityInspection';
            END;
        -- Plant Execution
        IF( @Page LIKE 'ABSENTEISMP%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'PieceRate';
            END;
        IF( @Page LIKE 'ABSENTEISMD%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'Daily';
            END;
        IF( @Page LIKE 'LOADBALANCINGM%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'MultiSkill';
            END;
        IF( @Page LIKE 'LOADBALANCINGS%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'SingleSkill';
            END;
        IF( @Page LIKE 'PlantProductionEntry%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'PlantProductionEntry%';
            END;
        IF( @Page LIKE 'ProductionEntryVerification' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
					SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
				
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionEntryVerification';
            END;
        IF( @Page LIKE 'TPOProductionEntry' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END

			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOProductionEntry';
            END;
        IF( @Page LIKE 'TPOProductionEntryVerification%' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOProductionEntryVerification%';
            END;
        -- for wages
        IF( @Page LIKE 'ProductionCard' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
	                   SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
                
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCard';
            END;
        IF( @Page LIKE 'ProductionCardRev' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCardRev';
            END;
	   IF( @Page LIKE 'EblekRelease' )
            BEGIN
			 IF(@TransactionCode IS NULL OR @TransactionCode = '')
				BEGIN
				    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6);
				END
			 SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EblekRelease';
            END;
		IF( @Page LIKE 'ProductionCardApprovalDetail' )
            BEGIN
			 --IF(@TransactionCode IS NULL OR @TransactionCode = '')
				--BEGIN
				--    SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_8) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_9);
				--END
			 --SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCardApprovalDetail';
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCardApprovalDetail';
				SELECT TOP 1 @RolesCodes = SourceRolesCodes FROM UtilFlowFunctionView uffv WHERE uffv.UserAD = @UserName and uffv.SourceFunctionForm = 'ProductionCardApprovalDetail';
				IF(@ActionButton LIKE 'Approve')
					BEGIN
						IF(@RolesCodes LIKE 'PPC') -- idflow 56
						BEGIN
							SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
							WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PGSC';
						END;
						ELSE IF(@RolesCodes LIKE 'PGSC') -- idflow 25
						BEGIN
							SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
							WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PAY';
						END;
						ELSE
						BEGIN
							SELECT TOP 1 @IDFlow = IDFlow FROM UtilTransactionLogs WHERE TransactionCode = @TransactionCode ORDER BY CreatedDate DESC;
							DECLARE @idflow22 INT;
							SELECT @idflow22 = min(uflow.IDFlow) FROM UtilFlows uflow WHERE uflow.FormSource = (SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCard') AND uflow.ActionButton = 
																							( SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.ParentIDFunction = (SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ProductionCard') 
																							AND ufun.FunctionName = 'Submit');

							DECLARE @idflow56 INT;
							SELECT TOP 1 @idflow56 = IDFlow FROM UtilFlowFunctionView uffv 
							WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.FunctionName = 'Approve'  AND uffv.DestinationRolesCodes = 'PGSC';
									
							IF @IDFlow = @idflow22
								BEGIN
									SELECT TOP 1   @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PGSC';
											
									INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
									VALUES( @TransactionCode, @Transaction_Date, @IDFlow, '', @ActionTime, @UserName, GETDATE(), @UserName );
								END;
							ELSE IF @IDFlow = @idflow56
								BEGIN
									SELECT TOP 1   @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PAY';
											
									INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
									VALUES( @TransactionCode, @Transaction_Date, @IDFlow, '', @ActionTime, @UserName, GETDATE(), @UserName );
								END;
							RETURN;
						END;
					END;
				ELSE IF(@ActionButton LIKE 'Return')
				BEGIN
					SELECT TOP 1   @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'ProductionCardApprovalDetail' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PAAPRD';

					INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
					VALUES( @TransactionCode, @Transaction_Date, @IDFlow, @MessageText, @ActionTime, @UserName, GETDATE(), @UserName );
					RETURN;
				END;
				ELSE
					BEGIN
						INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
						VALUES( @TransactionCode, @Transaction_Date, 26, '', @ActionTime, @UserName, GETDATE(), @UserName );
					END;
				RETURN;
            END;
		IF( @Page LIKE 'EblekReleaseApproval' )
			BEGIN
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EblekReleaseApproval';
				SELECT TOP 1 @RolesCodes = SourceRolesCodes FROM UtilFlowFunctionView uffv WHERE uffv.UserAD = @UserName;
				IF(@ActionButton LIKE 'Approve')
					BEGIN
							IF(@RolesCodes LIKE 'SPGS')-- SPGS = 30
								BEGIN
									SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PAAPRD';
									
									UPDATE ExeProductionEntryRelease SET IsLocked = 0 where ProductionEntryCode = @TransactionCode;
									UPDATE ExePlantProductionEntryVerification SET VerifySystem = 0, VerifyManual = 0 WHERE ProductionEntryCode = @TransactionCode;
								END;
							IF(@RolesCodes LIKE 'PGSC')-- PGSC = 29
								BEGIN
									SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'SPGS';
								END;
							ELSE
								BEGIN
									SELECT TOP 1 @IDFlow = IDFlow FROM UtilTransactionLogs WHERE TransactionCode = @TransactionCode ORDER BY CreatedDate DESC;
									DECLARE @idflow27 INT;
									SELECT @idflow27 = uflow.IDFlow FROM UtilFlows uflow WHERE uflow.FormSource = (SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EblekRelease') AND uflow.ActionButton = 
																							( SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.ParentIDFunction = (SELECT IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'EblekRelease') 
																							AND ufun.FunctionName = 'SendApproval');

									DECLARE @idflow29 INT;
									SELECT TOP 1 @idflow29 = IDFlow FROM UtilFlowFunctionView uffv 
									WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.FunctionName = 'Approve'  AND uffv.DestinationRolesCodes = 'SPGS';
									
									IF @IDFlow = @idflow27
										BEGIN
											SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
											WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'SPGS';
											
											INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
											VALUES( @TransactionCode, @Transaction_Date, @IDFlow, '', @ActionTime, @UserName, GETDATE(), @UserName );
										END;
									ELSE IF @IDFlow = @idflow29
										BEGIN
											SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
											WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.UserAD = @UserName AND uffv.FunctionName = @ActionButton AND uffv.SourceRolesCodes = @RolesCodes AND uffv.DestinationRolesCodes = 'PAAPRD';
											
											INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
											VALUES( @TransactionCode, @Transaction_Date, @IDFlow, '', @ActionTime, @UserName, GETDATE(), @UserName );

											UPDATE ExeProductionEntryRelease SET IsLocked = 0 where ProductionEntryCode = @TransactionCode;

											UPDATE ExePlantProductionEntryVerification SET VerifySystem = 0, VerifyManual = 0 WHERE ProductionEntryCode = @TransactionCode;
										END;
									RETURN;
								END;
					END;
				ELSE
					BEGIN
						SELECT TOP 1 @IDFlow = IDFlow, @MessageText = MessageText FROM UtilFlowFunctionView uffv 
						WHERE uffv.SourceFunctionForm = 'EblekReleaseApproval' AND uffv.FunctionName = @ActionButton;
					END;
				BEGIN
					INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
					VALUES( @TransactionCode, @Transaction_Date, @IDFlow, '', @ActionTime, @UserName, GETDATE(), @UserName );
				END;
				RETURN;
			END;
		  IF( @Page LIKE 'TPOFeeActual' )
			 BEGIN
				IF(@TransactionCode IS NULL OR @TransactionCode = '')
				    BEGIN
					   SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_8) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_9);
				    END
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOFeeActual';
			 END;
		  IF( @Page LIKE 'TPOFeeActualDetail' )
			 BEGIN
				IF(@TransactionCode IS NULL OR @TransactionCode = '')
				    BEGIN
					   SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_8) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_9);
				    END
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOFeeActualDetail';
			 END;
		  IF( @Page LIKE 'ApprovalPage' )
			 BEGIN
				IF(@TransactionCode IS NULL OR @TransactionCode = '')
				    BEGIN
					   SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_8) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_9);
				    END
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'ApprovalPage';
			 END;
		   IF ( @Page LIKE 'TPOFeeGL' )
				BEGIN
					SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOFeeGL';
				END;
		   IF( @Page LIKE 'TPOFeeAP' )
			 BEGIN
				IF(@TransactionCode IS NULL OR @TransactionCode = '')
				    BEGIN
					   SET @TransactionCode = CONVERT(NVARCHAR(50), @code_1) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_2) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_3) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_4) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_5) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_6) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_7) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_8) + CONVERT(NVARCHAR(50), @Separator) + CONVERT(NVARCHAR(50), @code_9);
				    END
				SELECT @IDFunct = IDFunction FROM UtilFunctions ufun WHERE ufun.FunctionName LIKE 'TPOFeeAP';
			 END;
        -- insert into transaction log table
        BEGIN
            SELECT @IDButton = IDFunction FROM UtilFunctions ufun WHERE ufun.ParentIDFunction = @IDFunct AND ufun.FunctionName = @ActionButton;
			INSERT INTO @FlowLists (IDFlow, MessageText) SELECT IDFlow, MessageText FROM UtilFlows uflow WHERE uflow.FormSource = @IDFunct AND uflow.ActionButton = @IDButton;
			SELECT @IDFlow = uflow.IDFlow, @MessageText = COALESCE(@MessageText, uflow.MessageText) FROM UtilFlows uflow WHERE uflow.FormSource = @IDFunct AND uflow.ActionButton = @IDButton;
		  if(@Page LIKE 'ProductionCard')
		  BEGIN
			 SELECT @IDButton = IDFunction FROM UtilFunctions ufun WHERE ufun.ParentIDFunction = @IDFunct AND ufun.FunctionName = @ActionButton;
			 INSERT INTO @FlowLists (IDFlow, MessageText) SELECT IDFlow, MessageText FROM UtilFlows uflow WHERE uflow.FormSource = @IDFunct AND uflow.ActionButton = @IDButton AND uflow.DestinationForm  = 143;
			 SELECT @IDFlow = uflow.IDFlow, @MessageText = uflow.MessageText FROM UtilFlows uflow WHERE uflow.FormSource = @IDFunct AND uflow.ActionButton = @IDButton AND uflow.DestinationForm  = 143;
		  END
		  SET @Counter = 0;
		  WHILE (SELECT COUNT(id) FROM @FlowLists) > 0
			BEGIN
				SET @Transaction_Date_New = DATEADD(second, @Counter, @Transaction_Date);
				SET @Counter = @Counter + 1;
				SELECT @IDFlow = IDFlow, @MessageText = COALESCE(@MessageText, MessageText) FROM @FlowLists WHERE id = (SELECT MIN(id) FROM @FlowLists);
				 INSERT INTO UtilTransactionLogs ( TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy )
					VALUES( @TransactionCode, @Transaction_Date_New, @IDFlow, @MessageText, @ActionTime, @UserName, GETDATE(), @UserName );
				DELETE FROM @FlowLists WHERE id = (SELECT MIN(id) FROM @FlowLists);
			END
		END;
    END;