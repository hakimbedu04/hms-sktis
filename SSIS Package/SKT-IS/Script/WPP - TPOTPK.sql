------ TPO Target Production Group
Select loc.LocationCode, 
		prod.ProcessGroup,
		prod.ProdGroup,
		prod.WorkerCount As WorkerRegister,
		prod.WorkerCount As WorkerAvailable,
		prod.WorkerCount As WorkerAlocation,
		prod.StatusEmp
from	MstGenLocation loc 
		inner join MstTPOProductionGroup prod on loc.LocationCode = prod.LocationCode
where	loc.StatusActive =1 and prod.StatusActive = 1

go		
------------------------------------------------------------------------------------------------------------------------------------------------
----WPP
select	max(StartDate) as TPKTPOStartProductionDate,
		max(KPSYear)as KPSYear,
		max(KPSWeek) as KPSWeek,
		LocationCode,
		BrandCode,		
		max(Value1) as WPPValue
from	PlanTmpWeeklyProductionPlanning planwpp
		inner join MstGenWeek as genWeek on genWeek.Week = planwpp.KPSWeek and genWeek.Year = planwpp.KPSYear
group by	LocationCode,BrandCode

go		
------------------------------------------------------------------------------------------------------------------------------------------------
----Historical

Declare @startdate date 
Declare @endate date 

SELECT	@startdate = min(StartDate) ,@endate = max(enddate) 
FROM	GetPastWeek(cast(2015 as int),cast(41 as int),3) past 
		inner join MstGenWeek week on week.Year = past.year and week.Week = past.week 

select	ExeTPOProduction.ProdGroup,ExeTPOProduction.BrandCode,MstGenProcess.ProcessGroup,status as StatusEmp,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 2 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 2 then WorkerCount end)) * 100 as PercentAttendance1,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 3 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 3 then WorkerCount end)) * 100 as PercentAttendance2,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 4 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 4 then WorkerCount end)) * 100 as PercentAttendance3,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 5 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 5 then WorkerCount end)) * 100 as PercentAttendance4,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 6 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 6 then WorkerCount end)) * 100 as PercentAttendance5,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 7 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 7 then WorkerCount end)) * 100 as PercentAttendance6,
		(SUM(case when datepart(dw,TPOProductionEntryDate) = 1 then cast((WorkerCount - Absent) as decimal) end) / SUM(case when datepart(dw,TPOProductionEntryDate) = 1 then WorkerCount end)) * 100 as PercentAttendance7,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 2 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker1,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 3 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker2,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 4 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker3,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 5 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker4,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 6 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker5,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 7 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker6,
		SUM(case when datepart(dw,TPOProductionEntryDate) = 1 then cast(ActualProduction as decimal) end) as HistoricalCapacityWorker7 
from	ExeTPOProduction inner join MstGenProcess on ExeTPOProduction.Process = MstGenProcess.ProcessIdentifier 
where	ExeTPOProduction.TPOProductionEntryDate >= @startdate and ExeTPOProduction.TPOProductionEntryDate <= @endate
group by ExeTPOProduction.ProdGroup,ExeTPOProduction.BrandCode,MstGenProcess.ProcessGroup,status

go
------------------------------------------------------------------------------------------------------------------------------------------------
----Historical WorkHour

Declare @startdate date 
Declare @endate date 

SELECT @startdate = min(StartDate) ,@endate = max(enddate) 
FROM GetPastWeek(cast(2015 as int),cast(41 as int),3) past inner join MstGenWeek week on week.Year = past.year and week.Week = past.week 

SELECT     ProdGroup, BrandCode, ProcessGroup, StatusEmp, 
		   sum(ProcessWorkHours1) As ProcessWorkHours1,
		   sum(ProcessWorkHours2) As ProcessWorkHours2, 
		   sum(ProcessWorkHours3) As ProcessWorkHours3, 
		   sum(ProcessWorkHours4) As ProcessWorkHours4, 
		   sum(ProcessWorkHours5) As ProcessWorkHours5,
		   sum(ProcessWorkHours6) As ProcessWorkHours6, 
		   sum(ProcessWorkHours7) As ProcessWorkHours7
FROM	PlanTPOTargetProductionKelompok
where TPKTPOStartProductionDate >= @startdate and TPKTPOStartProductionDate <= @endate 
group by ProdGroup, BrandCode, ProcessGroup, StatusEmp