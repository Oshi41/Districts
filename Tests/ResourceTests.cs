using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ResourceTests
    {
        private Assembly LoadMain()
        {
            var assembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(x => x.GetName().Name == "Districts");

            return assembly;
        }

        [TestMethod]
        public void FindAssembly()
        {
            Assert.IsNotNull(LoadMain());
        }

    }
}
