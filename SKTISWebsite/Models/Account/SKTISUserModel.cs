using System;
using System.Collections.Generic;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.Outputs;
namespace SKTISWebsite.Models.Account
{
    public class SKTISUserModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public List<LocationModel> Location { get; set; }
        public Responsibility Responsibility { get; set; }
    }

    public class LocationModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}