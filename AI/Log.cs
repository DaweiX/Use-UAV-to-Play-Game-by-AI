/* -------------------------------------
 *   The Util Functions to Get the Mat
 *              By DaweiX
 * ------------------------------------*/

using System.Diagnostics;
using System.Text;

namespace UAV_with_AI.AI
{
    public static class Log
    {
        public static string GetMatString(int[,] array)
        {
            StringBuilder builder = new StringBuilder();
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    builder.Append(array[i, j]);
                    builder.Append('\t');
                }
                builder.Append('\n');
            }
            return builder.ToString();
        }

        public static void PrintMat(int[,] array, string msg = "")
        {
            Debug.WriteLine(msg);
            Debug.WriteLine(GetMatString(array));
        }
    }
}
