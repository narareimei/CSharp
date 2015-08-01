using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PGLearning201507.CSV01;

namespace TestRunner
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void 設定確認用()
        {

            var filename = @"TestData\01.csv";

            var reader = new CSVReader();
            reader.Read(filename);

            Assert.True(true,"OK");
            return;
        }


    }
}
