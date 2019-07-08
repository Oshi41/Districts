using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.JsonClasses;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Json;
using DistrictsLib.Legacy.JsonClasses.Manage;
using DistrictsNew.ViewModel.Manage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Tests.JsonTest
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void SerializeOnlyInterface()
        {
            var jsonClass = (ICardManagement)new CardManagement
            {
                Number = "sfgstgh",
                Actions = new List<IManageRecord>
                {
                    new ManageRecord
                    {
                        ActionType = ActionTypes.Dropped,
                        Date = DateTime.Now,
                        Subject = "1231"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(jsonClass);

            var vm = (ICardManagement)new CardManagementViewModel(jsonClass, new ActionArbiter(), () => null, null);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver()
            };

            var secondJson = JsonConvert.SerializeObject(vm, settings);
        }
    }
}
