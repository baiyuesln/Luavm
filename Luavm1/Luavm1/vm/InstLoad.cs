using LuaVm = Luavm1.api.LuaState;

namespace Luavm1.vm
{
    //加载指令用于把nil值、布尔值或者常量表里的常量值加载到寄存器里。
    public class InstLoad
    {
        //LOADNIL指令（i ABC模式）用于给连续n个寄存器放置nil值
        internal static void loadNil(Instruction i,LuaVm vm)
        {
            //书上是abc...
            var ab_ = i.AsBx();
            var a = ab_.Item1 + 1;
            var b = ab_.Item2;

            vm.PushNil();
            for(var l =a;l<a+b;l++)
            {
                vm.Copy(-1, l);
            }
            vm.Pop(1);
        }

        /// <summary>
        /// LOADBOOL指令（iABC模式）给单个寄存器设置布尔值。
        /// 寄存器索引由操作数A指定，布尔值由寄存器B指定
        /// （0代表false，非0代表true），如果寄存器C非0则跳过下一条指令。
        /// </summary>
        internal static void loadBool(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2;
            var c = abc.Item3;

            vm.PushBoolean(b != 0);
            vm.Replace(a);
            if (c != 0)
            {
                vm.AddPC(1);
            }
        }

        /// <summary>
        /// LOADK指令（iABx模式）将常量表里的某个常量加载到指定寄存器，
        /// 寄存器索引由操作数A指定，常量表索引由操作数Bx指定
        /// </summary>
        internal static void loadK(Instruction i, LuaVm vm)
        {
            var aBx = i.ABx();
            var a = aBx.Item1 + 1;
            var bx = aBx.Item2;
            
            vm.GetConst(bx);
            vm.Replace(a);
        }

        /// <summary>
        /// LOADKX指令（也是iABx模式）需要和EXTRAARG指令（iAx模式）搭配使用，
        /// 用后者的Ax操作数来指定常量索引。
        /// Ax操作数占26个比特，可以表达的最大无符号
        /// </summary>
        /// <param name="i"></param>
        /// <param name="vm"></param>
        internal static void loadKx(Instruction i, LuaVm vm)
        {
            var aBx = i.ABx();
            var a = aBx.Item1 + 1;
            var ax = new Instruction(vm.Fetch()).Ax();

            vm.GetConst(ax);
            vm.Replace(a);
        }
    }

    


}
