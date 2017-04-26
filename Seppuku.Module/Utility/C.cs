using System;
using System.Text;
using JetBrains.Annotations;

namespace Seppuku.Module.Utility
{
    public static class C
    {
        private static readonly ConsoleColor[] ColorMap =
        {
            ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen,
            ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,
            ConsoleColor.Gray, ConsoleColor.DarkGray, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan,
            ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White
        };

        private static readonly char[] SpecialCharacters = {
            '`', '$', '&'
        };

        private static readonly object syncLock = new object();

        [StringFormatMethod("format")]
        public static void Write(string format, params object[] param)
        {
            // escape all parameters
            format = Format(format, param);

            lock (syncLock)
            {
                for (int i = 0; i < format.Length; i++)
                {
                    switch (format[i])
                    {
                        case '&':
                        case '$':
                        {
                            if (i == format.Length - 1)
                                throw new FormatException("Invalid position of color change character");
                            char fmtc = format[i + 1];
                            if (fmtc == 'r')
                            {
                                Console.ResetColor();
                            }
                            else
                            {
                                int cv = HexCharToInt(fmtc);
                                if (cv == -1) throw new FormatException("Invalid color code");
                                if (format[i] == '&')
                                    Console.ForegroundColor = ColorMap[cv];
                                else
                                    Console.BackgroundColor = ColorMap[cv];
                            }
                            ++i;
                        }
                            break;
                        case '`':
                        {
                            if (i == format.Length - 1)
                                throw new FormatException("Invalid position of color change character");
                            char fmtc = format[i + 1];
                            if (fmtc == 'i')
                            {
                                Write("&b[i]&r");
                            }
                            else if (fmtc == 'w')
                            {
                                Write("&e[!]&r");
                            }
                            else if (fmtc == 'e')
                            {
                                Write("&c[!]&r");
                            }
                            else if (fmtc == 'h')
                            {
                                Write("&0$7");
                            }
                            else
                            {
                                throw new FormatException("Invalid message code");
                            }
                            ++i;
                        }
                            break;
                        default:
                            Console.Write(format[i]);
                            break;
                    }
                }
            }
        }

        [StringFormatMethod("format")]
        public static void WriteLine(string format, params object[] param)
        {
            Write(format + '\n', param);
        }

        public static void Write(object o)
        {
            Write(o.ToString());
        }

        public static void WriteLine(object o)
        {
            WriteLine(o.ToString());
        }

        [StringFormatMethod("format")]
        public static string Format(string format, params object[] param)
        {
            object[] esc = new object[param.Length];
            for (int i = 0; i < param.Length; i++)
                esc[i] = Escape(param[i].ToString());

            return string.Format(format, esc);
        }
        public static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static int HexCharToInt(char hx)
        {
            hx = char.ToLower(hx);
            if (hx >= 'a' && hx <= 'f')
                return hx - 'a' + 0xa;
            if (hx >= '0' && hx <= '9')
                return hx - '0';
            return -1;
        }

        public static void Halt()
        {
            WriteLine("\n`h[CONTINUE]&r");
            Console.ReadKey();
        }

        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                bool valid = true;
                foreach (char p in SpecialCharacters)
                {
                    if (c == p)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
