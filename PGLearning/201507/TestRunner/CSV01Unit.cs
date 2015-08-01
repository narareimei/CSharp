using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PGLearning201507.CSV01;

namespace TestRunner
{
    [TestFixture]
    public class CSV01Unit
    {
        [Test]
        public void 設定確認用()
        {

            var filename = @"TestData\01.csv";

            CSVReader.Read(filename);

            Assert.True(true,"OK");
            return;
        }

        [Test]
        public void 設定確認用_Excelのセル中改行()
        {

            var filename = @"TestData\Book1.bin";

            CSVReader.Read(filename);

            Assert.True(true, "OK");
            return;
        }

        [Test]
        public void 設定確認用_ファイルなし()
        {

            var filename = @"TestData\NoFile.bin";

            CSVReader.Read(filename);

            Assert.True(true, "OK");
            return;
        }

    
    }
}
