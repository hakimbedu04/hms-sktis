using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class ExecutionBLLTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        //[TestMethod]
        //public void DataRangeIsNotUnionOrOverlapTest()
        //{
        //    //(X)
        //    //   1-----3  
        //    //      2-----4
        //    var result1 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), DateTime.Now.AddDays(2), DateTime.Now.AddDays(4));
        //    Assert.AreEqual(result1, false);

        //    // (X)
        //    //      1-----3
        //    //   2-----4
        //    var result2 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(2), DateTime.Now.AddDays(4), DateTime.Now.AddDays(1), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result2, false);

        //    // (X)
        //    //      2-----3
        //    //   1-----------4
        //    var result3 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), DateTime.Now.AddDays(1), DateTime.Now.AddDays(4));
        //    Assert.AreEqual(result3, false);

        //    // (X)
        //    //   1-----------4
        //    //      2-----3
        //    var result4 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(4), DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result4, false);

        //    // (Y)
        //    //   1-----2
        //    //               3-----4
        //    var result5 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), DateTime.Now.AddDays(4));
        //    Assert.AreEqual(result5, true);

        //    ////--------------------------------------------------------

        //    // (Y)
        //    //               3-----4
        //    //   1-----2
        //    var result6 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(3), DateTime.Now.AddDays(4), DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
        //    Assert.AreEqual(result6, true);

        //    ////--------------------------------------------------------

        //    // (X)
        //    //   1-----2
        //    //         2-----3
        //    var result7 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result7, false);

        //    // (X)
        //    //         2-----3
        //    //   1-----2
        //    var result8 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
        //    Assert.AreEqual(result8, false);

        //    // (X)
        //    //   1-----------3
        //    //   1-----2
        //    var result9 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
        //    Assert.AreEqual(result9, false);

        //    // (X)
        //    //         2-----3
        //    //   1-----------3
        //    var result10 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), DateTime.Now.AddDays(1), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result10, false);

        //    // (X)
        //    //   1-----------3
        //    //         2-----3
        //    var result11 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result11, false);

        //    // (X)
        //    //   1-----2
        //    //   1-----------3
        //    var result12 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), DateTime.Now.AddDays(1), DateTime.Now.AddDays(3));
        //    Assert.AreEqual(result12, false);

        //    // (X)
        //    //   1-----2
        //    //   1-----2
        //    var result13 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
        //    Assert.AreEqual(result13, false);

        //    // (X)
        //    //   1
        //    //   1
        //    var result14 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
        //    Assert.AreEqual(result14, false);

        //    // (Y)
        //    //   1
        //    //         2
        //    var result15 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), DateTime.Now.AddDays(2));
        //    Assert.AreEqual(result15, true);

        //    // (Y)
        //    //         2
        //    //   1
        //    var result16 = _calculationService.DataRangeIsNotUnionOrOverlap(DateTime.Now.AddDays(2), DateTime.Now.AddDays(2), DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
        //    Assert.AreEqual(result16, true);
        //}
    }
}
