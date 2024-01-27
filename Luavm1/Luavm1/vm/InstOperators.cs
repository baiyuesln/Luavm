using Luavm1.api;
using LuaVm = Luavm1.api.LuaState;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

namespace Luavm1.vm
{
    //Lua语言里的8个算术运算符和6个按位运算符分别和Lua虚拟机指令集里的14条指令一一对应
    public class InstOperators
    {
        /// <summary>
        /// 二元算术运算指令（iABC模式），
        /// 对两个寄存器或常量值（索引由操作数B和C指定）进行运算，
        /// 将结果放入另一个寄存器（索引由操作数A指定）。
        /// </summary>
        internal static void _binaryArith(Instruction i,LuaVm vm,ArithOp op)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2;
            var c = abc.Item3;

            vm.GetRK(b);
            vm.GetRK(c);
            vm.Arith(op);
            vm.Replace(a);
        }

        /// <summary>
        /// 一元算术运算指令（iABC模式），对操作数B所指定的寄存器里的值进行运算，
        /// 然后把结果放入操作数A所指定的寄存器中，操作数C没用
        /// </summary>
        internal static void _unaryArith(Instruction i, LuaVm vm, ArithOp op)
        {
            var ab_ = i.ABC();
            var a = ab_.Item1 + 1;
            var b = ab_.Item2 + 1;

            vm.PushValue(b);
            vm.Arith(op);
            vm.Replace(a);
        }

        #region 通过_binaryArith和_unaryArith实现14条算术运算指令
        internal static void add(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPADD);
        }

        internal static void sub(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPSUB);
        }

        internal static void mul(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPMUL);
        }

        internal static void mod(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPMOD);
        }

        internal static void pow(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPPOW);
        }

        internal static void div(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPDIV);
        }

        internal static void idiv(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPIDIV);
        }

        internal static void band(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPBAND);
        }

        internal static void bor(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPBOR);
        }

        internal static void bxor(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPBXOR);
        }

        internal static void shl(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPSHL);
        }

        internal static void shr(Instruction i, LuaVm vm)
        {
            _binaryArith(i, vm, Consts.LUA_OPSHR);
        }

        internal static void unm(Instruction i, LuaVm vm)
        {
            _unaryArith(i, vm, Consts.LUA_OPUNM);
        }

        internal static void bnot(Instruction i, LuaVm vm)
        {
            _unaryArith(i, vm, Consts.LUA_OPBNOT);
        }
        #endregion

        
        //LEN指令（iABC模式）进行的操作和一元算术运算指令类似   R(A) := length of R(B)
        internal static void length(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;

            vm.Len(b);
            vm.Replace(a);
        }

        /// <summary>
        /// CONCAT指令（iABC模式），
        /// 将连续n个寄存器（起止索引分别由操作数B和C指定）里的值拼接，
        /// 将结果放入另一个寄存器（索引由操作数A指定）。
        /// </summary>
        internal static void concat(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c = abc.Item3 + 1;

            var n = c - b + 1;

            vm.CheckStack(n);
            for(var l = b; l <= c; l++)
            {
                vm.PushValue(l);
            }
            vm.Concat(n);
            vm.Replace(a);
        }

        /// <summary>
        /// 比较指令（iABC模式），比较寄存器或常量表里的两个值
        /// （索引分别由操作数B和C指定），
        /// 如果比较结果和操作数A（转换为布尔值）匹配，
        /// 则跳过下一条指令。
        /// </summary>
        internal static void _compare(Instruction i, LuaVm vm,CompareOp op)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c =abc.Item3 + 1;

            vm.GetRK(b);
            vm.GetRK(c);
            if (vm.Compare(-2, -1, op) != (a != 0))
            {
                vm.AddPC(1);
            }
            vm.Pop(2);
        }

        #region 通过_compare实现3条比较运算指令
        internal static void eq(Instruction i, LuaVm vm)
        {
            _compare(i, vm, Consts.LUA_OPEQ);
        }

        internal static void lt(Instruction i, LuaVm vm)
        {
            _compare(i, vm, Consts.LUA_OPLT);
        }

        internal static void le(Instruction i, LuaVm vm)
        {
            _compare(i, vm, Consts.LUA_OPLE);
        }
        #endregion

        //NOT指令（iABC模式）进行的操作和一元算术运算指令类似
        internal static void not(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c = abc.Item3 + 1;

            vm.PushBoolean(!vm.ToBoolean(b));
            vm.Replace(a);
        }


        /// <summary>
        /// TESTSET指令（iABC模式），
        /// 判断寄存器B（索引由操作数B指定）中的值
        /// 转换为布尔值之后是否和操作数C表示的布尔值一致，
        /// 如果一致则将寄存器B中的值复制到寄存器A（索引由操作数A指定）中
        /// ，否则跳过下一条指令。
        /// </summary>
        internal static void testSet(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c = abc.Item3 + 1;

            if(vm.ToBoolean(b)==(c!=0))
            {
                vm.Copy(b, a);
            }
            else
            {
                vm.AddPC(1);
            }
        }
        /// <summary>
        /// TEST指令（iABC模式），
        /// 判断寄存器A（索引由操作数A指定）中的值转换为布尔值之后
        /// 是否和操作数C表示的布尔值一致，如果一致，则跳过下一条指令。
        /// </summary>
        internal static void test(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1 + 1;
            var b = abc.Item2 + 1;
            var c = abc.Item3 + 1;

            if (vm.ToBoolean(a) != (c != 0))
            {
                vm.AddPC(1);
            }
        }
    }
}
