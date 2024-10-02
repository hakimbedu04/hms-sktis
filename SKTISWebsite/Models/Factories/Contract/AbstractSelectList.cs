using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Core;

namespace SKTISWebsite.Models.Factories.Contract
{
    public abstract class AbstractSelectList
    {
        private List<MstGeneralListCompositeDTO> DataSource;

        public List<MstGeneralListCompositeDTO> _DataSource 
        {
            get { return DataSource; }
            set { DataSource = value; }
        }

        //#region Select List From MstGeneralList

        public abstract SelectList CreateDayType();

        public abstract SelectList CreateDay();

        public abstract SelectList CreateBrandFamily();

        public abstract SelectList CreatePack();

        public abstract SelectList CreateClass();

        //public abstract SelectList CreateShift();

        //public abstract SelectList CreateTPORank();

        public abstract SelectList CreateUom();

        //#endregion

        //#region Select List from Spesific Table

        public abstract SelectList CreateBrandGroupCode();

        public abstract SelectList CreateLocation();

        //#endregion
    }
}