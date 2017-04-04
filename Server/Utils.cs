/**
 * @file    Utils
 * @author  Lewis
 * @url     https://github.com/Lewis-H
 * @license http://www.gnu.org/copyleft/lesser.html
 */

namespace Asterion {
    using Text = System.Text;

    /**
     * Utility methods for the Asterion server.
     */
    public static class Utils {

        /**
         * Converts a byte array to a string.
         *
         * @param convertBytes
         *  The byte array to convert into a string.
         *
         * @return
         *  The string.
         */
        public static string BytesToStr(byte[] convertBytes) {
            return Text.Encoding.ASCII.GetString(convertBytes);
        }

        /**
         * Converts a string to a byte array.
         *
         * @param convertString
         *  The string to convert into a byte array.
         *
         * @return
         *  The byte array.
         */
        public static byte[] StrToBytes(string convertString) {
            return Text.Encoding.ASCII.GetBytes(convertString);
        }

        public static string BetweenOfFixed(string ActualStr, string StrFirst, string StrLast)
        {
            int startIndex = ActualStr.IndexOf(StrFirst) + StrFirst.Length;
            int endIndex = ActualStr.IndexOf(StrLast, startIndex);
            return ActualStr.Substring(startIndex, endIndex - startIndex);
        }

        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            System.Array.Reverse(arr);
            return new string(arr);
        }

        public static string time()
        {
            return System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
    }

}
