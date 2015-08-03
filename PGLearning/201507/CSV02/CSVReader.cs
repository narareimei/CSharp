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
        /////////////////////////////////////////////////////////////////////////////////
        // 定数
        /////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////
        // 列挙子
        /////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 処理状態
        /// </summary>
        enum Position
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
            QUOTATION,
            DELIMITTER,
            EOL,
            EOF
        };


        /// <summary>
        /// １行分のカラム取得
        /// </summary>
        /// <param name="reader">テキストリーダー</param>
        /// <param name="trimming">トークン前後の空白トリミング指定</param>
        /// <returns>カラム値の配列</returns>
        static public String [ ] ReadFields( TextReader reader, bool trimming = false )
        {
            var fields = new List<String>( );
            var field = new StringBuilder( );
            var quotedToken = false;
            var pos = Position.INIT;

            while ( true )
            {
                var ch = Read( reader );

                switch ( ClassifyCharacter( ch ) )
                {
                    case CharcterType.EOF:
                        if ( pos == Position.QUOTE_BEGIN || pos == Position.QUOTE_INSIDE )
                        {
                            throw new MalformedFormatException( "ダブルクォーテーションが閉じられずにファイルが終了しました。" );
                        }

                        else if ( pos == Position.DELIMITTER )
                        {
                            throw new MalformedFormatException( "行末がカンマで終了しています。" );
                        }

                        else
                        {
                            //INIT = 0,
                            //TOKEN_INSIDE,
                            //QUOTE_ESCAPE,
                            //TOKEN_OUTSIDE,
                            if ( field.Length > 0 )
                            {
                                RegisterField( fields, field.ToString( ), quotedToken, trimming );
                                quotedToken = false;
                                field.Clear( );
                            }
                        }
                        return fields.ToArray( );
                        break;

                    case CharcterType.EOL:
                        if ( pos == Position.QUOTE_BEGIN || pos == Position.QUOTE_INSIDE )
                        {
                            field.Append( ch );
                        }
                        else if ( pos == Position.DELIMITTER )
                        {
                            throw new InvalidDataException( "行末がカンマで終了しています。" );
                        }
                        else
                        {
                            //INIT = 0,
                            //TOKEN_INSIDE,
                            //QUOTE_ESCAPE,
                            //TOKEN_OUTSIDE,
                            if ( field.Length > 0 )
                            {
                                RegisterField( fields, field.ToString( ), quotedToken, trimming );
                                quotedToken = false;
                                field.Clear( );
                                return fields.ToArray( );
                            }
                        }
                        break;

                    case CharcterType.BLANK:
                        if ( pos == Position.QUOTE_BEGIN )
                        {
                            pos = Position.QUOTE_INSIDE;
                            field.Append( ch );
                        }
                        else if ( pos == Position.QUOTE_INSIDE )
                        {
                            field.Append( ch );
                        }
                        else if ( pos == Position.QUOTE_ESCAPE )
                        {
                            pos = Position.TOKEN_OUTSIDE;
                        }
                        else if ( pos == Position.TOKEN_OUTSIDE )
                        {
                            // do nothing
                            Debug.Print( "do nothing" );
                        }
                        else
                        {
                            //INIT = 0,
                            //TOKEN_INSIDE,
                            //DELIMITTER
                            pos = Position.TOKEN_INSIDE;
                            field.Append( ch );
                        }

                        break;

                    case CharcterType.DELIMITTER:
                        if ( pos == Position.QUOTE_BEGIN )
                        {
                            pos = Position.QUOTE_INSIDE;
                            field.Append( ch );
                        }
                        else if (  pos == Position.QUOTE_INSIDE )
                        {
                            field.Append( ch );
                        }
                        else
                        {
                            //INIT = 0,
                            //TOKEN_INSIDE,
                            //QUOTE_ESCAPE,
                            //TOKEN_OUTSIDE,
                            //DELIMITTER
                            pos = Position.DELIMITTER;

                            RegisterField( fields, field.ToString( ), quotedToken, trimming );
                            quotedToken = false;
                            field.Clear( );                        
                        }
                        
                    break;

                    case CharcterType.NORMAL_CHARACTER:
                        if ( pos == Position.QUOTE_BEGIN )
                        {
                            pos = Position.QUOTE_INSIDE;
                        }
                        else if ( pos == Position.QUOTE_INSIDE )
                        {
                            // do nothing
                        }
                        else if ( pos == Position.QUOTE_ESCAPE )
                        {
                            // エスケープじゃなかった
                            throw new MalformedFormatException( "フォーマット異常。クォート閉じ直後に文字が存在します。" );
                        }
                        else if ( pos == Position.TOKEN_OUTSIDE )
                        {
                            throw new MalformedFormatException( "フォーマット異常。クォート閉じ後に文字が存在します。" );
                        }
                        else
                        {
                            //INIT = 0,
                            //TOKEN_INSIDE,
                            //DELIMITTER
                            pos = Position.TOKEN_INSIDE;
                        }
                        field.Append( ch );
                        break;

                    case CharcterType.QUOTATION:
                        if ( pos == Position.INIT )
                        {
                            pos = Position.QUOTE_BEGIN;
                        }
                        else if ( pos == Position.QUOTE_BEGIN || pos == Position.QUOTE_INSIDE )
                        {
                            pos = Position.QUOTE_ESCAPE;
                            quotedToken = true;
                        }
                        else if ( pos == Position.QUOTE_ESCAPE )
                        {
                            field.Append( ch );

                            pos = Position.QUOTE_INSIDE;
                        }
                        else if ( pos == Position.TOKEN_INSIDE )
                        {
                            if ( string.IsNullOrEmpty( field.ToString( ).Trim( ) ) )
                            {
                                pos = Position.QUOTE_BEGIN;
                                field.Clear( );
                            }
                            else
                            {
                                throw new MalformedFormatException( "フォーマット異常。ダブルクォーテーションの前に文字が存在します。" );
                            }

                        }
                        else if ( pos == Position.TOKEN_OUTSIDE )
                        {
                            throw new MalformedFormatException( "フォーマット異常。トークン終了後にクォートが存在します。" );
                        }
                        else if ( pos == Position.DELIMITTER )
                        {
                            pos = Position.QUOTE_BEGIN;
                        }

                        break;

                    default:
                        Debug.Assert( false, "ClassifyCharacter()の戻り値が不正です" );
                        break;
                }
            }

            return fields.ToArray( );
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
        static private void RegisterField( List<string> fields, string field, bool quotedToken, bool trimming )
        {
            if ( !quotedToken && trimming )
            {
                fields.Add( field.Trim( ) );
            }
            else
            {
                fields.Add( field );
            }
            return;
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
        static private string Read( TextReader reader )
        {
            var ch = reader.Read( );

            if ( ch == -1 )
            {
                return "";
            }

            switch ( ch )
            {
                case '\u000d':
                    var next = reader.Peek( );

                    if ( next == '\u000a' )
                    {
                        //CRLF
                        reader.Read( );
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

            return ( ( char )ch ).ToString( );
        }


        /// <summary>
        /// 文字を分類する
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        static private CharcterType ClassifyCharacter( string ch )
        {
            if ( string.IsNullOrEmpty( ch ) )
            {
                return CharcterType.EOF;
            }

            else if ( ch.Equals( " ", StringComparison.Ordinal ) )
            {
                return CharcterType.BLANK;
            }

            else if ( ch.Equals( "\"", StringComparison.Ordinal ) )
            {
                return CharcterType.QUOTATION;
            }

            else if ( ch.Equals( ",", StringComparison.Ordinal ) )
            {
                return CharcterType.DELIMITTER;
            }

            else if ( ch.Equals( "\u000d", StringComparison.Ordinal )
                    || ch.Equals( "\u000a", StringComparison.Ordinal )
                    || ch.Equals( "\u000d\u000a", StringComparison.Ordinal ) )
            {
                return CharcterType.EOL;
            }

            return CharcterType.NORMAL_CHARACTER;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////
    // 例外クラス
    /////////////////////////////////////////////////////////////////////////////////
    public class MalformedFormatException : Exception
    {
        public MalformedFormatException( string message )
            : base( message )
        {
            ;
        }
    }
}
