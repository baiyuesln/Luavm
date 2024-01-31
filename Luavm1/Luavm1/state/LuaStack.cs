using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luavm1.state
{
    /// <summary>
    /// 定义lua 栈结构体
    /// </summary>
    public struct LuaStack
    {
        //存放值
        private LuaValue[] slots;
        //记录栈顶索引
        internal int top;

        //创建指定容量的栈
        internal static LuaStack newLuaStack(int size)
        {
            return new LuaStack
            {
                slots = new LuaValue[size],
                top = 0
            };
        }

        //检查栈 的空闲空间是否还可以容纳至少n个值,不满足就扩容
        internal void check(int n)
        {
            var free = slots.Length - top;
            if (n <= free) return;
            var newSlots = new LuaValue[top + n];
            Array.Copy(slots, newSlots, slots.Length);
            slots = newSlots;
        }

        //将值推入栈顶，如果溢出，抛出异常
        internal void push(object val)
        {
            if(top ==slots.Length)
            {
                throw new Exception("stack overflow!");
            }

            slots[top] = new LuaValue(val);
            top++;
        }

        //从栈顶弹出一个值，如果栈是空的，抛出异常
        internal object pop()
        {
            if(top<1)
            {
                throw new Exception("stack ovarflow!");
            }
            top--;
            var val = slots[top].value;
            slots[top] = null;
            return val;
        }

        //把索引转换成绝对索引(将负数索引转换为对应的绝对索引负数（从-1开始，表示从栈顶开始的位置）)
        internal int absIndex(int idx)
        {
            if(idx>0)
            {
                return idx;
            }

            return idx + top + 1;
        }

        //判断索引是否有效
        internal bool isValid(int idx)
        {
            var absIdx = absIndex(idx);
            return absIdx> 0 && absIdx <= top;
        }

        //根据索引从栈里取值，如果索引无效返回nil值
        internal object get(int idx)
        {
            var absIdx = absIndex(idx);
            if(absIdx>0&&absIdx <= top)
            {
                return slots[absIdx - 1].value;
            }
            return null;
        }

        //根据索引往栈里写入值，如果索引无效，抛出异常
        internal void set(int idx, object val)
        {
            var absIdx = absIndex(idx);
            if (absIdx <= 0 || absIdx > top) throw new Exception("invalid index!");
            slots[absIdx - 1] = new LuaValue(val);
        }

        //反转from和to之间的栈值
        internal void reverse(int from,int to)
        {
            if (to > from)
            {
                System.Array.Reverse(slots, from, to-from+1);
            }
            else if(to < from)
            {
                System.Array.Reverse(slots,to,from-to +1);
            }
        }

        //写表逻辑抽取成SetTable()方法
        internal void setTable(int idx, LuaValue k, LuaValue v)
        {
            var t = get(idx);
            if (new LuaValue(t).isLuaTable())
            {
                var tbl = (LuaTable)t;
                tbl.put(k, v);
                set(idx, tbl);
                return;
            }

            throw new Exception("not a table!");
        }
    }
}
