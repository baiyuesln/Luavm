using System;
using LuaVm = Luavm1.api.LuaState;
namespace Luavm1.vm
{
    //存放Lua虚拟机指令MOVE和JMP
    public class InstMisc
    {
        //实现move指令
        internal static void move(Instruction i ,LuaVm vm)
        {
            var ab_ = i.ABC();
            //寄存器索引加1才是相应的栈索引
            var a = ab_.Item1 + 1;
            var b = ab_.Item2;
            vm.Copy(b,a);
        }

        //实现jmp指令
        internal static void jmp(Instruction i ,LuaVm vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1;
            var sBx = asBx.Item2;

            vm.AddPC(sBx);
            if (a != 0)
            {
                throw new Exception("todo!");
            }
        }
    }
}
