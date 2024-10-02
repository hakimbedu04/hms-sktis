-- =============================================
-- Description: Add   EXEC RunSSISProductionReportByGroupMonthly,   EXEC RunSSISProductionReportByGroupWeekly
-- Ticket: http://tp.voxteneo.co.id/entity/3038
-- Author: AZKA
-- =============================================


-- =============================================
-- Description: Insert dummy for multiskillout
-- Ticket: http://tp.voxteneo.co.id/entity/3037
-- Author: Yudha
-- Update: 1.3 - 3/1/2016
-- =============================================
-- =============================================
-- Description: Alter SP for Insert data to ExeReportByGroups Plant
-- Ticket: http://tp.voxteneo.co.id/entity/3019
-- Author: ARDI
-- Update: 1.2 - 2/15/2016
-- =============================================
-- =============================================
-- Description: Alter SP for Insert data to ExeReportByGroups Plant
-- Ticket: http://tp.voxteneo.co.id/entity/3031
-- Author: AZKA
-- Update: 1.1 - 1/27/2016
-- =============================================

-- =============================================
-- Description: SP for Insert data to ExeReportByGroups Plant
-- Ticket: -
-- Author: Jejen Suhendar
-- Update: 1.0
-- =============================================

-- =============================================
-- Description: Update ValueHour, ValuePeople, ValuePeopleHour
-- Ticket: -
-- Author: Robby Prima Suherlan
-- Update: 1.0
-- =============================================

-- =============================================
-- Description: calculate multiskillout
-- Ticket: http://tp.voxteneo.co.id/entity/3029
-- Author: Azka
-- Update: 14/03/2016
-- =============================================

-- =============================================
-- Description: change workhour calculation
-- Ticket: http://tp.voxteneo.co.id/entity/3815
-- Author: Azka
-- Update: 28/03/2016
-- =============================================

-- =============================================
-- Description: change workhour calculation dbo.CalculateWorkHours
-- Ticket: http://tp.voxteneo.co.id/entity/3815
-- Author: Azka
-- Update: 30/03/2016
-- =============================================

-- =============================================
-- Description: edit update verify system to 1
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 05/04/2016
-- =============================================

-- =============================================
-- Description: nsert data production convert into stick
-- Ticket: http://tp.voxteneo.co.id/entity/3997
-- Author: Robby
-- Update: 05/04/2016
-- =============================================

-- =============================================
-- Description: update verification, verify system to 1 on dummy group
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 11/04/2016
-- =============================================

-- =============================================
-- Description: Delete calculation at column "In" delete(SLP,SLS)
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Ardi
-- Update: 21/04/2016
-- =============================================

-- =============================================
-- Description: Edit TPKValue
-- Ticket: http://tp.voxteneo.co.id/entity/3045
-- Author: AZKA
-- Updated: 4/27/2016
-- =============================================

-- =============================================
-- Description: round Production and TPKValue
-- Author: AZKA
-- Updated: 4/27/2016
-- =============================================

-- =============================================
-- Description: round Production and TPKValue version 2
-- Author: AZKA
-- Updated: 5/03/2016
-- =============================================

-- =============================================
-- Description: round Production and TPKValue version 3
-- Author: AZKA
-- Updated: 5/04/2016
-- =============================================

-- =============================================
-- Description: Mengurangi Absent S untuk [In] http://tp.voxteneo.co.id/entity/7004
-- Author: Indra
-- Updated: 9/6/2016
-- =============================================

ALTER PROCEDURE [dbo].[InsertPlantExeReportByGroups] 
-- Add the parameters for the stored procedure here
       @LocationCode VARCHAR(50),
       @Unit         VARCHAR(50),
       @Brand        VARCHAR(50),
       @Shift        INT,
       @Year         INT,
       @Week         INT,
       @Date         DATETIME,
	   @GroupCode	 VARCHAR(50),
	  @CreatedBy	 varchar(50)
