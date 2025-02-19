
-- =============================================
-- Description: create table tempcaculatebobottpu
-- Ticket: http://tp.voxteneo.co.id/entity/8605
-- Author: Sidiq
-- Update: 2016/08/12
-- =============================================

CREATE TABLE [dbo].[tempcaculatebobottpu](
	[ProductionStartDate] [datetime] NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[BrandCode] [varchar](11) NULL,
	[LocationCode] [varchar](8) NULL,
	[UnitCode] [varchar](4) NULL,
	[shift] [int] NULL,
	[WPP] [decimal](16, 4) NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcessWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
	[TPUCode] [varchar](256) NULL,
	[Attendance1] [varchar](20) NULL,
	[Attendance2] [varchar](20) NULL,
	[Attendance3] [varchar](20) NULL,
	[Attendance4] [varchar](20) NULL,
	[Attendance5] [varchar](20) NULL,
	[Attendance6] [varchar](20) NULL,
	[Attendance7] [varchar](20) NULL,
	[WorkerRegister] [varchar](20) NULL,
	[WorkerAvailable] [varchar](20) NULL,
	[WorkerAlocation] [varchar](20) NULL,
	[ProcessGroup] [varchar](20) NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[DefaultProductivity] [float] NULL,
	[ActualProduction1] [float] NULL,
	[ActualProduction2] [float] NULL,
	[ActualProduction3] [float] NULL,
	[ActualProduction4] [float] NULL,
	[ActualProduction5] [float] NULL,
	[ActualProduction6] [float] NULL,
	[ActualProduction7] [float] NULL,
	[ActualWorkhour1] [varchar](20) NULL,
	[ActualWorkhour2] [varchar](20) NULL,
	[ActualWorkhour3] [varchar](20) NULL,
	[ActualWorkhour4] [varchar](20) NULL,
	[ActualWorkhour5] [varchar](20) NULL,
	[ActualWorkhour6] [varchar](20) NULL,
	[ActualWorkhour7] [varchar](20) NULL,
	[Productivity1] [float] NULL,
	[Productivity2] [float] NULL,
	[Productivity3] [float] NULL,
	[Productivity4] [float] NULL,
	[Productivity5] [float] NULL,
	[Productivity6] [float] NULL,
	[Productivity7] [float] NULL,
	[PercentAttendance1] [float] NULL,
	[PercentAttendance2] [float] NULL,
	[PercentAttendance3] [float] NULL,
	[PercentAttendance4] [float] NULL,
	[PercentAttendance5] [float] NULL,
	[PercentAttendance6] [float] NULL,
	[PercentAttendance7] [float] NULL,
	[Bobot1] [float] NULL,
	[Bobot2] [float] NULL,
	[Bobot3] [float] NULL,
	[Bobot4] [float] NULL,
	[Bobot5] [float] NULL,
	[Bobot6] [float] NULL,
	[Bobot7] [float] NULL
) ON [PRIMARY]