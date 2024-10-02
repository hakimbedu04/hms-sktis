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
        public List<MstPlantProductionGroupView> GetMstPlantProductionGroupStub()
        {
            var mstPlantProductionGroups = new List<MstPlantProductionGroupView>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstPlantProductionGroup = new MstPlantProductionGroupView();
                mstPlantProductionGroups.Add(mstPlantProductionGroup);
            }
            return mstPlantProductionGroups;
        }

        [TestMethod]
        public void GetMstPlantProductionGroups_NotNull()
        {
            //arange
            var mstPlantProductionGroupsStub = GetMstPlantProductionGroupStub();
            _mstPlantProductionGroupViewRepo.Get().ReturnsForAnyArgs(mstPlantProductionGroupsStub);
            var input = new GetMstPlantProductionGroupsInput();

            //act
            var mstPlantProductionGroups = _bll.GetMstPlantProductionGroups(input);

            //assert
            Assert.IsNotNull(mstPlantProductionGroups);
            Assert.AreEqual(RowCount, mstPlantProductionGroups.Count);
        }

        [TestMethod]
        public void SaveMstPlantProductionGroup_WhenNew_Insert()
        {
            //arrange
            var plantProductionGroupDTO = new MstPlantProductionGroupDTO()
            {
                GroupCode = "1111",
                LocationCode = "Location1",
                UnitCode = "Unit1",
                ProcessGroup = "Gunting"
            };
            _mstPlantProductionGroupRepo.GetByID().ReturnsNullForAnyArgs();

            //act
            _bll.SaveMstPlantProductionGroup(plantProductionGroupDTO);

            _mstPlantProductionGroupRepo.Received().Insert(Arg.Is<MstPlantProductionGroup>(t => t.GroupCode == "1111" && t.LocationCode == "Location1" && t.UnitCode == "Unit1" && t.ProcessGroup == "Gunting"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod]
        public void SaveMstPlantProductionGroup_WhenNotNew_Update()
        {
            //arrange
            var plantProductionGroup = new MstPlantProductionGroup()
            {
                GroupCode = "1111",
                LocationCode = "Location1",
                UnitCode = "Unit1",
                ProcessGroup = "Gunting",
                Remark = "oldValue"
            };

            var plantProductionGroupDTO = new MstPlantProductionGroupDTO()
            {
                GroupCode = "1111",
                LocationCode = "Location1",
                UnitCode = "Unit1",
                ProcessGroup = "Gunting",
                Remark = "newValue"
            };

            _mstPlantProductionGroupRepo.GetByID().ReturnsForAnyArgs(plantProductionGroup);

            //act
            _bll.SaveMstPlantProductionGroup(plantProductionGroupDTO);

            Assert.AreEqual(plantProductionGroupDTO.Remark, plantProductionGroup.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
