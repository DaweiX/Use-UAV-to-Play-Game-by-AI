using System;
using System.Collections.Generic;

namespace UAV_WPF.AI
{
    /// <summary>
    /// 直接判断当前棋盘的旋转是否在树中存在。
    /// 若是，则获取对应的最优解，然后再旋转即可
    /// </summary>
    public static class Rotate
    {
        public static int[,] RotateMat(int[,] mat, int m = 3, int n = 3)
        {
            int[,] mat1 = new int[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    mat1[i, j] = mat[n - j - 1, i];
                }
            }
            return mat1;
        }

        public static List<int[,]> GetRotateMats(int[,] mat, int m = 3, int n = 3)
        {
            // Rotate the mat for 3 times
            List<int[,]> list = new List<int[,]>();
            int[,] temp = mat;
            for (int k = 0; k < 3; k++)
            {
                temp = RotateMat(temp);
                if (list.Find(o => Equals(o, temp) == true) == null && !Equals(temp, mat))  
                    list.Add(temp);
            }

            return list;
        }

        // Judge if 2 mats is value-equal
        public static bool Equals(int[,] a, int[,] b, int m = 3, int n = 3, int precision = 1)
        {
            if (precision < 0) throw new Exception("Precision cannot < 0.");
            double test = Math.Pow(10.0, -precision);
            if (test < double.Epsilon)
                throw new Exception("The precision given is not supported.");
            for (int r = 0; r < m; r++)
                for (int c = 0; c < n; c++)
                    if (Math.Abs(a[r, c] - b[r, c]) >= test) 
                        return false;

            return true;
        }

        // Get the rotate times (src2dst)
        public static int GetRotateTimes(int[,] src, int[,] dst, int m = 3, int n = 3)
        {
            var temp = src;
            for (int i = 0; i < 3; i++)
            {
                if (Equals(temp, dst)) return i;
                temp = RotateMat(temp);
            }
            return -1;
        }

        public static bool IsNodeinTheRotates(Node a,Node b, out int times)
        {
            times = GetRotateTimes(a.State.Board, b.State.Board);
            return times >= 0;
        }

        public static void Test()
        {
            int[,] board = new int[3, 3]
            {
            { 1, 2 ,3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            };
            int[,] board2 = new int[3, 3]
            {
            { 1, 0 ,0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
            };
            Log.PrintMat(RotateMat(board2));

            var list = GetRotateMats(board2);
            foreach (var item in list)
            {
                Log.PrintMat(RotateMat(item));
            }
        }
    }
}
