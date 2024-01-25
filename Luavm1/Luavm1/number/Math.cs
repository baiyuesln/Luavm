using System;

namespace Luavm1.number
{
    public class Math
    {
        //计算整数除法结果的方法，使用了向下取整的方式
        internal static long IFloorDiv(long a,long b)
        {
            //如果 a 和 b 都为正数，或者都为负数，或者 a 能够整除 b
            if (a>0 && b>0||a<0&&b<0||a%b==0)
            {
                return a/b;
            }
            return a / b - 1;
        }

        //计算浮点数除法结果的方法，与C sharp 一致
        internal static double FFloorDiv(double a,double b)
        {
            return System.Math.Floor(a/b);
        }

        //整数取模
        internal static long IMod(long a,long b)
        {
            return a - IFloorDiv(a, b) * b;
        }

        //浮点数取模
        internal static double FMod(double a,double b)
        {
            return a - System.Math.Floor(a / b) * b;
        }

        //按位左移函数
        internal static long ShiftLeft(long a,long n)
        {
            if (n >= 0)
            {
                return a << (int)n;
            }

            return ShiftRight(a, (int)-n);
        }

        internal static long ShiftRight(long a, long n)
        {
            if (n >= 0)
            {
                return a >> (int)n;
            }

            return ShiftLeft(a, -n);
        }
    }
}
