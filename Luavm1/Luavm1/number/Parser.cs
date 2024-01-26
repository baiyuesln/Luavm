using System;


namespace Luavm1.number
{
    public class Parser
    {
        //字符串解析为整数
        internal static Tuple<long,bool> ParseInteger(string str)
        {
            try
            {
                var i = Convert.ToInt64(str);
                return Tuple.Create(i, true);
            }
            catch (Exception e)
            {
                return Tuple.Create(0L,false);
            }
        }

        //字符串解析为浮点数
        internal static Tuple<double, bool> ParseFloat(string str)
        {
            try
            {
                var i = Convert.ToDouble(str);
                return Tuple.Create(i, true);
            }
            catch (Exception e)
            {
                return Tuple.Create(0D, false);
            }
        }
    }
}
