------ Unit
declare @startdate datetime
declare @enddate datetime
select @startdate = StartDate,@enddate = EndDate from MstGenWeek where Year = 2015 and Week = 41
select	un.LocationCode,
		un.UnitCode,
		ls.shift,
		count(1) as register,
		max(isnull( absn.absncount,0) )as absent
FROM	MstPlantUnit AS un		
		left outer join MstPlantEmpJobsDataAcv emp on un.LocationCode = emp.LocationCode and un.UnitCode = emp.UnitCode and emp.ProcessSettingsCode = 'ROLLING'
		LEFT OUTER JOIN (select	emp.UnitCode, count(*) absncount
						from	ExePlantWorkerAbsenteeism  absent
								inner join MstPlantEmpJobsDataAcv emp on absent.EmployeeID = emp.EmployeeID 
						where	emp.ProcessSettingsCode ='ROLLING' and (absent.StartDateAbsent  >= @startdate and  absent.EndDate <= @enddate )
						group by emp.UnitCode) absn on un.UnitCode = absn.UnitCode
		INNER JOIN GetLocationShift() ls on ls.locationcode = un.LocationCode

where un.StatusActive = 1
group by un.LocationCode,
		un.unitCode,ls.shift
		
go		
------------------------------------------------------------------------------------------------------------------------------------------------
----WPP
select	max(StartDate) as ProductionStartDate,
		max(KPSYear)as KPSYear,
		max(KPSWeek) as KPSWeek,
		LocationCode,
		BrandCode,		
		max(Value1) as WPP
from	PlanTmpWeeklyProductionPlanning planwpp
		inner join MstGenWeek as genWeek on genWeek.Week = planwpp.KPSWeek and genWeek.Year = planwpp.KPSYear
group by	LocationCode,BrandCode


go		
------------------------------------------------------------------------------------------------------------------------------------------------
----Historical

Declare @startdate date 
Declare @endate date 
SELECT	@startdate = min(StartDate),@endate = max(enddate) 
FROM	GetPastWeek(cast(2015 as int),cast(41 as int),3) past 
		inner join MstGenWeek week on week.Year = past.year and week.Week = past.week 
select	LocationCode,UnitCode,Shift,max(Brandcode) as BrandCode,
		SUM(case when datepart(dw,ProductionDate) = 2 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity1,
		SUM(case when datepart(dw,ProductionDate) = 3 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity2,
		SUM(case when datepart(dw,ProductionDate) = 4 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity3,
		SUM(case when datepart(dw,ProductionDate) = 5 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity4,
		SUM(case when datepart(dw,ProductionDate) = 6 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity5,
		SUM(case when datepart(dw,ProductionDate) = 7 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity6,
		SUM(case when datepart(dw,ProductionDate) = 1 and processgroup = 'ROLLING' and production + UpahLain>0  then production + UpahLain else 0 end) AS Capacity7,
		SUM(case when datepart(dw,ProductionDate) = 2 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours1,
		SUM(case when datepart(dw,ProductionDate) = 3 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours2,
		SUM(case when datepart(dw,ProductionDate) = 4 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours3,
		SUM(case when datepart(dw,ProductionDate) = 5 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours4,
		SUM(case when datepart(dw,ProductionDate) = 6 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours5,
		SUM(case when datepart(dw,ProductionDate) = 7 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours6,
		SUM(case when datepart(dw,ProductionDate) = 1 and processgroup =  'ROLLING'  and production + UpahLain>0 then WorkHours else 0 end) as WorkHours7,
		100*SUM(case when datepart(dw,ProductionDate) = 2 and Production + UpahLain > 0 then 1 else 0 end) as production1,
		100*SUM(case when datepart(dw,ProductionDate) = 3 and Production + UpahLain > 0 then 1 else 0 end) as production2,
		100*SUM(case when datepart(dw,ProductionDate) = 4 and Production + UpahLain > 0 then 1 else 0 end) as production3,
		100*SUM(case when datepart(dw,ProductionDate) = 5 and Production + UpahLain > 0 then 1 else 0 end) as production4,
		100*SUM(case when datepart(dw,ProductionDate) = 6 and Production + UpahLain > 0 then 1 else 0 end) as production5,
		100*SUM(case when datepart(dw,ProductionDate) = 7 and Production + UpahLain > 0 then 1 else 0 end) as production6,
		100*SUM(case when datepart(dw,ProductionDate) = 1 and Production + UpahLain > 0 then 1 else 0 end) as production7,
		SUM(case when datepart(dw,ProductionDate) = 2 then 1 else 0 end) as prodtotal1,
		SUM(case when datepart(dw,ProductionDate) = 3 then 1 else 0 end) as prodtotal2,
		SUM(case when datepart(dw,ProductionDate) = 4 then 1 else 0 end) as prodtotal3,
		SUM(case when datepart(dw,ProductionDate) = 5 then 1 else 0 end) as prodtotal4,
		SUM(case when datepart(dw,ProductionDate) = 6 then 1 else 0 end) as prodtotal5,
		SUM(case when datepart(dw,ProductionDate) = 7 then 1 else 0 end) as prodtotal6,
		SUM(case when datepart(dw,ProductionDate) = 1 then 1 else 0 end) as prodtotal7 

from ProductionCard 
where ProductionDate >= @startdate and ProductionDate <= @endate group by LocationCode,UnitCode,Shift