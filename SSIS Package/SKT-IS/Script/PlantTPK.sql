---------------------------------------------------------------------------------------------------------------------------------------------
---------------Plant Target Production Group
declare @startdate datetime
declare @enddate datetime

select @startdate = StartDate,@enddate = EndDate from MstGenWeek where Year = 2015 and Week = 38
select	distinct pg.LocationCode,
		pg.UnitCode,
		ls.Shift,
		pg.GroupCode,
		pg.ProcessGroup,
		pg.WorkerCount,
		isnull(absn.AbsnCount,0) AbsnCount
from	MstPlantProductionGroup pg 
		INNER JOIN GetLocationShift() ls on pg.LocationCode = ls.LocationCode
		inner join MstGenLocation loc on pg.LocationCode = loc.LocationCode
		inner join MstPlantUnit un on pg.UnitCode = un.UnitCode
		left outer join (select	emp.LocationCode,emp.GroupCode,emp.ProcessSettingsCode, count(*) absncount
						from	ExePlantWorkerAbsenteeism  absent
								inner join MstPlantEmpJobsDataAcv emp on absent.EmployeeID = emp.EmployeeID 
						where	(absent.StartDateAbsent  >= @startdate and  absent.EndDate <= @enddate )
						group by emp.LocationCode,emp.GroupCode,emp.ProcessSettingsCode) as absn on pg.GroupCode = absn.GroupCode and pg.ProcessGroup = absn.ProcessSettingsCode and pg.LocationCode = absn.LocationCode
where	loc.StatusActive = 1 and un.StatusActive = 1  and
		pg.LocationCode = 'ID21' and ls.Shift = 1
		
go	
---------------------------------------------------------------------------------------------------------------------------------------------
---------------Plant Target Production Group
	
select ProductionStartDate
      ,KPSYear
      ,KPSWeek
      ,BrandCode
      ,LocationCode
      ,UnitCode
      ,Shift
      ,TargetManual1
      ,TargetManual2
      ,TargetManual3
      ,TargetManual4
      ,TargetManual5
      ,TargetManual6
      ,TargetManual7
      
from PlanTargetProductionUnit
where	KPSYear = 2015 and KPSWeek = 38 and 
		LocationCode = 'ID21' and BrandCode = 'FA010783.15' and Shift = 1
		
go		
---------------------------------------------------------------------------------------------------------------------------------------------
---------------Plant Target Production Group
Declare @startdate date 
Declare @endate date 

SELECT	@startdate = min(StartDate),@endate = max(enddate) 
FROM	GetPastWeek(cast(2015 as int),cast(38 as int),3) past 
		inner join MstGenWeek week on week.Year = past.year and week.Week = past.week 

select	LocationCode,UnitCode,Shift,BrandCode,groupcode,ProcessGroup ,
		SUM(case when datepart(dw,ProductionDate) = 2 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 2 then WorkHours end) as HistoricalCapacityWorker1,
		SUM(case when datepart(dw,ProductionDate) = 3 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 3 then WorkHours end) as HistoricalCapacityWorker2,
		SUM(case when datepart(dw,ProductionDate) = 4 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 4 then WorkHours end) as HistoricalCapacityWorker3,
		SUM(case when datepart(dw,ProductionDate) = 5 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 5 then WorkHours end) as HistoricalCapacityWorker4,
		SUM(case when datepart(dw,ProductionDate) = 6 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 6 then WorkHours end) as HistoricalCapacityWorker5,
		SUM(case when datepart(dw,ProductionDate) = 7 then production + UpahLain end)/SUM(case when datepart(dw,ProductionDate) = 7 then WorkHours end) as HistoricalCapacityWorker6,
		1 as HistoricalCapacityWorker7,
		100*SUM(case when datepart(dw,ProductionDate) = 2 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 2 then 1 else 0 end) as absent1,
		100*SUM(case when datepart(dw,ProductionDate) = 3 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 3 then 1 else 0 end) as absent2,
		100*SUM(case when datepart(dw,ProductionDate) = 4 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 4 then 1 else 0 end) as absent3,
		100*SUM(case when datepart(dw,ProductionDate) = 5 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 5 then 1 else 0 end) as absent4,
		100*SUM(case when datepart(dw,ProductionDate) = 6 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 6 then 1 else 0 end) as absent5,
		100*SUM(case when datepart(dw,ProductionDate) = 7 and Production + UpahLain > 0 then 1 else 0 end)/SUM(case when datepart(dw,ProductionDate) = 7 then 1 else 0 end) as absent6,
		1 as absent7  from productioncard 
where ProductionDate >= @startdate and ProductionDate <= @endate and LocationCode = 'ID21' and Shift = 1 and BrandCode = 'FA010783.15' 
group by LocationCode,UnitCode,Shift,BrandCode,groupcode,ProcessGroup