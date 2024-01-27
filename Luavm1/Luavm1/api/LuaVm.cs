

using Luavm1.binchunk;
using System.CodeDom;

namespace Luavm1.api
{
    //拓展luaState接口
    public partial interface LuaState
    {
        //返回当前PC
        int PC();

        //修改当前PC
        void AddPC(int n);

        //取出当前指令，同时递增PC，让其指向下一条指令
        uint Fetch();

        //从常量表里取出指定常量并推入栈顶
        void GetConst(int idx);

        //根据情况从常量表里提取常量或者从栈里提取值，然后推入栈顶
        void GetRK(int rk);
        LuaState New(int stackSize, Prototype proto);

    }
}
