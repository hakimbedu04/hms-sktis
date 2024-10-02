using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace HMS.SKTIS.BusinessObjects.Outputs
{
    public class Responsibility
    {
        public int IDResponsibility;
        public string ResponsibilityName;
        public Dictionary<string, ResponsibilityPage> Page { get; set; }
        public int Role;
        public List<ResponsibilityLocation> Location { get; set; }
    }

    public class ResponsibilityLocation
    {
        public MstGenLocationDTO LocationData { get; set; }
        public string Units { get; set; }
    }

    public class ResponsibilityPage {
        public int PageID { get; set; }
        public string Name { get; set; }
        public Dictionary<string, ResponsibilityPage> Child { get; set; }
    }

    public class ResponsibilityButton
    {
        public int PageID { get; set; }
        public List<string> Button { get; set; }
        //public bool Add { get; set; }
        //public bool Allocation { get; set; }
        //public bool Approve { get; set; }
        //public bool Authorize { get; set; }
        //public bool BackToList { get; set; }
        //public bool Calculate { get; set; }
        //public bool CancelSubmit { get; set; }
        //public bool Complete { get; set; }
        //public bool Delete { get; set; }
        //public bool Detail { get; set; }
        //public bool Excel { get; set; }
        //public bool JKProses { get; set; }
        //public bool P1Template { get; set; }
        //public bool Print { get; set; }
        //public bool Return { get; set; }
        //public bool Revise { get; set; }
        //public bool Save { get; set; }
        //public bool SendApproval { get; set; }
        //public bool SendNotification { get; set; }
        //public bool Submit { get; set; }
        //public bool UnitWorkHours { get; set; }
        //public bool Upload { get; set; }
        //public bool Usage { get; set; }
        //public bool Verify { get; set; }
        //public bool View { get; set; }
        //public bool WIPStock { get; set; }
    }
}