AS
    BEGIN
		DECLARE @ProductionCode VARCHAR(200)
		DECLARE @TotalActualValue REAL
		SET DATEFIRST 1

		 SET @ProductionCode = 'EBL/' + @LocationCode 
         + '/' + CONVERT(varchar,@Shift) 
         + '/' + @Unit 
         + '/' + @GroupCode 
         + '/' + @Brand 
         + '/' + CONVERT(varchar,@Year) 
         + '/' + CONVERT(varchar,@Week)
         + '/' + CONVERT(varchar,(select datepart(dw, @Date)));
		
		SELECT @TotalActualValue = SUM(en.ProdActual) FROM ExePlantProductionEntry en 
		WHERE en.ProductionEntryCode = @ProductionCode;

		UPDATE ExePlantProductionEntryVerification SET TotalActualValue = @TotalActualValue, VerifySystem = 1
		WHERE ProductionEntryCode = @ProductionCode;

		DELETE FROM ExeReportByGroups
		WHERE LocationCode = @LocationCode AND GroupCode = @GroupCode AND ProductionDate = @Date AND BrandCode = @Brand;

        INSERT INTO dbo.ExeReportByGroups
               ( LocationCode,
                 UnitCode,
                 ProcessGroup,
                 GroupCode,
                 BrandGroupCode,
                 BrandCode,
                 StatusEmp,
                 ProductionDate,
                 Shift,
                 Production,
                 TPKValue,
                 WorkHour,
                 WeekDay,
                 Absennce_A,
                 Absence_I,
                 Absence_C,
                 Absence_CH,
                 Absence_CT,
                 Absence_SLS,
                 Absence_SLP,
                 Absence_ETC,
                 Multi_TPO,
                 Multi_ROLL,
                 Multi_CUTT,
                 Multi_PACK,
                 Multi_STAMP,
                 Multi_FWRP,
                 Multi_SWRP,
                 Multi_GEN,
                 Multi_WRP,
                 Out,
                 Attend,
                 CreatedDate,
                 CreatedBy,
                 UpdatedDate,
                 UpdatedBy,
                 Register,
                 EmpIn,
                 KPSYear,
                 KPSWeek,
				 ValueHour,
				 ValuePeople,
				 ValuePeopleHour,
				 ActualWorker,
				 Absence_S
               )
               SELECT eppev.LocationCode,
                      eppev.UnitCode,
                      eppev.ProcessGroup,
                      eppev.GroupCode,
                      mgb.BrandGroupCode,
                      eppev.BrandCode,
                      mges.StatusEmp,
                      eppev.ProductionDate,
                      eppev.Shift,
                      round(eppev.TotalActualValue * pslv.UOMEblek, 0) as Production,
                      round(eppev.TPKValue * pslv.UOMEblek, 0) as TPKValue,
					  dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))) WorkHour,
                      --COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7) WorkHour,
                      CASE DATEPART(DW, eppev.ProductionDate) - 1 WHEN 0 THEN 7 ELSE DATEPART(DW, eppev.ProductionDate) - 1 END WeekDay,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'A' ) Absence_A,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'I' ) Absence_I,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'C' ) Absence_C,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'CH' ) Absence_CH,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'CT' ) Absence_CT,
					  [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLS' ) Absence_SLS,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLP' ) Absence_SLP,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, NULL ) Absence_ETC,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'TPO', 'MO') AS Multi_TPO,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'ROLLING', 'MO') AS Multi_ROLL,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'CUTTING', 'MO') AS  Multi_CUT,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'PACKING', 'MO') AS  Multi_PACK,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'STAMPING', 'MO') AS  Multi_STAMP,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'FOILROLL', 'MO') AS  Multi_FWRP,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'STICKWRAPPING', 'MO') AS  Multi_SWRP,
                      0 Multi_GEN,
                      [dbo].GetAbsentMultiskillOut(eppev.ProductionEntryCode, @LocationCode, @Shift, @Unit, @GroupCode, @Brand, 'WRAPPING', 'MO') AS  Multi_WRP,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'T' ) [Out],
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'A' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'I' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'C' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CH' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CT' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLS' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLP' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, NULL ) [Attend],
                      GETDATE() CreatedDate,
                      @CreatedBy CreatedBy,
                      GETDATE() UpdatedDate,
                      @CreatedBy UpdatedDate,
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) Register,
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'A' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'I' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'C' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CH' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CT' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, NULL ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'T' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'S' ) AS [In],
                      eppev.KPSYear,
                      eppev.KPSWeek,
					  CASE WHEN WorkHour != 0
						THEN COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))), 0), 0)
						--THEN COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7), 0), 0)
						ELSE 0
						END ValueHour,
					  CASE WHEN [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' ) != 0 AND eppev.TotalActualValue != 0
						THEN COALESCE(
							CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) AS FLOAT) , 0), 0)
						ELSE 0
						END ValuePeople,
						(COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) as FLOAT), 0), 0)) / NULLIF(dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))), 0) ValuePeopleHour,
						--(COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) as FLOAT), 0), 0)) / NULLIF(COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7), 0) ValuePeopleHour,
					  [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' ) ActualWorker,
					  [dbo].GetAbsent( eppev.ProductionEntryCode, 'S' ) Absence_S
               FROM dbo.ExePlantProductionEntryVerification eppev
                    INNER JOIN dbo.MstGenBrand mgb ON mgb.BrandCode = eppev.BrandCode
					INNER JOIN dbo.ProcessSettingsAndLocationView pslv 
						ON pslv.BrandGroupCode = mgb.BrandGroupCode
						AND pslv.LocationCode = eppev.LocationCode
						AND pslv.ProcessGroup = eppev.ProcessGroup
                    --LEFT JOIN dbo.ExeActualWorkHours eawh ON eawh.LocationCode = eppev.LocationCode
                                                         --AND eawh.UnitCode = eppev.UnitCode
                                                         --AND eawh.Shift = eppev.Shift
                                                         --AND eawh.ProductionDate = eppev.ProductionDate
                                                         --AND eawh.ProcessGroup = eppev.ProcessGroup
                                                         --AND eawh.BrandCode = eppev.BrandCode
                    LEFT JOIN dbo.MstGenEmpStatus mges ON SUBSTRING(eppev.GroupCode, 2, 1) = mges.StatusIdentifier
               WHERE eppev.LocationCode = @LocationCode
                 AND eppev.KPSYear = @Year
                 AND eppev.KPSWeek = @Week
                 AND eppev.ProductionDate = @Date
                 AND eppev.Shift = @Shift
                 AND eppev.UnitCode = @Unit
                 AND eppev.BrandCode = @Brand
				 AND eppev.GroupCode = @GroupCode;
				 
		-- DUMMY GROUP INSERT --
		DECLARE @isDummyExist INT;
		DECLARE @dummyGroupCode VARCHAR(4);
		
		SET @dummyGroupCode = SUBSTRING(@GroupCode, 1, 1) + '5' + SUBSTRING(@GroupCode, 3, 2);
		
		SELECT @isDummyExist = COUNT(*)
               FROM dbo.ExePlantProductionEntryVerification eppev
               WHERE eppev.LocationCode = @LocationCode
                 AND eppev.KPSYear = @Year
                 AND eppev.KPSWeek = @Week
                 AND eppev.ProductionDate = @Date
                 AND eppev.Shift = @Shift
                 AND eppev.UnitCode = @Unit
                 AND eppev.BrandCode = @Brand
				 AND eppev.GroupCode = @dummyGroupCode;
				 
		IF (@isDummyExist > 0)
		BEGIN
			DELETE FROM ExeReportByGroups
			WHERE LocationCode = @LocationCode AND GroupCode = @dummyGroupCode AND ProductionDate = @Date AND BrandCode = @Brand;

			UPDATE ExePlantProductionEntryVerification SET TotalActualValue = @TotalActualValue, VerifySystem = 1
			WHERE LocationCode = @LocationCode
                 AND KPSYear = @Year
                 AND KPSWeek = @Week
                 AND ProductionDate = @Date
                 AND Shift = @Shift
                 AND UnitCode = @Unit
                 AND BrandCode = @Brand
				 AND GroupCode = @dummyGroupCode;

			INSERT INTO dbo.ExeReportByGroups
               ( LocationCode,
                 UnitCode,
                 ProcessGroup,
                 GroupCode,
                 BrandGroupCode,
                 BrandCode,
                 StatusEmp,
                 ProductionDate,
                 Shift,
                 Production,
                 TPKValue,
                 WorkHour,
                 WeekDay,
                 Absennce_A,
                 Absence_I,
                 Absence_C,
                 Absence_CH,
                 Absence_CT,
                 Absence_SLS,
                 Absence_SLP,
                 Absence_ETC,
                 Multi_TPO,
                 Multi_ROLL,
                 Multi_CUTT,
                 Multi_PACK,
                 Multi_STAMP,
                 Multi_FWRP,
                 Multi_SWRP,
                 Multi_GEN,
                 Multi_WRP,
                 Out,
                 Attend,
                 CreatedDate,
                 CreatedBy,
                 UpdatedDate,
                 UpdatedBy,
                 Register,
                 EmpIn,
                 KPSYear,
                 KPSWeek,
				 ValueHour,
				 ValuePeople,
				 ValuePeopleHour,
				 ActualWorker
               )
               SELECT eppev.LocationCode,
                      eppev.UnitCode,
                      eppev.ProcessGroup,
                      eppev.GroupCode,
                      mgb.BrandGroupCode,
                      eppev.BrandCode,
                      mges.StatusEmp,
                      eppev.ProductionDate,
                      eppev.Shift,
                      round(eppev.TotalActualValue, 2) * pslv.UOMEblek as Production,
                      round(eppev.TPKValue, 2) * pslv.UOMEblek as TPKValue,
					  dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))) WorkHour,
                      --COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7) WorkHour,
                      CASE DATEPART(DW, eppev.ProductionDate) - 1 WHEN 0 THEN 7 ELSE DATEPART(DW, eppev.ProductionDate) - 1 END WeekDay,
                      0 AS Absence_A,
                      0 AS Absence_I,
                      0 AS Absence_C,
                      0 AS Absence_CH,
                      0 AS Absence_CT,
					  0 AS Absence_SLS,
                      0 AS Absence_SLP,
                      0 AS Absence_ETC,
                      0 AS Multi_TPO,
                      0 AS Multi_ROLL,
                      0 AS Multi_CUT,
                      0 AS Multi_PACK,
                      0 AS Multi_STAMP,
                      0 AS Multi_FWRP,
                      0 AS Multi_SWRP,
                      0 AS Multi_GEN,
                      0 AS Multi_WRP,
                      [dbo].GetAbsent( eppev.ProductionEntryCode, 'T' ) [Out],
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'A' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'I' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'C' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CH' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CT' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLS' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'SLP' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, NULL ) [Attend],
                      GETDATE() CreatedDate,
                      @CreatedBy CreatedBy,
                      GETDATE() UpdatedDate,
                      @CreatedBy UpdatedDate,
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) Register,
                      [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'A' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'I' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'C' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CH' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'CT' ) - [dbo].GetAbsent( eppev.ProductionEntryCode, NULL ) - [dbo].GetAbsent( eppev.ProductionEntryCode, 'T' ) AS [In],
                      eppev.KPSYear,
                      eppev.KPSWeek,
					 CASE WHEN WorkHour != 0
						THEN COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))), 0), 0)
						--THEN COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7), 0), 0)
						ELSE 0
						END ValueHour,
					  CASE WHEN [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' ) != 0 AND eppev.TotalActualValue != 0
						THEN COALESCE(
							CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) AS FLOAT) , 0), 0)
						ELSE 0
						END ValuePeople,
						(COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) as FLOAT), 0), 0)) / NULLIF(dbo.CalculateWorkHours(@LocationCode, @Unit, @GroupCode, @Brand, @Shift, eppev.ProcessGroup, @Date, @Year, @Week, (select datepart(dw, @Date))), 0) ValuePeopleHour,
						--(COALESCE(CAST(eppev.TotalActualValue AS FLOAT) / NULLIF(CAST(([dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' )) as FLOAT), 0), 0)) / NULLIF(COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7), 0) ValuePeopleHour,
					  [dbo].GetEmployeeEblekCount(eppev.ProductionEntryCode) - dbo.GetAbsent( eppev.ProductionEntryCode, 'MO' ) ActualWorker
               FROM dbo.ExePlantProductionEntryVerification eppev
                    INNER JOIN dbo.MstGenBrand mgb ON mgb.BrandCode = eppev.BrandCode
					INNER JOIN dbo.ProcessSettingsAndLocationView pslv 
						ON pslv.BrandGroupCode = mgb.BrandGroupCode
						AND pslv.LocationCode = eppev.LocationCode
						AND pslv.ProcessGroup = eppev.ProcessGroup
                    --LEFT JOIN dbo.ExeActualWorkHours eawh ON eawh.LocationCode = eppev.LocationCode
                                                         --AND eawh.UnitCode = eppev.UnitCode
                                                         --AND eawh.Shift = eppev.Shift
                                                         --AND eawh.ProductionDate = eppev.ProductionDate
                                                         --AND eawh.ProcessGroup = eppev.ProcessGroup
                                                         --AND eawh.BrandCode = eppev.BrandCode
                    LEFT JOIN dbo.MstGenEmpStatus mges ON SUBSTRING(eppev.GroupCode, 2, 1) = mges.StatusIdentifier
               WHERE eppev.LocationCode = @LocationCode
                 AND eppev.KPSYear = @Year
                 AND eppev.KPSWeek = @Week
                 AND eppev.ProductionDate = @Date
                 AND eppev.Shift = @Shift
                 AND eppev.UnitCode = @Unit
                 AND eppev.BrandCode = @Brand
				 AND eppev.GroupCode = @dummyGroupCode;
		END;

		EXEC [dbo].[RunSSISProductionReportByGroupMonthly]
		EXEC [dbo].[RunSSISProductionReportByGroupWeekly]

    END;





GO


