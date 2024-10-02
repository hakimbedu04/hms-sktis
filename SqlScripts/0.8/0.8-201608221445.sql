-- =============================================
-- Description: Recalculate
-- Ticket: http://tp.voxteneo.co.id/entity/3161
-- Author: Dwi Yudha
-- Version: 1.1
-- =============================================

-- =============================================
-- Description: SP for Insert data to ExeReportByGroups TPO
-- Ticket: -
-- Author: Harizal
-- =============================================

-- =============================================
-- Description: recalculate multiskill
-- Ticket: http://tp.voxteneo.co.id/entity/3029
-- Author: Azka
-- Update: 14/03/2016
-- =============================================

-- =============================================
-- Description: Fix wrong calculation valueHour, ValuePeopleHour
-- Ticket: http://tp.voxteneo.co.id/entity/3626
-- Author: Robby
-- Update: 18/03/2016
-- =============================================

-- =============================================
-- Description: change workhour calculation and left join to ExeTPOActualWorkHours (before from ExeActualWorkHours)
-- Ticket: http://tp.voxteneo.co.id/entity/3815
-- Author: Azka
-- Update: 28/03/2016
-- =============================================

-- =============================================
-- Description: change workhour calculation dbo.CalculateWorkHoursTPO
-- Ticket: http://tp.voxteneo.co.id/entity/3815
-- Author: Azka
-- Update: 30/03/2016
-- =============================================

-- =============================================
-- Description: nsert data production convert into stick
-- Ticket: http://tp.voxteneo.co.id/entity/3997
-- Author: Robby
-- Update: 05/04/2016
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

ALTER PROCEDURE [dbo].[InsertTPOExeReportByGroups] 
-- Add the parameters for the stored procedure here
       @LocationCode VARCHAR(50),
       @Brand        VARCHAR(50),
       @Year         INT,
       @Week         INT,
       @Date         DATETIME,
       @CreatedBy    VARCHAR(50)
