using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using LuaType = System.Int32;

namespace Luavm1.state
{
    //实现lua虚拟机中table的get类方法
    partial struct LuaState
    {
        /// <summary>
        /// 如果无法预先估计表的用法和容量，可以使用NewTable()方法创建表。
        /// NewTable()方法只是CreateTable()方法的特例
        /// </summary>
        public void NewTable()
        {
            CreateTable(0, 0);
        }

        /// <summary>
        /// 创建一个空的Lua表，将其推入栈顶。
        /// 该方法提供了两个参数，用于指定数组部分和哈希表部分的初始容量
        /// </summary>
        public void CreateTable(int nArr,int nRec)
        {
            var t = LuaTable.newLuaTable(nArr, nRec);
            stack.push(t);
        }

        /// <summary>
        /// 根据键（从栈顶弹出）从表（索引由参数指定）里取值，
        /// 然后把值推入栈顶并返回值的类型
        /// </summary>
        public int GetTable(int idx)
        {
            var t = new LuaValue(stack.get(idx)).toLuaTable();
            var k = stack.pop();
            return getTable(new LuaValue(t), new LuaValue(k));
        }

        //根据键从表里取值
        LuaType getTable(LuaValue t, LuaValue k)
        {
            if (t.isLuaTable())
            {
                var tb1 = t.toLuaTable();
                var v = tb1.get(k).value;
                if (v.GetType().IsEquivalentTo(typeof(LuaValue)))
                {
                    v = ((LuaValue)v).value;
                }

                stack.push(v);
                return new LuaValue(v).typeOf();
            }

            throw new Exception("not a table!");
        }

        //用来从记录里获取字段的
        public LuaType GetField(int idx,string k)
        {
            var t = new LuaValue(stack.get(idx)).toLuaTable();
            return getTable(new LuaValue(t), new LuaValue(k));
        }

        //根据索引获取数组元素
        public LuaType GetI(int idx,long i)
        {
            var t = new LuaValue(stack.get(idx)).toLuaTable();
            return getTable(new LuaValue(t),new LuaValue(i));
        }
    }
}
