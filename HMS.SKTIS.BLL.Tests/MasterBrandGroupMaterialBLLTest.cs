using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    //[TestClass]
    //public class MasterBrandGroupMaterialBLLTest
    //{
    //    private IMasterDataBLL _masterDataBll;
    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _masterDataBll = Substitute.For<IMasterDataBLL>();
    //        BLLMapper.Initialize();
    //    }

    //    [TestMethod]
    //    public void GetBrandGroupMaterialIsCorrect()
    //    {
    //        var dummy = new List<BrandGroupMaterialDTO>
    //        {
    //            new BrandGroupMaterialDTO
    //            {
    //                BrandGroupCode = "FA029194.12",
    //                MaterialName = "Tembakau",
    //                Description = "Description",
    //                Uom = "EA",
    //                Remark = "Remark",
    //                UpdatedBy = "Oka",
    //                UpdatedDate = Convert.ToDateTime("7/7/2015")
    //            }
    //        };

    //        _masterDataBll.GetBrandGroupMaterial().Returns(dummy);
    //        var result = _masterDataBll.GetBrandGroupMaterial();

    //        Assert.IsNotNull(result);
    //    }

    //    [TestMethod]
    //    public void AutoMapperIsCorrect()
    //    {
    //        var dummy = new List<MstMaterial>
    //        {
    //            new MstMaterial
    //            {
    //                BrandGroupCode = "FA029194.12",
    //                MaterialName = "Tembakau",
    //                Description = "Description",
    //                UOM = "EA",
    //                Remark = "Remark",
    //                UpdatedBy = "Oka",
    //                UpdatedDate = Convert.ToDateTime("7/7/2015")
    //            }
    //        };

    //        var result = Mapper.Map<List<BrandGroupMaterialDTO>>(dummy);

    //        Assert.IsNotNull(result);
    //    }
    //}
}
