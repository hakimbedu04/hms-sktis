using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class BLLMapperTests
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            BLLMapper.Initialize(); // initialize automapper
        }

        [TestMethod]
        public void BLLMapper_AutomapperConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
