using System;
using Luavm1.api;
using LuaType = System.Int32;

namespace Luavm1.state 
{
    //Access系列方法仅使用索引访问栈里存储的信息
    public partial struct LuaState
    {
        //把给定的lua类型转换从对应的字符串表示
        public string TypeName(LuaType tp)
        {
            switch (tp)
            {
                case Consts.LUA_TNONE: return "no value";
                case Consts.LUA_TNIL: return "nil";
                case Consts.LUA_TBOOLEAN: return "boolean";
                case Consts.LUA_TNUMBER: return "number";
                case Consts.LUA_TSTRING: return "string";
                case Consts.LUA_TTABLE: return "table";
                case Consts.LUA_TFUNCTION: return "function";
                case Consts.LUA_TTHREAD: return "thread";
                default: return "userdata";
            }
        }

        //根据索引返回值的类型，索引无效的话，返回LUA_TNONE
        public LuaType Type(int idx)
        {
            if (!stack.isValid(idx)) return Consts.LUA_TNONE;
            var val = stack.get(idx);
            return LuaValue.typeOf(val);
        }

        //判断给定索引的值是否属于下面类型
        public bool IsNone(int idx)
        {
            return Type(idx) == Consts.LUA_TNONE;
        }

        public bool IsNil(int idx)
        {
            return Type(idx) == Consts.LUA_TNIL;
        }

        public bool IsNoneOrNil(int idx)
        {
            return Type(idx) <= Consts.LUA_TNIL;
        }

        public bool IsBoolean(int idx)
        {
            return Type(idx) == Consts.LUA_TBOOLEAN;
        }

        public bool IsString(int idx)
        {
            var t = Type(idx);
            return t == Consts.LUA_TSTRING || t== Consts.LUA_TNUMBER;
        }

        public bool IsNumber(int idx)
        {
            return ToNumberX(idx).Item2;
        }

        //判断给定索引处的值是否是整数类型
        public bool IsInteger(int idx)
        {
            var val = stack.get(idx);
            return val.GetType().Name.Equals("Int64");
        }

        //从指定索引处取出一个bool值，如果不是bool类型，则转换
        public bool ToBoolean(int idx)
        {
            var val = stack.get(idx);
            return LuaValue.converToBoolean(val);
        }

        //从指定索引取出一个数字，如果不是数字类型并没有办法转为数字，返回0
        public double ToNumber(int idx)
        {
            return ToNumberX(idx).Item1;
        }

        //取出整数，同上
        public long ToInteger(int idx)
        {
            var val = ToIntegerX(idx);
            return val.Item1;
        }

        //从指定索引取出一个数字，如果不是数字类型并没有办法转为数字，返回是否成功报告
        public Tuple<double, bool> ToNumberX(int idx)
        {
            var val = stack.get(idx);
            return LuaValue.convertToFloat(val);
        }

        //取出整数，情况同上
        public Tuple<long, bool> ToIntegerX(int idx)
        {
            var val = stack.get(idx);
            return LuaValue.convertToInteger(val);
        }

        //取出一个字符串，同上
        public string ToString(int idx)
        {
            return ToStringX(idx).Item1;
        }

        //取出一个字符串，如果值是数字，转为字符串（修改栈）返回
        public Tuple<string,bool> ToStringX(int idx)
        {
            var val = stack.get(idx);
            switch(val.GetType().Name)
            {
                case "String":return Tuple.Create((string)val, true);
                case "Int64":
                case "Double":
                    var s = val;
                    stack.set(idx, s);
                    return Tuple.Create(Convert.ToString(s), true);
                default:return Tuple.Create("", false);
            }
        }

    }
}
