using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using System.Configuration;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public class ExecutionOtherBLL : IExecutionOtherBLL
    {
        private IUnitOfWork _uow;
        private IMasterDataBLL _masterDataBll;
        private IGenericRepository<ProductAdjustment> _productAdjustmentRepo;
        private IGenericRepository<MstGenProcessSettingsLocation> _processSettingLocationRepo;
        private IGenericRepository<ExeProductionEntryPrintView> _productionEntryPrintViewRepo;
        private IGenericRepository<MstPlantProductionGroup> _plantProductionGroupRepo;
        private IGenericRepository<ExePlantProductionEntryVerification> _exePlantProductionEntryVerificationRepo;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<ExeReportByProcess> _exeReportByProcess;
        private IGenericRepository<ExeReportByGroup> _exeReportByGroupRepo;
        private IGenericRepository<MstADTemp> _mstAdTemp;

        public ExecutionOtherBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll)
        {
            _uow = uow;
            _masterDataBll = masterDataBll;
            _productAdjustmentRepo = _uow.GetGenericRepository<ProductAdjustment>();
            _processSettingLocationRepo = _uow.GetGenericRepository<MstGenProcessSettingsLocation>();
            _productionEntryPrintViewRepo = _uow.GetGenericRepository<ExeProductionEntryPrintView>();
            _plantProductionGroupRepo = _uow.GetGenericRepository<MstPlantProductionGroup>();
            _exePlantProductionEntryVerificationRepo = _uow.GetGenericRepository<ExePlantProductionEntryVerification>();
            _sqlSPRepo = _uow.GetSPRepository();
            _exeReportByProcess = _uow.GetGenericRepository<ExeReportByProcess>();
            _exeReportByGroupRepo = _uow.GetGenericRepository<ExeReportByGroup>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
        }

        public List<ProductAdjustmentDTO> GetProductAdjustments(ProductAdjustmentInput input)
        {
            var queryFilter = PredicateHelper.True<ProductAdjustment>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }

            if (input.ProductionDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ProductAdjustment>();
            var dbResult = _productAdjustmentRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ProductAdjustmentDTO>>(dbResult);
        }

        public List<string> GetBrandCodeByInput(GetExeOthersProductionEntryPrintInput input)
        {
            var queryFilter = PredicateHelper.True<ExeProductionEntryPrintView>();

            if (input.Week > 0)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.Week);
            }

            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.Year);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            var dbResult = _productionEntryPrintViewRepo
                .Get(queryFilter)
                .Select(x => x.BrandCode.ToString())
                .Distinct()
                .ToList();

            return dbResult;
        }

        public List<ExeProductionEntryPrintDataViewDTO> GetExeProductionEntryPrintData(
            GetExeOthersProductionEntryPrintInput input)
        {
            var weekdate = _masterDataBll.GetDateByWeek(input.Year, input.Week);
            var result = new List<ExeProductionEntryPrintDataViewDTO>();
            if (!string.IsNullOrEmpty(input.GroupCode))
            {
                var groupcodes = input.GroupCode.Split(',');
                foreach (var code in groupcodes)
                {

                    var queryFilter = PredicateHelper.True<ExeProductionEntryPrintView>();

                    if (!string.IsNullOrEmpty(input.LocationCode))
                    {
                        queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
                    }

                    if (!string.IsNullOrEmpty(input.UnitCode))
                    {
                        queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
                    }

                    if (input.Shift.HasValue)
                    {
                        queryFilter = queryFilter.And(p => p.Shift == input.Shift);
                    }

                    if (!string.IsNullOrEmpty(input.Process))
                    {
                        queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);
                    }

                    var groupDummy = code.Remove(1, 1).Insert(1, "5");
                    queryFilter = queryFilter.And(p => p.GroupCode == code || p.GroupCode == groupDummy);

                    if (!string.IsNullOrEmpty(input.Brand))
                    {
                        queryFilter = queryFilter.And(p => p.BrandCode == input.Brand);
                    }

                    if (input.Week > 0)
                    {
                        queryFilter = queryFilter.And(p => p.KPSWeek == input.Week);
                    }

                    if (input.Year > 0)
                    {
                        queryFilter = queryFilter.And(p => p.KPSYear == input.Year);
                    }

                    var dbResult = _productionEntryPrintViewRepo
                        .Get(queryFilter)
                        .OrderBy(o => o.EmployeeNumber.Substring(o.EmployeeNumber.Length - 2));

                    var list = Mapper.Map<List<ExeProductionEntryPrintViewDTO>>(dbResult);

                    var listEmp = list.Where(c => c.GroupCode.Substring(1, 1) != "5").ToList();
                    var listEmpDummy = list.Where(c => c.GroupCode.Substring(1, 1) == "5").ToList();

                    list.Clear();
                    list.AddRange(listEmp);
                    list.AddRange(listEmpDummy);

                    var mandor = "";
                    var mandorSource = GetMstPlantProductionGroupDto(code, input.LocationCode, input.UnitCode,
                        input.Process);
                    if (mandorSource != null)
                    {
                        if (mandorSource.Leader1 != "")
                            mandor = mandorSource.LeaderInspectionName + " | " + mandorSource.Leader1Name + " | " +
                                 mandorSource.Leader2Name;
                    }

                    var tpkvalue = new float[8];
                    var workhours = new int[8];

                    for (int i = 1; i < 8; i++)
                    {
                        var productionEntryCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           input.LocationCode,
                           input.Shift,
                           input.UnitCode,
                           code,
                           input.Brand,
                           input.Year,
                           input.Week,
                           i);

                        var exePlantProductionEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(productionEntryCode);
                        if (exePlantProductionEntryVerification == null)
                        {
                            tpkvalue[i] = 0;
                            workhours[i] = 0;
                        }
                        else
                        {
                            if (exePlantProductionEntryVerification.TPKValue.HasValue)
                            {
                                tpkvalue[i] = exePlantProductionEntryVerification.TPKValue.GetValueOrDefault();
                            }
                            else
                            {
                                tpkvalue[i] = 0;
                            }
                            workhours[i] = exePlantProductionEntryVerification.WorkHour;
                        }
                    }

                    var data = new ExeProductionEntryPrintDataViewDTO()
                    {
                        WeekDateList = weekdate,
                        Location = input.LocationCode + " - " + input.LocationName,
                        UnitCode = input.UnitCode,
                        Shift = input.Shift,
                        Process = input.Process,
                        GroupCode = code,
                        BrandCode = input.Brand,
                        KpsWeek = input.Week,
                        KpsYear = input.Year,
                        Mandor = mandor,
                        Remark = input.Remark,
                        TableData = list.Select((v, i) => new { Index = i, Value = v })
                                        .GroupBy(x => x.Index / 50)
                                        .Select(x => new ExeProductionEntryPrintViewPartitionDTO(x.Select(v => v.Value).ToList()))
                                        .ToList(),
                        TPKValue = tpkvalue.ToList(),
                        WorkHours = workhours.ToList(),
                        MonTotalCapacity = list.Sum(t => t.MonProdCapacity),
                        TueTotalCapacity = list.Sum(t => t.TueProdCapacity),
                        WedTotalCapacity = list.Sum(t => t.WedProdCapacity),
                        ThuTotalCapacity = list.Sum(t => t.ThuProdCapacity),
                        FriTotalCapacity = list.Sum(t => t.FriProdCapacity),
                        SatTotalCapacity = list.Sum(t => t.SatProdCapacity),
                        SunTotalCapacity = list.Sum(t => t.SunProdCapacity),
                        MonTotalTarget = list.Sum(t => t.MonProdTarget),
                        TueTotalTarget = list.Sum(t => t.TueProdTarget),
                        WedTotalTarget = list.Sum(t => t.WedProdTarget),
                        ThuTotalTarget = list.Sum(t => t.ThuProdTarget),
                        FriTotalTarget = list.Sum(t => t.FriProdTarget),
                        SatTotalTarget = list.Sum(t => t.SatProdTarget),
                        SunTotalTarget = list.Sum(t => t.SunProdTarget),
                        MonTotalActual = list.Sum(t => t.MonProdActual),
                        TueTotalActual = list.Sum(t => t.TueProdActual),
                        WedTotalActual = list.Sum(t => t.WedProdActual),
                        ThuTotalActual = list.Sum(t => t.ThuProdActual),
                        FriTotalActual = list.Sum(t => t.FriProdActual),
                        SatTotalActual = list.Sum(t => t.SatProdActual),
                        SunTotalActual = list.Sum(t => t.SunProdActual),
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        #region send email

        public void SendEmailSubmitPlantEntry(GetExePlantProductionEntryInput input, string currUserName)
        {
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.Brand,
                KpsWeek = input.Week,
                KpsYear = input.Year,
                Shift = Convert.ToInt32(input.Shift),
                UnitCode = input.UnitCode,
                FunctionName = Enums.PageName.PlantProductionEntry.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlantProductionEntry),
                Date = input.Date.Value,
                GroupCode = input.Group
            };

            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            var usename = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(usename)).FirstOrDefault();

            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                // PLANT PROD ENTRY
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                emailInput.UnitCode = item.Unit;
                emailInput.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.PlantProductionEntry);
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailSubmitPlantEntryToVerif(emailInput)
                };
                listEmail.Add(email);
            }

            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        public void SendEmailSubmitPlantEntryVerif(GetExePlantProductionEntryVerificationInput input, string currUserName)
        {
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.BrandCode,
                KpsWeek = input.KpsWeek,
                KpsYear = input.KpsYear,
                Shift = input.Shift,
                UnitCode = input.UnitCode,
                FunctionName = Enums.PageName.ProductionEntryVerification.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlanTPK),
                Date = input.Date.Value
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                // PROD CARD
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                emailInput.UnitCode = item.Unit;
                emailInput.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ProductionCard);
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailSubmitPlantEntryVerifToProdCard(emailInput)
                };
                listEmail.Add(email);
            }

            // disabled story #8403
            //foreach (var item in listUserAndEmailDestination)
            //{
            //    // Production Report by Process
            //    emailInput.Recipient = item.Name;
            //    emailInput.IDResponsibility = item.IDResponsibility ?? 0;
            //    emailInput.UnitCode = item.Unit;
            //    emailInput.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ProductionReportbyProcess);
            //    var email = new MailInput
            //    {
            //        FromName = currUserEmail == null ? "" : currUserEmail.Name,
            //        FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
            //        ToName = item.Name,
            //        ToEmailAddress = item.Email,
            //        Subject = emailInput.EmailSubject,
            //        BodyEmail = CreateBodyMailSubmitPlantEntryVerifToProductionReportbyProcess(emailInput)
            //    };
            //    listEmail.Add(email);
            //}

            // disabled story #8403
            //foreach (var item in listUserAndEmailDestination)
            //{
            //    // DailyProductionAchievement
            //    emailInput.Recipient = item.Name;
            //    emailInput.IDResponsibility = item.IDResponsibility ?? 0;
            //    emailInput.UnitCode = item.Unit;
            //    emailInput.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.DailyProductionAchievement);
            //    var email = new MailInput
            //    {
            //        FromName = currUserEmail == null ? "" : currUserEmail.Name,
            //        FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
            //        ToName = item.Name,
            //        ToEmailAddress = item.Email,
            //        Subject = emailInput.EmailSubject,
            //        BodyEmail = CreateBodyMailSubmitPlantEntryVerifToDailyProductionAchievement(emailInput)
            //    };
            //    listEmail.Add(email);
            //}

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        public void SendEmailReturnEntryVerification(GetExePlantProductionEntryVerificationInput input, string currUserName)
        {
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                UnitCode = input.UnitCode,
                Shift = input.Shift,
                GroupCode = input.GroupCode,
                BrandCode = input.BrandCode,
                KpsWeek = input.KpsWeek,
                KpsYear = input.KpsYear,
                Process = input.ProcessGroup,
                Date = input.ProductionDate == null ? DateTime.Now.Date : input.ProductionDate.Value,
                FunctionName = Enums.PageName.ProductionEntryVerification.ToString(),
                ButtonName = Enums.ButtonName.Return.ToString(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlantProductionEntryVerification),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.PlantProductionEntry),
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                emailInput.UnitCode = item.Unit;
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailReturnEntryVerification(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMailSubmitPlantEntryVerifToDailyProductionAchievement(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Laporan Produksi per Status  sudah tersedia, silakan melanjutkan proses  berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/ExeReportDailyProduction/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailSubmitPlantEntryVerifToProductionReportbyProcess(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            var brand = _masterDataBll.GetBrandGroupById(emailInput.BrandCode);

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Laporan Produksi per Process sudah tersedia, silakan melanjutkan proses  berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/ExeReportByProcess/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + brand.BrandGroupCode + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailSubmitPlantEntryToVerif(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Entry (Eblek)  sudah disubmit, Silakan melakukan verifikasi : " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/ExePlantProductionEntryVerification/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailSubmitPlantEntryVerifToProdCard(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Card sudah tersedia, Silakan melanjutkan proses  berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/ProductionCard/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">"
                                                                   + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailReturnEntryVerification(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            //var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Entry (Eblek) sudah direturn. Silahkan melakukan revisi: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/ExePlantProductionEntry/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.Process + "/"
                                                                   + emailInput.GroupCode + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">"
                                                                   + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        #endregion


        public MstPlantProductionGroupDTO GetMstPlantProductionGroupDto(string groupCode, string locationCode, string unitCode, string processGroup)
        {
            var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();

            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            queryFilter = queryFilter.And(p => p.ProcessGroup == processGroup);
            queryFilter = queryFilter.And(p => p.GroupCode == groupCode);

            var dbResult = _plantProductionGroupRepo.Get(queryFilter).SingleOrDefault(); ;

            return Mapper.Map<MstPlantProductionGroupDTO>(dbResult);
        }


        public void SaveProductAdjustment(ProductAdjustmentDTO input)
        {
            // get brand
            var brand = _masterDataBll.GetMstGenBrandById(input.BrandCode);

            if (brand == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            // get MstGenProcessSettingsLocation
            var mstGenProcessSettingLocation = _processSettingLocationRepo.Get(p => p.LocationCode == input.LocationCode);

            

            // get process group
            input.ProcessGroup =
                mstGenProcessSettingLocation.Select(
                    p => p.MstGenProcessSettings.Where(x => x.BrandGroupCode == brand.BrandGroupCode).Select(x => x.ProcessGroup).FirstOrDefault()).FirstOrDefault();
            /*
            // fixing process group
            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig))
            {
                input.ProcessGroup = EnumHelper.GetDescription(Enums.Process.Cutting);
            }
            else if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig))
            {
                input.ProcessGroup = EnumHelper.GetDescription(Enums.Process.Packing);
            }
            else if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig) 
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove) 
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig))
            {
                input.ProcessGroup = EnumHelper.GetDescription(Enums.Process.Stamping);
            }*/

            if (input.ProcessGroup == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var dbResult =
                _productAdjustmentRepo.Get(
                    p =>
                        p.LocationCode == input.LocationCode && p.UnitCode == input.UnitCode && p.Shift == input.Shift &&
                        p.BrandCode == input.BrandCode && p.ProductionDate == input.ProductionDate && p.AdjustmentType == input.AdjustmentType).FirstOrDefault();

            // validation
            if (dbResult != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            }

            var productAdjustment = Mapper.Map<ProductAdjustment>(input);
            _productAdjustmentRepo.Insert(productAdjustment);
            
            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig)
           || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig)
           || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig)
           || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)
           || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                SetValueForInsertExeReportByGroup(input);
            }

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove) ||
                input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                UpdateExereportByProcessFromAdjusment(input);
            }

       

            try
            {
                //throw (new Exception("error"));
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }


        public void UpdateProductAdjustment(ProductAdjustmentDTO input)
        {
            // get brand
            var brand = _masterDataBll.GetMstGenBrandById(input.BrandCode);

            if (brand == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            // get MstGenProcessSettingsLocation
            var mstGenProcessSettingLocation = _processSettingLocationRepo.Get(p => p.LocationCode == input.LocationCode);

            // get process group
            input.ProcessGroup =
                mstGenProcessSettingLocation.Select(
                    p => p.MstGenProcessSettings.Where(x => x.BrandGroupCode == brand.BrandGroupCode).Select(x => x.ProcessGroup).FirstOrDefault()).FirstOrDefault();

            if (input.ProcessGroup == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var dbResult = _productAdjustmentRepo.GetByID(input.ProductionDate, input.UnitCode, input.LocationCode,
                input.Shift, input.BrandCode, input.ProcessGroup, input.AdjustmentType);

            int? adjValueDif = 0;
            if (input.AdjustmentValue >= dbResult.AdjustmentValue)
            {
                adjValueDif = input.AdjustmentValue - dbResult.AdjustmentValue;
            }
            if (input.AdjustmentValue < dbResult.AdjustmentValue)
            {
                adjValueDif = dbResult.AdjustmentValue - input.AdjustmentValue;
                adjValueDif = -(adjValueDif);
            }

            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            input.CreatedBy = dbResult.CreatedBy;
            input.CreatedDate = dbResult.CreatedDate;
            Mapper.Map(input, dbResult);
            _productAdjustmentRepo.Update(dbResult);

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                var reportGroup = Mapper.Map<ExeReportByGroupDTO>(input);
                SetValueForUpdateExeReportByGroup(reportGroup, input, adjValueDif);
            }

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove) ||
                input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)
                || input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                UpdateExereportByProcessFromAdjusment(input);
            }

            
            try
            {
                //throw (new Exception("error"));
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }

        public List<string> GetBrandCodeFromReportByProcess(GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcess>();
            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            }

            if (input.KpsYear.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            }
            if (input.ProductionDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);
            }

            var brandCode = _exeReportByProcess.Get(queryFilter).Select(br => br.BrandCode).Distinct().ToList();
            
            return brandCode;
        }

        private void UpdateExereportByProcessFromAdjusment(ProductAdjustmentDTO input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcess>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(c => c.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(c => c.BrandCode == input.BrandCode);

            queryFilter = queryFilter.And(c => c.ProductionDate == input.ProductionDate);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.STAMPING.ToString() || c.ProcessGroup == Enums.ProcessGroup.WRAPPING.ToString());

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
            {
                queryFilter = queryFilter.And(c => c.UOMOrder == 7 || c.UOMOrder == 8 || c.UOMOrder == 9 || c.UOMOrder == 10 || c.UOMOrder == 11 || c.UOMOrder == 12 || c.UOMOrder == 13 || c.UOMOrder == 14);
            }

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove))
            {
                queryFilter = queryFilter.And(c => c.UOMOrder == 13 || c.UOMOrder == 14 || c.UOMOrder == 11 || c.UOMOrder == 12);
            }
            
            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4))
            {
                queryFilter = queryFilter.And(c => c.UOMOrder == 1 || c.UOMOrder == 2 || c.UOMOrder == 5 || c.UOMOrder == 6 || c.UOMOrder == 7 || c.UOMOrder == 8);
            }

            if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                queryFilter = queryFilter.And(c => c.UOMOrder == 1 || c.UOMOrder == 2 || c.UOMOrder == 7 || c.UOMOrder == 8);
            }

            var dbresult = _exeReportByProcess.Get(queryFilter);

            double[] keluarBersih = new double[14];
            double[] beginingStock = new double[14];

            foreach (var item in dbresult.OrderBy(m => m.UOMOrder))
            {
                var brandGroup = _masterDataBll.GetMstGenByBrandCode(input.BrandCode);
                var pack = _masterDataBll.GetBrandGroupById(brandGroup.BrandGroupCode);
                var oldEndingStock = item.EndingStock;

                switch (item.UOMOrder)
                {
                    case 1:
                        // UnCutCigarette
                        item.Production = item.Production + Convert.ToDouble(input.AdjustmentValue);
                        item.EndingStock = item.Production - item.KeluarBersih;
                        break;

                    case 2:
                        // CutCigarette
                        item.Production = item.Production + Convert.ToDouble(input.AdjustmentValue);
                        item.EndingStock = item.Production - item.KeluarBersih;
                        break;

                    case 5:
                        // UnStampPack Stick
                        item.Production = item.Production + Convert.ToDouble(input.AdjustmentValue);
                        item.EndingStock = item.EndingStock + Convert.ToDouble(input.AdjustmentValue);
                        break;

                    case 6:
                        // UnStampPack Pack
                        item.Production = item.Production + (Convert.ToDouble(input.AdjustmentValue) / pack.StickPerPack);
                        item.EndingStock = item.EndingStock + (Convert.ToDouble(input.AdjustmentValue) / pack.StickPerPack);
                        break;

                    case 7:
                        // StampPack Stick
                        if ((input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)) ||
                            (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8)))
                        {
                            item.Production = item.Production + Convert.ToDouble(input.AdjustmentValue);
                        }
                        else
                        {
                            item.RejectSample = Convert.ToDouble(input.AdjustmentValue);
                            //item.EndingStock = item.EndingStock - Convert.ToDouble(input.AdjustmentValue);
                            item.KeluarBersih = item.Production - item.RejectSample;
                            keluarBersih[7] = item.KeluarBersih;
                        }
                        break;
                    case 8:
                        // StampPack Pack
                        if ((input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4)) ||
                            (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8)))
                        {
                            item.Production = item.Production + (Convert.ToDouble(input.AdjustmentValue) / pack.StickPerPack);
                        }
                        else
                        {
                            item.RejectSample = Convert.ToDouble(input.AdjustmentValue) / pack.StickPerPack;
                            //item.EndingStock = item.EndingStock - (Convert.ToDouble(input.AdjustmentValue) / pack.StickPerPack);
                            item.KeluarBersih = item.Production - item.RejectSample;
                            keluarBersih[8] = item.KeluarBersih;
                        }
                        break;
                    case 9:
                        // DisplayCarton Stick

                        item.Production = keluarBersih[7];
                        item.KeluarBersih = item.Production;
                        keluarBersih[9] = item.KeluarBersih;
                        item.RejectSample = 0;
                        item.EndingStock = 0;
                        break;
                    case 10:
                        // DisplayCarton Slof
                        //item.RejectSample = Convert.ToDouble(input.AdjustmentValue) / Convert.ToDouble(pack.StickPerSlof);
                        //item.EndingStock = item.EndingStock - Convert.ToDouble(input.AdjustmentValue);

                        item.Production = keluarBersih[7] / Convert.ToDouble(pack.StickPerSlof);
                        item.KeluarBersih = item.Production;
                        keluarBersih[10] = item.KeluarBersih;
                        item.RejectSample = 0;
                        item.EndingStock = 0;
                        break;
                    case 11:
                        // InternalMove Stick
                        //item.RejectSample = Convert.ToDouble(input.AdjustmentValue);
                        //item.EndingStock = item.EndingStock - Convert.ToDouble(input.AdjustmentValue);
                        if (input.AdjustmentType != EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove))
                        {
                            item.Production = keluarBersih[9];
                            item.KeluarBersih = item.Production;
                            item.RejectSample = 0;
                            item.EndingStock = 0;
                        }
                        keluarBersih[11] = item.KeluarBersih;
                        beginingStock[11] = item.BeginningStock;
                        
                        break;
                    case 12:
                        // InternalMove Box
                        //item.RejectSample = Convert.ToDouble(input.AdjustmentValue) / Convert.ToDouble(pack.StickPerBox);
                        //item.EndingStock = item.EndingStock - Convert.ToDouble(input.AdjustmentValue);
                        if (input.AdjustmentType != EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove))
                        {
                            item.Production = keluarBersih[9] / Convert.ToDouble(pack.StickPerBox);
                            item.KeluarBersih = item.Production;
                            item.RejectSample = 0;
                            item.EndingStock = 0;
                        }
                        keluarBersih[12] = item.KeluarBersih;
                        beginingStock[12] = item.BeginningStock;

                        break;
                    case 13:
                        // ExternalMove Stick
                        if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
                        {
                            item.EndingStock = item.BeginningStock + keluarBersih[11]-item.KeluarBersih;

                            //GetDeltaValue 
                            var delta = item.EndingStock - oldEndingStock;
                            SetvalueEndingAndBeginingStokReportByProcess(item, delta, input);

                            item.Production = 0;
                        }
                        if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove))
                        {
                            item.KeluarBersih = Convert.ToDouble(input.AdjustmentValue);
                            item.EndingStock = item.BeginningStock + keluarBersih[11] - item.KeluarBersih;
                            if (input.AdjustmentValue == 0)
                            {
                                item.EndingStock = (item.BeginningStock + keluarBersih[11]) - item.KeluarBersih;
                            }

                            //GetDeltaValue 
                            var delta = item.EndingStock - oldEndingStock;
                            SetvalueEndingAndBeginingStokReportByProcess(item, delta,input);

                            item.RejectSample = 0;
                        }
                        break;
                    case 14:
                        // ExternalMove Box
                        if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
                        {
                            item.EndingStock = item.BeginningStock + keluarBersih[12]-item.KeluarBersih;
                            //GetDeltaValue 
                            var delta = item.EndingStock - oldEndingStock;
                            SetvalueEndingAndBeginingStokReportByProcess(item, delta,input);

                            item.Production = 0;
                        }
                        if (input.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove))
                        {
                            item.KeluarBersih = Convert.ToDouble(input.AdjustmentValue) / Convert.ToDouble(pack.StickPerBox);
                            item.EndingStock = item.BeginningStock + keluarBersih[12] - item.KeluarBersih;
                            if (input.AdjustmentValue == 0)
                            {
                                item.EndingStock = (item.BeginningStock + keluarBersih[12]) - item.KeluarBersih;
                            }

                            //GetDeltaValue 
                            var delta = item.EndingStock - oldEndingStock;
                            SetvalueEndingAndBeginingStokReportByProcess(item, delta,input);

                            item.RejectSample = 0;
                        }
                        break;

                }
                item.UpdatedDate = DateTime.Now;
                _exeReportByProcess.Update(item);

            }
        }

        #region InsertReportByGruopAndUpdateByProcess
        private void SetValueForInsertExeReportByGroup(ProductAdjustmentDTO inputAdjustment)
        {

            var year = inputAdjustment.ProductionDate.Year;
            var dataWeek = _masterDataBll.GetWeekByDate(inputAdjustment.ProductionDate);
            var brandGroup = _masterDataBll.GetMstGenByBrandCode(inputAdjustment.BrandCode);
            var listItem = new List<ExeReportByGroupDTO>();

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig))
            {
                var itemRolling = new ExeReportByGroupDTO();
                var dataRolling = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataRolling.ProcessGroup = Enums.ProcessGroup.ROLLING.ToString();
                dataRolling.GroupCode = "1100";
                listItem.Add(dataRolling);

                var itemCutting = new ExeReportByGroupDTO();
                var dataCutting = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataCutting.ProcessGroup = Enums.ProcessGroup.CUTTING.ToString();
                dataCutting.GroupCode = "2100";
                listItem.Add(dataCutting);

            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig))
            {

                var itemRolling = new ExeReportByGroupDTO();
                var dataRolling = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataRolling.ProcessGroup = Enums.ProcessGroup.ROLLING.ToString();
                dataRolling.GroupCode = "1100";
                listItem.Add(dataRolling);

                var itemCutting = new ExeReportByGroupDTO();
                var dataCutting = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataCutting.ProcessGroup = Enums.ProcessGroup.CUTTING.ToString();
                dataCutting.GroupCode = "2100";
                listItem.Add(dataCutting);

                var itemPacking = new ExeReportByGroupDTO();
                var dataPacking = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataPacking.ProcessGroup = Enums.ProcessGroup.PACKING.ToString();
                dataPacking.GroupCode = "3100";
                listItem.Add(dataPacking);
            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig))
            {

                var itemRolling = new ExeReportByGroupDTO();
                var dataRolling = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataRolling.ProcessGroup = Enums.ProcessGroup.ROLLING.ToString();
                dataRolling.GroupCode = "1100";
                listItem.Add(dataRolling);

                var itemCutting = new ExeReportByGroupDTO();
                var dataCutting = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataCutting.ProcessGroup = Enums.ProcessGroup.CUTTING.ToString();
                dataCutting.GroupCode = "2100";
                listItem.Add(dataCutting);

                var itemPacking = new ExeReportByGroupDTO();
                var dataPacking = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataPacking.ProcessGroup = Enums.ProcessGroup.PACKING.ToString();
                dataPacking.GroupCode = "3100";
                listItem.Add(dataPacking);

                var itemStickWraping = new ExeReportByGroupDTO();
                var dataStickWraping = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataStickWraping.ProcessGroup = Enums.ProcessGroup.STICKWRAPPING.ToString();
                dataStickWraping.GroupCode = "7100";
                listItem.Add(dataStickWraping);

            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4))
            {
                var itemStamping = new ExeReportByGroupDTO();
                var dataStamping = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataStamping.ProcessGroup = Enums.ProcessGroup.STAMPING.ToString();
                dataStamping.GroupCode = "4100";
                listItem.Add(dataStamping);

            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                var itemStamping = new ExeReportByGroupDTO();
                var dataStamping = Mapper.Map<ExeReportByGroupDTO>(inputAdjustment);
                dataStamping.ProcessGroup = Enums.ProcessGroup.STAMPING.ToString();
                dataStamping.GroupCode = "8100";
                listItem.Add(dataStamping);

            }

            foreach (var dataListItem in listItem)
            {
                dataListItem.BrandGroupCode = brandGroup.BrandGroupCode;
                dataListItem.KPSWeek = dataWeek.Week;
                dataListItem.KPSYear = year;
                dataListItem.Production = inputAdjustment.AdjustmentValue.HasValue ? inputAdjustment.AdjustmentValue.Value : 0;
                dataListItem.StatusEmp = Enums.StatusEmp.Resmi.ToString();
                GetWorkHourReportByGruop(dataListItem);

            }

            InsertExereportByGroupFromAdjusmnet(listItem, inputAdjustment);

        }

        private void InsertExereportByGroupFromAdjusmnet(IEnumerable<ExeReportByGroupDTO> listItem, ProductAdjustmentDTO inputAdjustment)
        {
            foreach (var item in listItem)
            {
                var queryFilter = PredicateHelper.True<ExeReportByGroup>();

                if (!string.IsNullOrEmpty(item.LocationCode))
                {
                    queryFilter = queryFilter.And(c => c.LocationCode == item.LocationCode);
                }
                if (!string.IsNullOrEmpty(item.UnitCode))
                {
                    queryFilter = queryFilter.And(c => c.UnitCode == item.UnitCode);
                }
                if (!string.IsNullOrEmpty(item.ProcessGroup))
                {
                    queryFilter = queryFilter.And(c => c.ProcessGroup == item.ProcessGroup);
                }
                if (!string.IsNullOrEmpty(item.GroupCode))
                {
                    queryFilter = queryFilter.And(c => c.GroupCode == item.GroupCode);
                }
                if (!string.IsNullOrEmpty(item.BrandCode))
                {
                    queryFilter = queryFilter.And(c => c.BrandCode == item.BrandCode);
                }
                queryFilter = queryFilter.And(c => c.ProductionDate == item.ProductionDate);

                var dbResult = _exeReportByGroupRepo.Get(queryFilter).FirstOrDefault();

                if (dbResult != null)
                {
                    dbResult.UpdatedDate = inputAdjustment.UpdatedDate.HasValue ? inputAdjustment.UpdatedDate.Value : DateTime.Now;
                    dbResult.UpdatedBy = inputAdjustment.UpdatedBy;
                    dbResult.Production = dbResult.Production + inputAdjustment.AdjustmentValue;


                    if (inputAdjustment.AdjustmentValue != 0)
                    {
                        _exeReportByGroupRepo.Update(dbResult);

                    }
                    else
                    {
                        DeleteDataDummyExeReportByGroup(dbResult);
                    }

                }
                else
                {
                    var data = Mapper.Map<ExeReportByGroup>(item);
                    data.CreatedDate = DateTime.Now;
                    data.UpdatedDate = DateTime.Now;
                    if (inputAdjustment.AdjustmentValue != 0)
                    {
                        _exeReportByGroupRepo.Insert(data);
                    }

                }

                var process = Mapper.Map<ExeReportByProcess>(item);
                UpdateExeReportByProcessAfterInsertExereportByGroup(process, inputAdjustment);

            }

        }

        private void UpdateExeReportByProcessAfterInsertExereportByGroup(ExeReportByProcess process, ProductAdjustmentDTO inputAdjustment)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcess>();
            if (!string.IsNullOrEmpty(process.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == process.LocationCode);

            if (!string.IsNullOrEmpty(process.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == process.UnitCode);

            if (!string.IsNullOrEmpty(process.BrandCode))
                queryFilter = queryFilter.And(c => c.BrandCode == process.BrandCode);

            queryFilter = queryFilter.And(c => c.ProductionDate == process.ProductionDate);

            if (!string.IsNullOrEmpty(process.ProcessGroup))
                queryFilter = queryFilter.And(c => c.ProcessGroup == process.ProcessGroup);

            var dbResutl = _exeReportByProcess.Get(queryFilter);

            foreach (var item in dbResutl)
            {
                var oldEndingStock = item.EndingStock;
                if (item.ProcessGroup == EnumHelper.GetDescription(Enums.ProcessGroup.PACKING) &&
                    item.UOM == Enums.Conversion.Pack.ToString())
                {
                    var brandGruopCode = _masterDataBll.GetBrandGruopCodeByBrandCode(item.BrandCode);
                    var stickPerpack = _masterDataBll.GetMasterGenBrandGroupPack(brandGruopCode);

                    var adjValuePerPack = inputAdjustment.AdjustmentValue/stickPerpack;
                    item.Production = item.Production + Convert.ToDouble(adjValuePerPack);
                    // fixed by bagus 21-11-2016
                    //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                    item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                }
                else
                {
                    item.Production = item.Production + Convert.ToDouble(inputAdjustment.AdjustmentValue);
                    // fixed by bagus 21-11-2016
                    //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                    item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                }
                // Other Process
                if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig))
                {
                    if (item.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString())
                    {
                        item.KeluarBersih = item.KeluarBersih + Convert.ToDouble(inputAdjustment.AdjustmentValue);
                        // fixed by bagus 21-11-2016
                        //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                        item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                    }

                    if ( item.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString())
                    {
                        item.KeluarBersih = item.KeluarBersih; // + Convert.ToDouble(inputAdjustment.AdjustmentValue);
                        // fixed by bagus 21-11-2016
                        //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                        item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                    }
                }
                if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig))
                {
                    if (item.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString())
                    {
                        item.KeluarBersih = item.KeluarBersih + Convert.ToDouble(inputAdjustment.AdjustmentValue);
                        // fixed by bagus 21-11-2016
                        //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                        item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                    }

                    if ( item.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString())
                    {
                        item.KeluarBersih = item.KeluarBersih; // + Convert.ToDouble(inputAdjustment.AdjustmentValue);
                        // fixed by bagus 21-11-2016
                        //item.EndingStock = item.EndingStock + Convert.ToDouble(adjValuePerPack);
                        item.EndingStock = item.BeginningStock + item.Production - item.KeluarBersih;
                    }
                }

                if (inputAdjustment.AdjustmentValue == 0)
                {
                    //revert the value back to before adjustment
                    var delta = item.EndingStock - item.BeginningStock;
                    //item.Production = item.Production - delta;
                    //item.EndingStock = item.EndingStock - delta;
                    item.Production = item.KeluarBersih + delta;
                    item.EndingStock = item.Production - item.KeluarBersih + item.BeginningStock;
                }
                else
                {
                    //getEndingStock
                    var delta = item.EndingStock - oldEndingStock;
                    SetvalueEndingAndBeginingStokReportByProcess(process, delta, inputAdjustment);
                }
                item.UpdatedDate = DateTime.Now;

                _exeReportByProcess.Update(item);
            }

        }
        #endregion

        #region updateReportByGruopAndByProcess
        private void SetValueForUpdateExeReportByGroup(ExeReportByGroupDTO exeReportByGroup, ProductAdjustmentDTO inputAdjustment, int? adjValueDif)
        {
            var brandGroup = _masterDataBll.GetMstGenByBrandCode(exeReportByGroup.BrandCode);

            var queryFilter = PredicateHelper.True<ExeReportByGroup>();
            if (!string.IsNullOrEmpty(exeReportByGroup.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == exeReportByGroup.LocationCode);

            if (!string.IsNullOrEmpty(exeReportByGroup.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == exeReportByGroup.UnitCode);

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig))
            {
                if (!string.IsNullOrEmpty(exeReportByGroup.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString());
                queryFilter = queryFilter.And(c => c.GroupCode == "1100" || c.GroupCode == "2100");
            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig))
            {
                if (!string.IsNullOrEmpty(exeReportByGroup.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.PACKING.ToString());
                queryFilter = queryFilter.And(c => c.GroupCode == "1100" || c.GroupCode == "2100" || c.GroupCode == "3100");
            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig))
            {
                if (!string.IsNullOrEmpty(exeReportByGroup.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.PACKING.ToString()
                        || c.ProcessGroup == Enums.ProcessGroup.STICKWRAPPING.ToString());
                queryFilter = queryFilter.And(c => c.GroupCode == "1100" || c.GroupCode == "2100" || c.GroupCode == "3100" || c.GroupCode == "7100");
            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig4))
            {
                if (!string.IsNullOrEmpty(exeReportByGroup.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.STAMPING.ToString());
                queryFilter = queryFilter.And(c => c.GroupCode == "4100");
            }

            if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.StampCig8))
            {
                if (!string.IsNullOrEmpty(exeReportByGroup.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == Enums.ProcessGroup.STAMPING.ToString());
                queryFilter = queryFilter.And(c => c.GroupCode == "8100");
            }

            if (!string.IsNullOrEmpty(brandGroup.BrandGroupCode))
                queryFilter = queryFilter.And(c => c.BrandGroupCode == brandGroup.BrandGroupCode);

            if (!string.IsNullOrEmpty(exeReportByGroup.BrandCode))
                queryFilter = queryFilter.And(c => c.BrandCode == exeReportByGroup.BrandCode);

            queryFilter = queryFilter.And(c => c.ProductionDate == exeReportByGroup.ProductionDate);

            if (!string.IsNullOrEmpty(exeReportByGroup.StatusEmp))
                queryFilter = queryFilter.And(c => c.StatusEmp == Enums.StatusEmp.Resmi.ToString());

            var dbResutl = _exeReportByGroupRepo.Get(queryFilter);

            if (dbResutl.Any())
            {
                var inputProcess = Mapper.Map<List<ExeReportByProcess>>(dbResutl);
                foreach (var item in dbResutl)
                {
                    
                    item.UpdatedDate = inputAdjustment.UpdatedDate.HasValue
                        ? inputAdjustment.UpdatedDate.Value
                        : DateTime.Now;
                    item.UpdatedBy = inputAdjustment.UpdatedBy;
                    item.Production = item.Production + adjValueDif;

                    _exeReportByGroupRepo.Update(item);
                    
                    if (inputAdjustment.AdjustmentValue == 0)
                    {
                        DeleteDataDummyExeReportByGroup(item);
                    }
                }

                UpdateExeReportByProcessAfterUpdateExereportByGroup(inputProcess, adjValueDif, inputAdjustment);
            }

            else
            {
                SetValueForInsertExeReportByGroup(inputAdjustment);
            }

        }

        private void UpdateExeReportByProcessAfterUpdateExereportByGroup(IEnumerable<ExeReportByProcess> process, int? adjValueDif, ProductAdjustmentDTO inputAdjustment)
        {
            foreach (var input in process)
            {
                var queryFilter = PredicateHelper.True<ExeReportByProcess>();
                if (!string.IsNullOrEmpty(input.LocationCode))
                    queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

                if (!string.IsNullOrEmpty(input.UnitCode))
                    queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);

                if (!string.IsNullOrEmpty(input.BrandCode))
                    queryFilter = queryFilter.And(c => c.BrandCode == input.BrandCode);

                queryFilter = queryFilter.And(c => c.ProductionDate == input.ProductionDate);

                if (!string.IsNullOrEmpty(input.ProcessGroup))
                    queryFilter = queryFilter.And(c => c.ProcessGroup == input.ProcessGroup);

                var dbResutl = _exeReportByProcess.Get(queryFilter);

                foreach (var item in dbResutl)
                {
                    var adjust = adjValueDif;
                    if (item.ProcessGroup == EnumHelper.GetDescription(Enums.ProcessGroup.PACKING) && item.UOM == Enums.Conversion.Pack.ToString())
                    {
                        var brandGruopCode = _masterDataBll.GetBrandGruopCodeByBrandCode(item.BrandCode);
                        var stickPerpack = _masterDataBll.GetMasterGenBrandGroupPack(brandGruopCode);

                        adjust = adjValueDif / stickPerpack;
                        //}

                        item.Production = item.Production + Convert.ToDouble(adjust);
                        //item.EndingStock = item.EndingStock + Convert.ToDouble(adjust);
                        item.EndingStock = item.BeginningStock + (item.Production - item.KeluarBersih);
                    }
                    item.UpdatedDate = DateTime.Now;
                    // Other Process
                    if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig))
                    {

                        if (item.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString() || item.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString())
                        {
                            item.Production = item.Production + Convert.ToDouble(adjust);
                            item.KeluarBersih = item.KeluarBersih + Convert.ToDouble(adjust);
                            item.EndingStock = item.BeginningStock + (item.Production - item.KeluarBersih);
                        }
                        else if (item.ProcessGroup == Enums.ProcessGroup.PACKING.ToString()) {
                            item.Production = item.Production + Convert.ToDouble(adjust);
                            item.EndingStock = item.BeginningStock + (item.Production - item.KeluarBersih);
                        }
                    }
                    if (inputAdjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig))
                    {
                        if (item.ProcessGroup == Enums.ProcessGroup.ROLLING.ToString())
                        {
                            item.Production = item.Production + Convert.ToDouble(adjust);
                            item.KeluarBersih = item.KeluarBersih + Convert.ToDouble(adjust);
                            item.EndingStock =  item.BeginningStock+ (item.Production - item.KeluarBersih);
                        }
                        else if (item.ProcessGroup == Enums.ProcessGroup.CUTTING.ToString())
                        {
                            item.Production = item.Production + Convert.ToDouble(adjust);
                            item.EndingStock = item.BeginningStock + (item.Production - item.KeluarBersih);
                        }
                    }

                    _exeReportByProcess.Update(item);
                }
            }
        }
        #endregion

        #region GetWorkHOurValueNotDummy
        private void GetWorkHourReportByGruop(ExeReportByGroupDTO input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByGroup>();
            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(c => c.LocationCode == input.LocationCode);
            }
            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);
            }
            if (!string.IsNullOrEmpty(input.ProcessGroup))
            {
                queryFilter = queryFilter.And(c => c.ProcessGroup == input.ProcessGroup);
            }
            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(c => c.BrandCode == input.BrandCode);
            }
            queryFilter = queryFilter.And(c => c.ProductionDate == input.ProductionDate);

            var dbResult = _exeReportByGroupRepo.Get(queryFilter).FirstOrDefault();
            if (dbResult != null)
            {
                input.WorkHour = dbResult.WorkHour.HasValue ? dbResult.WorkHour.Value : 0;
            }
            else
            {
                input.WorkHour = 0;
            }

        }
        #endregion

        #region DeleteDataDummyExeReportByGruop

        private void DeleteDataDummyExeReportByGroup(ExeReportByGroup data)
        {
            var dbData = _exeReportByGroupRepo.GetByID(data.LocationCode, data.GroupCode, data.BrandCode, data.StatusEmp, data.ProductionDate);

            //_exeReportByGroupRepo.Delete(dbData);
        }
        #endregion

        #region SetEndingStokAndBeginingStock

        private void SetvalueEndingAndBeginingStokReportByProcess(ExeReportByProcess input, double deltaStockValue, ProductAdjustmentDTO adjustment)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcess>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(c => c.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(c => c.BrandCode == input.BrandCode);

            queryFilter = queryFilter.And(c => c.ProductionDate > input.ProductionDate);
            if (adjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.ExtMove) 
                || adjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
            {
            queryFilter = queryFilter.And(c => c.UOMOrder == input.UOMOrder);    
            }

            if (adjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.CutCig)
             || adjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PackCig)
             || adjustment.AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.WrapCig))
            {
                queryFilter = queryFilter.And(c => c.ProcessGroup == input.ProcessGroup);    
            }
            
            var dbresult = _exeReportByProcess.Get(queryFilter);

            foreach (var item in dbresult)
            {
                item.EndingStock = item.EndingStock + deltaStockValue;
                item.BeginningStock = item.BeginningStock + deltaStockValue;
                _exeReportByProcess.Update(item);
            }

        }
        #endregion
    }
}
