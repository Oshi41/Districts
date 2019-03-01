using System;
using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DistrictsTest.v2
{
    [TestClass]
    public class RecordParserTests
    {
        [TestMethod]
        public void Records_ToJson_Equals()
        {
            var records = new List<iRecord>
            {
                new Record(ActionType.Deleted, "1", DateTime.Now),

                new Record(ActionType.Dropped, "1", DateTime.Now),

                new Record(ActionType.Taken, "2", DateTime.Now),
            };

            var json = JsonConvert.SerializeObject(records);
            var copy = JsonConvert.DeserializeObject<List<Record>>(json);

            Assert.IsTrue(records.IsTermwiseEquals(copy));
        }
    }
}
