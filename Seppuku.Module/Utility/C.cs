using System;

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

        private static readonly object syncLock = new object();

        public static void Write(string f)
        {
            lock (syncLock)
            {
                for (int i = 0; i < f.Length; i++)
                {
                    switch (f[i])
                    {
                        case '&':
                        case '$':
                        {
                            if (i == f.Length - 1)
                                throw new FormatException("Invalid position of color change character");
                            char fmtc = f[i + 1];
                            if (fmtc == 'r')
                            {
                                Console.ResetColor();
                            }
                            else
                            {
                                int cv = HexCharToInt(fmtc);
                                if (cv == -1) throw new FormatException("Invalid color code");
                                if (f[i] == '&')
                                    Console.ForegroundColor = ColorMap[cv];
                                else
                                    Console.BackgroundColor = ColorMap[cv];
                            }
                            ++i;
                        }
                            break;
                        case '`':
                        {
                            if (i == f.Length - 1)
                                throw new FormatException("Invalid position of color change character");
                            char fmtc = f[i + 1];
                            if (fmtc == 'i')
                            {
                                Write("&b[i]&r");
                            }
                            else if (fmtc == 'w')
                            {
                                Console.Beep();
                                Write("&e[!]&r");
                            }
                            else if (fmtc == 'e')
                            {
                                Console.Beep();
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
                            Console.Write(f[i]);
                            break;
                    }
                }
            }
        }

        public static void WriteLine(string f)
        {
            Write(f + '\n');
        }

        public static void Write(object o)
        {
            Write(o.ToString());
        }

        public static void WriteLine(object o)
        {
            WriteLine(o.ToString());
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
    }
}
