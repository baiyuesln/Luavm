using System;

namespace Luavm1.state
{
    public partial struct LuaState
    {
        //访问指定处索引的值，取其长度，然后推入栈顶
        public void Len(int idx)
        {
            var val = stack.get(idx);
            if(new LuaValue(val).isString())
            {
                var s = new LuaValue(val).toString();
                stack.push((long)s.Length);
            }
            else if (new LuaValue(val).isLuaTable())
            {
                var t = new LuaValue(val).toLuaTable();
                stack.push((long)t.len());
            }
            else
            {
                throw new Exception("Length error!");
            }
        }

        //从栈顶弹出n个值，对这些值进行拼接，然后把结果推入栈顶
        public void Concat(int n)
        {
            if (n == 0)
            {
                stack.push("");
            }
            else if (n >= 2)
            {
                for (var i = 1; i < n; i++)
                {
                    if (IsString(-1) && IsString(-2))
                    {
                        var s2 = ToString(-1);
                        var s1 = ToString(-2);
                        stack.pop();
                        stack.pop();
                        stack.push(s1 + s2);
                        continue;
                    }

                    throw new Exception("concatenation error!");
                }
            }
        }
    }
}
