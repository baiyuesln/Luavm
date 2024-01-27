

namespace Luavm1.state
{
    //实现LuaVm中state的接口
    public partial struct LuaState
    {
        //返回pC
        public int PC()
        {
            return pc;
        }

        //修改pc
        public void AddPC(int n)
        {
            pc += n;
        }

        ////取出当前指令，同时递增PC，让其指向下一条指令
        public uint Fetch()
        {
            var i = proto.Code[pc];
            pc++;
            return i;
        }

        //从常量表里取出指定常量并推入栈顶
        public void GetConst(int idx)
        {
            var c = proto.Constants[idx];
            stack.push(c);
        }

        //根据情况从常量表里提取常量或者从栈里提取值，然后推入栈顶
        /// <summary>
        /// 传递给GetRK()方法的参数实际上是iABC模式指令里的OpArgK类型
        /// 参数。，这种类型的参数一共占9个比特。
        /// 如果最高位是1，那么参数里存放的是常量表索引
        /// ，把最高位去掉就可以得到索引值；否则最高位是0，
        /// 参数里存放的就是寄存器索引值。
        /// Lua虚拟机指令操作数里携带的寄存器索引是从0开始的，
        /// 而Lua API里的栈索引是从1开始的，
        /// 所以当需要把寄存器索引当成栈索引使用时，要对寄存器索引加1。
        /// </summary>
        /// <param name="rk"></param>
        public void GetRK(int rk)
        {
            if(rk > 0xFF)
            {
                GetConst(rk & 0xFF);
            }
            else
            {
                PushValue(rk + 1);
            }
        }
    }
}
