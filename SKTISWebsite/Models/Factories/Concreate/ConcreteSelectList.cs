using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Factories.Contract;

namespace SKTISWebsite.Models.Factories.Concreate
{
    public class ConcreteSelectList : AbstractSelectList
    {
        private ISelectListBLL _selectListBll;
        public ConcreteSelectList(ISelectListBLL selectListBll)
        {
            _selectListBll = selectListBll;
        }

        #region Select List From MstGeneralList

        public override SelectList CreateDayType()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.DayType.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        public override SelectList CreateDay()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.Day.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        public override SelectList CreateBrandFamily()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.BrandFamily.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        public override SelectList CreatePack()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.Pack.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        public override SelectList CreateClass()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.Class.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        //public override SelectList CreateShift()
        //{
        //    var data = base._DataSource.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.Shift));
        //    return new SelectList(data, Enums.DropdownOption.ListGroup.ToString(), Enums.DropdownOption.ListDetail.ToString());
        //}

        //public override SelectList CreateTPORank()
        //{
        //    var data = base._DataSource.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.TPORank));
        //    return new SelectList(data, Enums.DropdownOption.ListGroup.ToString(), Enums.DropdownOption.ListDetail.ToString());
        //}

        public override SelectList CreateUom()
        {
            var data = base._DataSource.Where(p => p.ListGroup == Enums.MasterGeneralList.MtrlUOM.ToString());
            return new SelectList(data, Enums.DropdownOption.ListDetail.ToString(), Enums.DropdownOption.ListDetail.ToString());
        }

        #endregion

        #region SelectList From Spesific Table

        /// <summary>
        /// Create Brand Group Code select list data source
        /// </summary>
        /// <returns>SelectList</returns>
        public override SelectList CreateBrandGroupCode()
        {
            var data = _selectListBll.GetBrandGroupCodes();
            return new SelectList(data, Enums.DropdownOption.BrandGroupCode.ToString(), Enums.DropdownOption.BrandGroupCode.ToString());
        }

        ///// <summary>
        ///// Create Location select list data source
        ///// </summary>
        ///// <returns>SelectList</returns>
        public override SelectList CreateLocation()
        {
            var data = _selectListBll.GetLocations();
            return new SelectList(data, Enums.DropdownOption.LocationCode.ToString(), Enums.DropdownOption.LocationCode.ToString());
        }

        #endregion
    }
}