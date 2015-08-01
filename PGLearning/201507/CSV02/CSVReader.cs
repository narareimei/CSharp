using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace PGLearning201507.CSV02
{
    public class CSVReader
    {
        /// <summary>
        /// 処理状態
        /// </summary>
        enum Mode
        {
            INIT = 0,
            //TOKEN_BEGIN,
            TOKEN_INSIDE,
            QUOTE_BEGIN,
            QUOTE_INSIDE,
            QUOTE_ESCAPE,
            //QUOTE_ESCAPE_INSIDE,
            //QUOTE_ESCAPE_END,
            //QUOTE_END,
            //TOKEN_END,
            TOKEN_OUTSIDE,
            DELIMITTER
        };

        /// <summary>
        /// 文字の種類
        /// </summary>
        enum CharcterType
        {
            NORMAL_CHARACTER = 0,
            BLANK,
            QUOTATION ,
            DELIMITTER,
            EOL,
            EOF
        };


        static public int Read(String fileName)
        {
            return 0;
        }


        static public String[] ReadFields(TextReader reader)
        {

            //var fields = new[] { "a", "b", "c" };

            var fields = new List<String>();

            //fields.Add("a");
            //fields.Add("b");
            //fields.Add("c");

            var sb = new StringBuilder();
            Mode mode = Mode.INIT;
            while (true)
            {
                var ch = Read(reader);

                switch(ClassifyCharacter(ch)) 
                {
                    case CharcterType.EOF:
                        if(mode ==Mode.DELIMITTER)
                        {
                            // TODO
                            throw new InvalidDataException("行末がカンマで終了しています。");
                        }else
                        if (sb.Length > 0)
                        {
                            fields.Add(sb.ToString());
                        }
                        return fields.ToArray();
                        break;

                    case CharcterType.EOL:
                        // TODO 暫定
                        if(mode ==Mode.DELIMITTER)
                        {
                            // TODO
                            throw new InvalidDataException("行末がカンマで終了しています。");
                        }else
                        if (sb.Length > 0)
                        {
                            fields.Add(sb.ToString());
                        }
                        return fields.ToArray();
                        break;

                    case CharcterType.BLANK:
                        if (mode == Mode.QUOTE_ESCAPE)
                        {
                            mode = Mode.TOKEN_OUTSIDE;
                        }
                        else
                        {
                        mode = Mode.TOKEN_INSIDE;
                            sb.Append(ch);
                        }

                        break;
                    
                    case CharcterType.DELIMITTER:
                        mode = Mode.DELIMITTER;

                        fields.Add(sb.ToString());
                        sb.Clear();
                        break;

                    case CharcterType.NORMAL_CHARACTER:
                        if( mode == Mode.QUOTE_BEGIN)
                        {
                            mode = Mode.QUOTE_INSIDE;
                        }
                        else if (mode == Mode.QUOTE_INSIDE)
                        {
                            // do nothing
                        }
                        else if (mode == Mode.TOKEN_OUTSIDE)
                        {
                            throw new Exception("フォーマット異常。クォート閉じ後に文字が存在します。");
                        }
                        else if (mode == Mode.QUOTE_ESCAPE)
                        {
                            // エスケープじゃなかった
                            throw new Exception("フォーマット異常。クォート閉じ直後に文字が存在します。");
                        }

                        else
                        {
                            mode = Mode.TOKEN_INSIDE;
                        }
                        sb.Append(ch);
                        break;

                    case CharcterType.QUOTATION:
                        //if(mode == Mode.QUOTE_INSIDE)
                        //{
                        //    mode = Mode.QUOTE_END;
                        //    fields.Add(sb.ToString());
                        //    sb.Clear();
                        //}else
                        if(mode == Mode.TOKEN_INSIDE)
                        {
                            if( string.IsNullOrEmpty(sb.ToString().Trim()))
                            {
                                mode = Mode.QUOTE_BEGIN;
                                sb.Clear();
                            }else
                            {
                                throw new Exception("フォーマット異常。ダブルクォーテーションの前に文字が存在します。");
                            }

                        }else
                        if(mode == Mode.INIT || mode == Mode.DELIMITTER)
                        {
                            mode = Mode.QUOTE_BEGIN;
                        }else
                        if( mode == Mode.QUOTE_BEGIN || mode == Mode.QUOTE_INSIDE)
                        {
                            //fields.Add(sb.ToString());
                            //sb.Clear();
                            //mode = Mode.QUOTE_END;
                            mode = Mode.QUOTE_ESCAPE;
                        }
                        else if( mode == Mode.QUOTE_ESCAPE )
                        {
                            sb.Append(ch);

                            mode = Mode.QUOTE_INSIDE;
                        }

                        //throw new NotImplementedException("クォテーションはまだ実装していません。");
                        break;

                    default:
                        Debug.Assert(false, "ClassifyCharacter()の戻り値が不正です");
                        break;
                }


            }

            return fields.ToArray();
        }

        /// <summary>
        /// １文字読み込む。CRLFは一度に読み込む。EOF時は""を返す
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>Windows上での改行コードのみ、CRとLFと２文字合わせて１改行文字として扱う</para>
        /// <para>処理ごとにそれを考慮していくのは煩雑なため、ここに集約する</para>
        /// </remarks>
        static private string Read(TextReader reader)
        {
            var ch = reader.Read();

            if (ch == -1)
            {
                return "";
            }

            switch (ch)
            {
                case '\u000d':
                    var next = reader.Peek();

                    if (next == '\u000a')
                    {
                        //CRLF
                        reader.Read();
                        return "\u000d\000a";
                    }
                    else
                    {
                        return "\u000d";
                    }
                    break;
                case '\u000a':
                    return "\u000a";
                    break;
                
            }

            return ((char)ch).ToString();
        }


        /// <summary>
        /// 文字を分類する
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        static private CharcterType ClassifyCharacter(string ch)
        {
            if(string.IsNullOrEmpty(ch))
            {
                return CharcterType.EOF;
            }

           else if (ch.Equals( " ", StringComparison.Ordinal))
            {
                return CharcterType.BLANK;
            }

           else if (ch.Equals( "\"", StringComparison.Ordinal))
            {
                return CharcterType.QUOTATION;
            }

            else if (ch.Equals(",", StringComparison.Ordinal))
            {
                return CharcterType.DELIMITTER;
            }

            else if (ch.Equals("\u000d", StringComparison.Ordinal)
                    || ch.Equals("\u000a", StringComparison.Ordinal)
                    || ch.Equals("\u000d\u000a", StringComparison.Ordinal))
            {
                return CharcterType.EOL;
            }

            return CharcterType.NORMAL_CHARACTER;
        }

    }
}
