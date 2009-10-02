using System;
using System.Text;

namespace Chronicle
{
    public static class Log
    {
        private static object sLock = new object();

        public static void WriteLine(ELogLevel pLogLevel, string pFormat, params object[] pArgs)
        {
            string buffer = DateTime.Now.ToString() + " (" + pLogLevel.ToString() + ") " + string.Format(pFormat, pArgs);
            lock (sLock) Console.WriteLine(buffer);
        }

        public static void Dump(byte[] pBuffer, int pStart, int pLength)
        {
            string[] split = (pLength > 0 ? BitConverter.ToString(pBuffer, pStart, pLength) : "").Split('-');
            StringBuilder hex = new StringBuilder(16 * 3);
            StringBuilder ascii = new StringBuilder(16);
            StringBuilder buffer = new StringBuilder();
            char temp;
            if (pLength > 0)
            {
                for (int index = 0; index < split.Length; ++index)
                {
                    temp = Convert.ToChar(pBuffer[pStart + index]);
                    hex.Append(split[index] + ' ');

                    if (char.IsWhiteSpace(temp) || char.IsControl(temp)) temp = '.';

                    ascii.Append(temp);
                    if ((index + 1) % 16 == 0)
                    {
                        buffer.AppendLine(string.Format("{0} {1}", hex, ascii));
                        hex.Length = 0;
                        ascii.Length = 0;
                    }
                }
                if (hex.Length > 0)
                {
                    if (hex.Length < (16 * 3)) hex.Append(new string(' ', (16 * 3) - hex.Length));
                    buffer.AppendLine(string.Format("{0} {1}", hex, ascii));
                }
                lock (sLock) Console.WriteLine(buffer);
            }
        }
    }

    public enum ELogLevel
    {
        Info,
        Warn,
        Error,
        Exception,
        Debug
    }
}
