UPDATE ExeReportByGroups
SET 
	ActualWorker = EmpIn - (Multi_CUTT + Multi_FWRP + Multi_GEN + Multi_PACK + Multi_ROLL + Multi_STAMP + Multi_SWRP + Multi_TPO + Multi_WRP),
	ValuePeople = COALESCE(CAST(Production as REAL) / NULLIF(ActualWorker, 0), 0),
	ValuePeopleHour =  COALESCE(COALESCE(Production / NULLIF(WorkHour, 0), 0) / NULLIF(ActualWorker, 0), 0),
	ValueHour = COALESCE(Production / NULLIF(WorkHour, 0), 0)

UPDATE ExeReportByGroupsWeekly
SET 
	ActualWorker = EmpIn - (Multi_CUTT + Multi_FWRP + Multi_GEN + Multi_PACK + Multi_ROLL + Multi_STAMP + Multi_SWRP + Multi_TPO + Multi_WRP),
	ValuePeople = COALESCE(CAST(Production as REAL) / NULLIF(ActualWorker, 0), 0),
	ValuePeopleHour =  COALESCE(COALESCE(Production / NULLIF(WorkHour, 0), 0) / NULLIF(ActualWorker, 0), 0),
	ValueHour = COALESCE(Production / NULLIF(WorkHour, 0), 0)

UPDATE ExeReportByGroupsMonthly
SET 
	ActualWorker = EmpIn - (Multi_CUTT + Multi_FWRP + Multi_GEN + Multi_PACK + Multi_ROLL + Multi_STAMP + Multi_SWRP + Multi_TPO + Multi_WRP),
	ValuePeople = COALESCE(CAST(Production as REAL) / NULLIF(ActualWorker, 0), 0),
	ValuePeopleHour =  COALESCE(COALESCE(Production / NULLIF(WorkHour, 0), 0) / NULLIF(ActualWorker, 0), 0),
	ValueHour = COALESCE(Production / NULLIF(WorkHour, 0), 0)