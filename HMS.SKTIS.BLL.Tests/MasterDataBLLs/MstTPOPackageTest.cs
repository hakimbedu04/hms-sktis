using System;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace HMS.SKTIS.BLL.Tests
{
    public partial class MasterDataBLLTest
    {
        public List<MstTPOPackage> GetMstTPOPackagesStub()
        {
            var mstTPOPackages = new List<MstTPOPackage>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstTPOPackage = new MstTPOPackage();
                mstTPOPackages.Add(mstTPOPackage);
            }
            return mstTPOPackages;
        }

        [TestMethod]
        public void GetTPOPackages_NotNull()
        {
            //arange
            var mstTPOPackagesStub = GetMstTPOPackagesStub();
            _mstTPOPackageRepo.Get().ReturnsForAnyArgs(mstTPOPackagesStub);
            var input = new GetMstTPOPackagesInput();

            //act
            var mstGeneralLists = _bll.GetTPOPackages(input);

            //assert
            Assert.IsNotNull(mstGeneralLists);
            Assert.AreEqual(RowCount, mstGeneralLists.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertTPOPackage_WhenKeyExist_ThrownException()
        {
            //arrange
            _mstTPOPackageRepo.GetByID().ReturnsForAnyArgs(x => new MstTPOPackage());

            try
            {
                //act
                _bll.InsertTPOPackage(new MstTPOPackageDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertTPOPackage_WhenValid_Insert()
        {
            //arrange
            var mstTPOPackageDTO = new MstTPOPackageDTO
            {
                EffectiveDate = new DateTime(2012, 10, 20),
                BrandGroupCode = "DSS12HR-20",
                LocationCode = "SKT"
            };

            //act
            _bll.InsertTPOPackage(mstTPOPackageDTO);

            //assert
            _mstTPOPackageRepo.Received()
                .Insert(
                    Arg.Is<MstTPOPackage>(
                        m =>
                            m.EffectiveDate == new DateTime(2012, 10, 20) && m.BrandGroupCode == "DSS12HR-20" &&
                            m.LocationCode == "SKT"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateTPOPackage_WhenNull_ThrowException()
        {
            //arrange
            _mstTPOPackageRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateTPOPackage(new MstTPOPackageDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateTPOPackage_WhenValid_Update()
        {
            //arrange
            var mstTPOPackageDTO = new MstTPOPackageDTO
            {
                EffectiveDate = new DateTime(2012, 10, 20),
                BrandGroupCode = "DSS12HR-20",
                LocationCode = "SKT",
                Remark = "NewRemark"
            };
            var dbMstTPOPackage = new MstTPOPackage()
            {
                EffectiveDate = new DateTime(2012, 10, 20),
                BrandGroupCode = "DSS12HR-20",
                LocationCode = "SKT",
                Remark = "OldRemark"
            };

            _mstTPOPackageRepo.GetByID().ReturnsForAnyArgs(dbMstTPOPackage);

            //act
            _bll.UpdateTPOPackage(mstTPOPackageDTO);

            //assert
            Assert.AreEqual(mstTPOPackageDTO.Remark, dbMstTPOPackage.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
