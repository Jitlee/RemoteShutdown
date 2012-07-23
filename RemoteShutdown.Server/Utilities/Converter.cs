using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RemoteShutdown.Utilities
{
    public class Converter
    {
        public static int ToInt(object o)
        {
            if (null != o)
            {
                int result;
                if (int.TryParse(o.ToString(), out result))
                {
                    return result;
                }
            }
            return default(int);
        }

        public static char ToChar(object o)
        {
            if (null != o)
            {
                char result;
                if (char.TryParse(o.ToString(), out result))
                {
                    return result;
                }
            }
            return default(char);
        }

        public static double ToDouble(object o)
        {
            if (null != o)
            {
                double result;
                if (double.TryParse(o.ToString(), out result))
                {
                    return result;
                }
            }
            return default(double);
        }

        public static decimal ToDecimal(object o)
        {
            if (null != o)
            {
                decimal result;
                if (decimal.TryParse(o.ToString(), out result))
                {
                    return result;
                }
            }
            return default(decimal);
        }

        public static DateTime ToDateTime(object o)
        {
            if (null != o)
            {
                DateTime result;
                if (DateTime.TryParse(o.ToString(), out result))
                {
                    return result;
                }
            }
            return default(DateTime);
        }

        public static TResult ToEnum<TResult>(object o) where TResult : struct
        {
            if (null != o)
            {
                Type type = typeof(TResult);
                if (!type.IsEnum)
                {
                    throw new NotSupportedException("TResult must be an Enum.");
                }

                TResult result;
                if (Enum.TryParse(o.ToString(), out result))
                {
                    return (TResult)System.Convert.ChangeType(result, type);
                }
            }
            return default(TResult);
        }

        public static int[] ToIntArray(byte[] array)
        {
            Debug.Assert(array.Length % 4 == 0, "This message length must multiple of 4.");
            int[] result = new int[array.Length / 4];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = BitConverter.ToInt32(array, i * 4);
            }
            array = null;
            return result;
        }

        public static byte[] ToByteArray(int[] array)
        {
            byte[] result = new byte[array.Length * 4];
            var index = 0;
            foreach (int id in array)
            {
                result[index] = (byte)(id); index++;
                result[index] = (byte)(id >> 8); index++;
                result[index] = (byte)(id >> 16); index++;
                result[index] = (byte)(id >> 24); index++;
            }
            array = null;
            return result;
        }
    }
}
