using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DistrictsTest.v2
{
    [TestClass]
    public class DoorParserTests
    {
        private IList<iDoor> CreateDoors()
        {
            return new List<iDoor>
            {
                new Door("1", 1, 1, -1, 1, 1, DoorStatus.Good,
                    new List<iCode>
                    {
                        new Code("1234", CodeStatus.Good)
                    }),

                new Door("1", 1, 1, -1, 2, 1, DoorStatus.Good,
                    new List<iCode>
                    {
                        new Code("1234", CodeStatus.NotWorking),
                        new Code("1234k1587", CodeStatus.Good)
                    }),

                new Door("1", 1, 1, -1, 222, 4, DoorStatus.Good,
                    new List<iCode>()),


                new Door("2", 5, 16, 4, 1, 1, DoorStatus.Good,
                    new List<iCode>
                    {
                        new Code("1", CodeStatus.NotWorking),
                    }),

                new Door("2", 5, 16, 4, 140, 3, DoorStatus.Good,
                    new List<iCode>
                    {
                    }),
            };

        }

        [TestMethod]
        public void OneDoor_ToJson_Equals()
        {
            var door = new Door("1", 1, 1, -1, 2, 1, DoorStatus.Good,
                new List<iCode>
                {
                    new Code("1234", CodeStatus.NotWorking),
                    new Code("1234k1587", CodeStatus.Good)
                }
            );

            var json = JsonConvert.SerializeObject(door, Formatting.Indented);

            var copy = JsonConvert.DeserializeObject<Door>(json);

            Assert.IsTrue(door.Equals(copy));
        }

        [TestMethod]
        public void Doors_ToJson_Equals()
        {
            var doors = CreateDoors();
            var json = JsonConvert.SerializeObject(doors);
            var copy = JsonConvert.DeserializeObject<List<Door>>(json);
            Assert.IsTrue(doors.IsTermwiseEquals(copy));
        }
    }
}
