using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;


namespace PGLearning201507.CSV01
{
    public class CSVReader
    {
        static public int Read(String fileName)
        {
            var result = new List<String[]>();
            TextFieldParser parser = null;

            try
            {
                parser = new TextFieldParser(fileName, System.Text.Encoding.UTF8);
                parser.SetDelimiters(new[] { "," });

                while (!parser.EndOfData)
                {
                    // TODO CRLFで複数行に分割され、空行が存在する場合に対応できず
                    var fields = parser.ReadFields();

                    // TODO カラム数のチェック

                    Debug.WriteLine("----------------------");
                    foreach (var item in fields.ToList())
                    {
                        Debug.Write(item);
                        Debug.WriteLine("**");
                    }
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
            }
            catch (MalformedLineException ex)
            {
                // 解析不能例外
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (parser != null)
                {
                    parser.Close();
                }
            }

            return 0;
        }



    }
}
