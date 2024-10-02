
DECLARE @jobId BINARY(16)
EXEC  msdb.dbo.sp_add_job @job_name=N'SKTIS_HRDV2_IMDL_UserResponsibility', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@job_id = @jobId OUTPUT

/****** Object:  Step [CheckUser_HRDV2]    Script Date: 6/12/2017 9:13:21 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'CheckUser_HRDV2', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC [dbo].[HRDV2_CheckUSER]', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Clean_Temp_1]    Script Date: 6/12/2017 9:13:21 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Clean_Temp_1', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DELETE FROM HRDV2_SKTIS_Temp1', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Copy_To_Temp1]    Script Date: 6/12/2017 9:13:21 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Copy_To_Temp1', 
		@step_id=3, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_COPYTO_TEMP1', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Check_Temp1_Temp2_CopyToTempMail]    Script Date: 6/12/2017 9:13:21 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Check_Temp1_Temp2_CopyToTempMail', 
		@step_id=4, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_COPYTO_TEMPEMAIL', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Clean_Temp2]    Script Date: 6/12/2017 9:13:22 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Clean_Temp2', 
		@step_id=5, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DELETE FROM HRDV2_SKTIS_Temp2', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [CopyTemp1_To_Temp2]    Script Date: 6/12/2017 9:13:22 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'CopyTemp1_To_Temp2', 
		@step_id=6, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_COPYTEMP1_TOTEMP2', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Read_XML_IMDL]    Script Date: 6/12/2017 9:13:22 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Read_XML_IMDL', 
		@step_id=7, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_READ_IMDL_XML', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Compare_IMDL_CurrentSKTIS]    Script Date: 6/12/2017 9:13:22 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Compare_IMDL_CurrentSKTIS', 
		@step_id=8, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_COMPARE_IMDL_UtilUserResponsibility', 
		@database_name=N'SKTIS', 
		@flags=0

/****** Object:  Step [Send_TempEmail]    Script Date: 6/12/2017 9:13:22 AM ******/
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Send_TempEmail', 
		@step_id=9, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC HRDV2_CHECKING_EMAIL', 
		@database_name=N'SKTIS', 
		@flags=0
GO

