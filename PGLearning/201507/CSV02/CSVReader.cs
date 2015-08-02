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
            TOKEN_INSIDE,
            QUOTE_BEGIN,
            QUOTE_INSIDE,
            QUOTE_ESCAPE,
            TOKEN_OUTSIDE,
            DELIMITTER
        };

        /// <summary>
        /// 文字分類
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        static public String[] ReadFields(TextReader reader, bool trimming = false)
        {
            // TODO クォートで囲まれない場合に、トークン前後の空白をトリミングするかを指定可能とする
            // TODO 例外は専用例外を設ける

            var fields = new List<String>();
            var field = new StringBuilder();
            var mode = Mode.INIT;
            var quotedToken = false;

            while (true)
            {
                var ch = Read(reader);

                switch(ClassifyCharacter(ch)) 
                {
                    case CharcterType.EOF:

                        if (mode == Mode.QUOTE_BEGIN || mode == Mode.QUOTE_INSIDE)
                        {
                            throw new Exception("ダブルクォーテーションが閉じられずにファイルが終了しました。");
                        }

                        else if(mode ==Mode.DELIMITTER)
                        {
                            // TODO 行末カンマ
                            throw new Exception("行末がカンマで終了しています。");
                        }
                        
                        else if (field.Length > 0)
                        {
                            RegisterField(fields, field.ToString(), quotedToken, trimming);
                            quotedToken = false;
                            field.Clear();
                        }
                        return fields.ToArray();
                        break;

                    case CharcterType.EOL:
                        if( mode == Mode.QUOTE_INSIDE )
                        {
                            field.Append(ch);
                        }
                        else if(mode ==Mode.DELIMITTER)
                        {
                            // TODO 行末カンマ
                            throw new InvalidDataException("行末がカンマで終了しています。");
                        }
                        else if (field.Length > 0)
                        {
                            RegisterField(fields, field.ToString(), quotedToken, trimming);
                            quotedToken = false;
                            field.Clear();
                            return fields.ToArray();
                        }
                        break;

                    case CharcterType.BLANK:
                        if (mode == Mode.QUOTE_ESCAPE)
                        {
                            mode = Mode.TOKEN_OUTSIDE;
                        }
                        else if (mode == Mode.QUOTE_BEGIN)
                        {
                            mode = Mode.QUOTE_INSIDE;
                            field.Append(ch);
                        }
                        else if (mode == Mode.QUOTE_INSIDE)
                        {
                            field.Append(ch);
                        }
                        else
                        {
                            mode = Mode.TOKEN_INSIDE;
                            field.Append(ch);
                        }

                        break;
                    
                    case CharcterType.DELIMITTER:
                        mode = Mode.DELIMITTER;

                        RegisterField(fields, field.ToString(), quotedToken, trimming);
                        quotedToken = false;
                        field.Clear();
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
                        else if (mode == Mode.QUOTE_ESCAPE)
                        {
                            // エスケープじゃなかった
                            throw new Exception("フォーマット異常。クォート閉じ直後に文字が存在します。");
                        }
                        else if (mode == Mode.TOKEN_OUTSIDE)
                        {
                            throw new Exception("フォーマット異常。クォート閉じ後に文字が存在します。");
                        }

                        else
                        {
                            mode = Mode.TOKEN_INSIDE;
                        }
                        field.Append(ch);
                        break;

                    case CharcterType.QUOTATION:
                        if(mode == Mode.INIT)
                        {
                            mode = Mode.QUOTE_BEGIN;
                        }
                        else　if( mode == Mode.QUOTE_BEGIN || mode == Mode.QUOTE_INSIDE)
                        {
                            mode = Mode.QUOTE_ESCAPE;
                            quotedToken = true;
                        }
                        else if( mode == Mode.QUOTE_ESCAPE )
                        {
                            field.Append(ch);

                            mode = Mode.QUOTE_INSIDE;
                        }
                        else if(mode == Mode.TOKEN_INSIDE)
                        {
                            if( string.IsNullOrEmpty(field.ToString().Trim()))
                            {
                                mode = Mode.QUOTE_BEGIN;
                                field.Clear();
                            }else
                            {
                                throw new Exception("フォーマット異常。ダブルクォーテーションの前に文字が存在します。");
                            }

                        }
                        else if (mode == Mode.TOKEN_OUTSIDE)
                        {
                            throw new Exception("フォーマット異常。トークン終了後にクォートが存在します。");
                        }

                        else if (mode == Mode.DELIMITTER)
                        {
                            mode = Mode.QUOTE_BEGIN;
                        }

                        break;

                    default:
                        Debug.Assert(false, "ClassifyCharacter()の戻り値が不正です");
                        break;
                }
            }

            return fields.ToArray();
        }


        /// <summary>
        /// １フィールドを結果リストへ登録する
        /// <para>トークン前後のトリミングを行なう。</para>
        /// <para>クオテーションで囲まれていない場合のみトリミングの対象とする</para>
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="field"></param>
        /// <param name="quotedToken"></param>
        /// <param name="trimming"></param>
        static private void RegisterField( List<string> fields, string field, bool quotedToken, bool trimming)
        {
            if (!quotedToken && trimming)
            {
                fields.Add(field.Trim());
            }else
            {
                fields.Add(field);
            }
            return;
        }

        /// <summary>
        /// トークン前後空白のトリミング
        /// </summary>
        /// <param name="token"></param>
        /// <param name="trimming"></param>
        /// <returns></returns>
        private string TrimToken(string token, bool trimming)
        {
            // TODO そのトークンがダブルクォーテーションでくくられていたか否かで、トリミングの扱いが違ってくる

            return (trimming) ? token.Trim() : token;
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
