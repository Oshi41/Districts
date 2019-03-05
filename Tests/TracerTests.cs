using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Districts.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TracerTests
    {
        [TestMethod]
        public void TastMultiTheading()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Tracer.Write($"Message {j}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Thread.Sleep(1000);
        }
    }
}
