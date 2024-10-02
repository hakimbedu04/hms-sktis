using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SKTISWebsite.Tests
{
    [TestClass]
    public class WebsiteMapperTests
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            SKTISWebsiteMapper.Initialize(); // initialize automapper
        }

        [TestMethod]
        public void WebsiteMapper_AutomapperConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
