

namespace Luavm1.state
{
    //push系列方法用于将Lua值从外部推入栈顶
    public partial struct LuaState
    {
        public void PushNil()
        {
            stack.push(null);
        }

        public void PushBoolean(bool b)
        {
            stack.push(b);
        }

        public void PushInteger(long n)
        {
            stack.push(n);
        }

        public void PushNumber(double n)
        {
            stack.push(n);
        }

        public void PushString(string s)
        {
            stack.push(s);
        }
    }
}
