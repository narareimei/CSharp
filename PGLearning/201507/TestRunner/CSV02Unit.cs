using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

using PGLearning201507.CSV02;


namespace TestRunner
{
    [TestFixture]
    class CSV02Unit
    {
        [Test]
        public void 設定確認()
        {
            Assert.True(true, "設定不備");
            return; 
        }

        [Test]
        public void _01_ダブルクォーテーションなし_改行なし_一行()
        {
           
            var reader = new StringReader("a,b,c");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 3);
            Assert.True(fields[0].Equals("a", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("b", StringComparison.Ordinal) == true);
            Assert.True(fields[2].Equals("c", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        public void _01_ダブルクォーテーションなし_改行なし_二行()
        {

            var reader = new StringReader("a,b,c\nd,e,f");

            // １行目
            var fields = CSVReader.ReadFields(reader);
            Assert.True(fields.Count() == 3);
            Assert.True(fields[0].Equals("a", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("b", StringComparison.Ordinal) == true);
            Assert.True(fields[2].Equals("c", StringComparison.Ordinal) == true);

            // ２行目
            fields = CSVReader.ReadFields(reader);
            Assert.True(fields.Count() == 3);
            Assert.True(fields[0].Equals("d", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("e", StringComparison.Ordinal) == true);
            Assert.True(fields[2].Equals("f", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        public void _01_ダブルクォーテーションなし_改行なし_一行_日本語()
        {

            var reader = new StringReader("一,二,さん,日本語,ニホンゴ");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 5);
            Assert.True(fields[0].Equals("一", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("二", StringComparison.Ordinal) == true);
            Assert.True(fields[2].Equals("さん", StringComparison.Ordinal) == true);
            Assert.True(fields[3].Equals("日本語", StringComparison.Ordinal) == true);
            Assert.True(fields[4].Equals("ニホンゴ", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        public void _02_ダブルクォーテーション有り_クォート前文字なし_改行なし_一行()
        {

            var reader = new StringReader("\"abc\",\"def\"");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 2);
            Assert.True(fields[0].Equals("abc", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("def", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        public void _02_ダブルクォーテーション有り_クォート前空白_改行なし_一行()
        {

            var reader = new StringReader("\"abc\", \"def\"");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 2);
            Assert.True(fields[0].Equals("abc", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("def", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void _02_ダブルクォーテーション有り_クォート前文字_改行なし_一行()
        {

            var reader = new StringReader("\"abc\",a\"def\"");
            var fields = CSVReader.ReadFields(reader);

            return;
        }

        [Test]
        public void _02_ダブルクォーテーション有り_クォート後ろ空白_改行なし_一行()
        {

            var reader = new StringReader("\"abc\" ,\"def\"");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 2);
            Assert.True(fields[0].Equals("abc", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("def", StringComparison.Ordinal) == true);

            return;
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void _02_ダブルクォーテーション有り_クォート後ろ文字_改行なし_一行()
        {

            var reader = new StringReader("\"abc\"あ,\"def\"");
            var fields = CSVReader.ReadFields(reader);

            return;
        }

        [Test]
        public void _02_ダブルクォーテーション有り_クォートエスケープ有り_改行なし_一行()
        {

            var reader = new StringReader("\"ab\"\"c\",\"def\"");
            var fields = CSVReader.ReadFields(reader);

            Assert.True(fields.Count() == 2);
            Assert.True(fields[0].Equals("ab\"c", StringComparison.Ordinal) == true);
            Assert.True(fields[1].Equals("def", StringComparison.Ordinal) == true);

            return;
        }

    }
}
