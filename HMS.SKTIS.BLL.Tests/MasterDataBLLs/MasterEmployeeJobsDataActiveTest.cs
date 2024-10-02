using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    public partial class MasterDataBLLTest
    {
        //public List<MstPlantEmpJobsDataAcv> MstEmployeeJobsDataActivesStub()
        //{
        //    var mstPlantEmpJobsDataAcvs = new List<MstPlantEmpJobsDataAcv>();
        //    for (int i = 1; i <= RowCount; i++)
        //    {
        //        var mstPlantEmpJobsDataAcv = new MstPlantEmpJobsDataAcv();
        //        mstPlantEmpJobsDataAcvs.Add(mstPlantEmpJobsDataAcv);
        //    }
        //    return mstPlantEmpJobsDataAcvs;
        //}

        //[TestMethod]
        //public void GetMstEmployeeJobsDataActives_NotNull()
        //{
        //    //arange
        //    var mstEmployeeJobsDataActivesStub = MstEmployeeJobsDataActivesStub();
        //    _mstEmployeeJobsDataActive.Get().ReturnsForAnyArgs(mstEmployeeJobsDataActivesStub);
        //    var input = new GetMstEmployeeJobsDataActivesInput();

        //    //act
        //    var mstGeneralLists = _bll.GetMstEmployeeJobsDataActives(input);

        //    //assert
        //    Assert.IsNotNull(mstGeneralLists);
        //    Assert.AreEqual(RowCount, mstGeneralLists.Count);
        //}
    }
}
