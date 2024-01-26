using Luavm1.api;
using System;
using LuaType = System.Int32;

namespace Luavm1.state
{
    /// <summary>
    /// 定义用于表示Lua值的luaValue类型
    /// </summary>
    public class LuaValue
    {
        //创建一个通用的 Lua 值对象，可以存储任何类型的值。
        internal readonly object value;

        public LuaValue(object value)
        {
            this.value = value;
        }

        //根据变量值返回其类型
        internal static LuaType typeOf(object val)
        {
            if(val == null)
            {
                return Consts.LUA_TNIL;
            }

            switch(val.GetType().Name)
            {
                case "Boolean": return Consts.LUA_TBOOLEAN;
                case "Double": return Consts.LUA_TNUMBER;
                case "Int64": return Consts.LUA_TNUMBER;
                case "String": return Consts.LUA_TSTRING;
                default:throw new Exception("type todo!");
            }
        }

        //将类型转为bool
        internal static bool converToBoolean(object val)
        {
            if (val == null)
            {
                return false;
            }
            switch (val.GetType().Name)
            {
                case "Boolean": return (bool)val;
                default: return true;
            }
        }

        internal static Tuple<double,bool> convertToFloat(object val)
        {
            switch(val.GetType().Name)
            {
                case "Double":return Tuple.Create((double)val, true);
                case "Int64": return Tuple.Create(Convert.ToDouble(val), true);
                case "String":return number.Parser.ParseFloat((string)val);
                default: return Tuple.Create(0d, false);
            }
        }

        internal static Tuple<long, bool> convertToInteger(object val)
        {
            switch (val.GetType().Name)
            {
                case "Int64": return Tuple.Create<long, bool>((long)val, true);
                case "Double":return number.Math.FloatToInteger((double)val);
                case "String": return Tuple.Create(Convert.ToInt64(val), true);
                default: return Tuple.Create(0L, false);
            }
        }

        //string解析为整数，如果不行解析为浮点数后再转为整数
        private Tuple<long,bool> _stringToInteger(string s)
        {
            var v = number.Parser.ParseInteger(s);
            if (v.Item2)
            {
                return Tuple.Create(v.Item1,true);
            }
            var v2 = number.Parser.ParseFloat(s);
            if (v2.Item2)
            {
                return number.Math.FloatToInteger(v2.Item1);
            }

            return Tuple.Create<long, bool>(0L, false);
        }
    }
}
