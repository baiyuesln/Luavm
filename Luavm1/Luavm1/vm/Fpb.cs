﻿namespace Luavm1.vm
{
    //Lua 现成的浮点字节编码和解码函数,用于拓展字节表示的最大大小
    public class Fpb
    {
        public static int Int2fb(int x)
        {
            var e = 0;
            if (x < 8)
            {
                return x;
            }

            for (; x >= (8 << 4);)
            {
                x = (x + 0xf) >> 4;
                e += 4;
            }

            for (; x >= (8 << 1);)
            {
                x = (x + 1) >> 1;
                e++;
            }

            return ((e + 1) << 3) | (x - 8);
        }

        public static int Fb2int(int x)
        {
            if (x < 8)
            {
                return x;
            }
            else
            {
                return ((x & 7 + 8)) << ((x >> 3) - 1);
            }
        }
    }
}