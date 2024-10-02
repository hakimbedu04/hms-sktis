using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using SKTISWebsite.Models.EquipmentRequest;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentFulfillment
{
    public class InitEquipmentFulfillment
    {
        public InitEquipmentFulfillment()
        {
            DefaultRequestDate = DateTime.Now.Date;
        }
        public string FilterLocationCode { get; set; }
        public List<LocationLookupList> FilterLocation { get; set; }
        public SelectList FilterRequestNumber { get; set; }
        public SelectList FilterRequestor { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }
        public DateTime DefaultRequestDate { get; set; }
        public string DefaultRequestor { get; set; }
        //public List<EquipmentRequestViewModel> EquipmentRequestList { get; set; }
        public string Param1Locationcode { get; set; }
        public DateTime Param2Date { get; set; }
        public List<MstMntcItemCompositeDTO> ItemsList { get; set; }
    }
}