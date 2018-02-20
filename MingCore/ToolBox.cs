using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MingCore
{
    public class ToolBox
    {
        public static string bin2strTrim(byte[] data) //将byte数组转化为可读的十六进制字串
        {
            string result = "";
            int i = 0;

            for (i = 0; i < data.Length; i++)
            {
                result = result + dec2hexTrim(data[i]);
            }

            return result;
        }

        public static string bin2str(byte[] data) //将byte数组转化为可读的十六进制字串
        {
            string result = "";
            int i = 0;

            for (i = 0; i < data.Length; i++)
            {
                result = result + data[i].ToString() + " ";
            }

            return result;
        }

        public static string bin2hexstr(byte[] data) //将byte数组转化为可读的十六进制字串
        {
            string result = "";
            int i = 0;

            for (i = 0; i < data.Length; i++)
            {
                result = result + dec2hex(data[i]);
            }

            return result;
        }

        public static string dec2hex(byte value)  //把字节转换成对应的两位十六进制字符。不足补0字符在第一位,后面补空格字符。。
        {
            string res = "";

            res = Microsoft.VisualBasic.Conversion.Hex(value);
            if (res.Length == 1)
            {
                res = "0" + res;
            }
            res = res + " ";
            return res;
        }

        public static string dec2hexTrim(byte value)  //把字节转换成对应的两位十六进制字符。不足补0字符在第一位,后面不补空格字符。。
        {
            string res = "";
            res = Microsoft.VisualBasic.Conversion.Hex(value);
            if (res.Length == 1)
            {
                res = "0" + res;
            }
            return res;
        }

        public static byte[] str2binNoSpace(string str)
        {
            string contentAddWithSpace = "";
            for (int checkPos = 0; checkPos < str.Length; checkPos += 2)
            {
                contentAddWithSpace += str[checkPos].ToString();
                if (str.Length > checkPos + 1)
                {
                    contentAddWithSpace += str[checkPos + 1].ToString();
                }
                contentAddWithSpace += " ";
            }
            string[] splitted = null;
            int length = 0;
            int i = 0;
            byte[] result = null;

            contentAddWithSpace = contentAddWithSpace.Trim();
            splitted = contentAddWithSpace.Split(' ');
            length = splitted.Length;
            result = new byte[length];
            for (i = 0; i < length; i++)
            {
                result[i] = byte.Parse(System.Convert.ToString(Microsoft.VisualBasic.Conversion.Val("&H" + splitted[i])));
            }
            //str2bin = result
            return result;
        }

        public static string bin2ASCIIString(byte[] pmDirectBytes)
        {
            string result = "";

            result = Encoding.ASCII.GetString(pmDirectBytes);

            return result;
        }

        public static string bin2ASCIIString(byte[] pmReversedBytes, int pmOffset, int pmLength)
        {
            string result = "";

            result = Encoding.ASCII.GetString(pmReversedBytes.Skip(pmOffset).Take(pmLength).ToArray()).TrimEnd('\0');

            return result;
        }

        public static string bin2ASCIIStringReversesBytes(byte[] pmReversedBytes)
        {
            string result = "";

            result = Encoding.ASCII.GetString(pmReversedBytes.Reverse().ToArray());

            return result;
        }

        public static string bin2ASCIIStringReversesBytes(byte[] pmReversedBytes, int pmOffset, int pmLength)
        {
            string result = "";

            result = Encoding.ASCII.GetString(pmReversedBytes.Skip(pmOffset).Take(pmLength).Reverse().ToArray()).TrimStart('\0');

            return result;
        }

        public static byte[] ASCIIString2bin(string pmASCIIString)
        {
            byte[] result = Encoding.ASCII.GetBytes(pmASCIIString);

            return result;
        }

        public static byte[] ASCIIString2binReversesBytes(string pmASCIIString)
        {
            byte[] result = Encoding.ASCII.GetBytes(pmASCIIString).Reverse().ToArray();

            return result;
        }

        public static int DoDeflate(ref byte[] pmSource, int pmOffSet, int pmLength, int pmCompressionLevel)
        {
            int deflatedLength = 0;

            ICSharpCode.SharpZipLib.Zip.Compression.Deflater checkDeflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(pmCompressionLevel);
            deflatedLength = checkDeflater.Deflate(pmSource, pmOffSet, pmLength);

            return deflatedLength;
        }
    }
}