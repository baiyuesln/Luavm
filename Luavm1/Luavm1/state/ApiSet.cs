using System;

namespace Luavm1.state
{
    partial struct LuaState
    {
        /// <summary>
        /// 把键值对写入表。其中键和值从栈里弹出，表则位于指定索引处
        /// </summary>
        public void SetTable(int idx)
        {
            var v = stack.pop();
            var k = stack.pop();
            stack.setTable(idx,new LuaValue(k),new LuaValue(v));
        }

        //键是由参数传入的字符串。用于给记录的字段赋值。
        public void SetField(int idx, string k)
        {
            var t = stack.get(idx);
            var v = stack.pop();
            stack.setTable(idx, new LuaValue(k), new LuaValue(v));
        }

        //参数传入的键是数字而非字符串
        public void SetI(int idx, long n)
        {
            var t = stack.get(idx);
            var v = stack.pop();
            stack.setTable(idx, new LuaValue(n), new LuaValue(v));
        }
    }
}