AS
    BEGIN
		SET DATEFIRST 1

		DELETE FROM ExeReportByGroups
		WHERE LocationCode = @LocationCode AND ProductionDate = @Date AND BrandCode = @Brand;
		
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
       SELECT etev.LocationCode,
              'PROD' UnitCode,
              etev.ProcessGroup,
              et.ProductionGroup GroupCode,
              mgb.BrandGroupCode,
              etev.BrandCode,
              et.StatusEmp,
              etev.ProductionDate,
              mgl.Shift,
              round(et.ActualProduction * pslv.UOMEblek, 0) as Production,
              round(etev.TotalTPKValue * pslv.UOMEblek, 0) as TotalTPKValue, --[dbo].[GetTargetManual]( etev.LocationCode, etev.ProcessGroup, etev.BrandCode, et.ProductionGroup, et.StatusEmp, etev.KPSYear, etev.KPSWeek, etev.ProductionDate ) TPKValue,
               dbo.CalculateWorkHoursTPO(@LocationCode, et.ProductionGroup, @Brand, etev.ProcessGroup, @Date, @Year, @Week, et.StatusEmp, (select datepart(dw, @Date))) WorkHour,
			  --COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7) WorkHour,
              CASE DATEPART(DW, etev.ProductionDate) - 1 WHEN 0 THEN 7 ELSE DATEPART(DW, etev.ProductionDate) - 1 END WeekDay,
              COALESCE(et.Absent, 0) AS Absennce_A,
              0 Absennce_I,
              0 Absennce_C,
              0 Absennce_CH,
              0 Absennce_CT,
              0 Absennce_SLS,
              0 Absennce_SLP,
              0 Absennce_ETC,
              0 Multi_TPO,
              0 Multi_ROLL,
              0 Multi_CUT,
              0 Multi_PACK,
              0 Multi_STAMP,
              0 Multi_FWRP,
              0 Multi_SWRP,
              0 Multi_GEN,
              0 Multi_WRP,
              0 [Out],
              et.WorkerCount - et.Absent Attend,
              GETDATE() createdDate,
              @CreatedBy CreatedBy,
              GETDATE() UpdatedDate,
              @CreatedBy UpdatedBy,
              et.WorkerCount Register,
              et.WorkerCount - et.Absent [In],
              etev.KPSYear,
              etev.KPSWeek,
			  CASE WHEN dbo.CalculateWorkHoursTPO(@LocationCode, et.ProductionGroup, @Brand, etev.ProcessGroup, @Date, @Year, @Week, et.StatusEmp, (select datepart(dw, @Date))) != 0 THEN COALESCE(et.ActualProduction/NULLIF(dbo.CalculateWorkHoursTPO(@LocationCode, et.ProductionGroup, @Brand, etev.ProcessGroup, @Date, @Year, @Week, et.StatusEmp, (select datepart(dw, @Date))),0), 0) ELSE 0 END ValueHour,
			  --CASE WHEN COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7) != 0 THEN COALESCE(et.ActualProduction/(COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7)), 0) ELSE 0 END ValueHour,
			  CASE
				--WHEN et.WorkerCount - dbo.GetAbsent( etev.ProductionEntryCode, 'MO' ) != 0 AND et.ActualProduction != 0
				WHEN et.WorkerCount - COALESCE(et.Absent, 0) != 0 AND et.ActualProduction != 0
				THEN COALESCE(et.ActualProduction / NULLIF((et.WorkerCount - et.Absent), 0), 0) 
				ELSE 0 
				END ValuePeople,
			  CASE
				--WHEN COALESCE(et.ActualProduction / NULLIF((et.WorkerCount - et.Absent), 0), 0) != 0 AND COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7) != 0
				--THEN COALESCE(et.ActualProduction / NULLIF((et.WorkerCount - et.Absent), 0), 0) / COALESCE(DATEDIFF(HOUR, ( DATEADD(MINUTE, (( DATEPART(HOUR, eawh.TimeIn) * 60 ) + DATEPART(MINUTE, eawh.TimeIn) * 60 ) + DATEPART(SECOND, eawh.TimeIn), eawh.BreakTime)), eawh.TimeOut), 7)
				WHEN COALESCE(et.ActualProduction / NULLIF((et.WorkerCount - et.Absent), 0), 0) != 0 AND dbo.CalculateWorkHoursTPO(@LocationCode, et.ProductionGroup, @Brand, etev.ProcessGroup, @Date, @Year, @Week, et.StatusEmp, (select datepart(dw, @Date))) != 0
				THEN COALESCE((COALESCE(et.ActualProduction / NULLIF((et.WorkerCount - et.Absent), 0), 0) / NULLIF(dbo.CalculateWorkHoursTPO(@LocationCode, et.ProductionGroup, @Brand, etev.ProcessGroup, @Date, @Year, @Week, et.StatusEmp, (select datepart(dw, @Date))), 0)), 0)
				ELSE 0 
				END ValuePeopleHour,
			  et.WorkerCount - et.Absent ActualWorker
       FROM dbo.ExeTPOProductionEntryVerification etev
            INNER JOIN dbo.ExeTPOProduction et ON et.ProductionEntryCode = etev.ProductionEntryCode
            INNER JOIN dbo.MstGenBrand mgb ON mgb.BrandCode = etev.BrandCode
            INNER JOIN dbo.MstGenLocation mgl ON mgl.LocationCode = etev.LocationCode
			INNER JOIN dbo.ProcessSettingsAndLocationView pslv 
				ON pslv.BrandGroupCode = mgb.BrandGroupCode
				AND pslv.LocationCode = etev.LocationCode
				AND pslv.ProcessGroup = etev.ProcessGroup
            --LEFT JOIN dbo.ExeTPOActualWorkHours eawh ON eawh.LocationCode = etev.LocationCode
                                                 --AND eawh.UnitCode = 'PROD'
                                                 --AND eawh.ProductionDate = etev.ProductionDate
                                                 --AND eawh.BrandCode = etev.BrandCode
												 --AND eawh.ProcessGroup = etev.ProcessGroup
												 --AND eawh.StatusEmp = et.StatusEmp
       WHERE etev.LocationCode = @LocationCode
         AND etev.BrandCode = @Brand
         AND etev.KPSYear = @Year
         AND etev.KPSWeek = @Week
         AND etev.ProductionDate = @Date;

		--EXEC [dbo].[RunSSISProductionReportByGroupMonthly]
		--EXEC [dbo].[RunSSISProductionReportByGroupWeekly]
		exec RECALCULATEREPORTBYGROUPWEEKLYMONTHLYNOGROUP @LocationCode, @Brand, @Week, @Year, @Date
    END;

