using Luavm1.api;
using LuaVm = Luavm1.api.LuaState;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

namespace Luavm1.vm
{
    //数值for循环相关指令，数值for循环用于按一定步长遍历某个范围内的数值
    //sealed,表示不能被其他类继承
    public sealed class InstFor
    {
        /// <summary>
        /// FORPREP指令执行的操作其实就是在循环开始之前预先给数值减去步长
        /// ，然后跳转到FORLOOP指令正式开始循环
        /// </summary>
        internal static void forPrep(Instruction i,LuaVm vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1 + 1;
            var sBx = asBx.Item2;

            vm.PushValue(a);
            vm.PushValue(a + 2);
            vm.Arith(Consts.LUA_OPSUB);
            vm.Replace(a);
            vm.AddPC(a);
        }

        /// <summary>
        /// FORLOOP指令则是先给数值加上步长，
        /// 然后判断数值是否还在范围之内。
        /// 如果已经超出范围，则循环结束；
        /// 若未超过范围则把数值拷贝给用户定义的局部变量，
        /// 然后跳转到循环体内部开始执行具体的代码块。
        /// </summary>
        internal static void forLoop(Instruction i,LuaVm vm)
        {
            var asBx = i.AsBx();
            var a = asBx.Item1 + 1;
            var sBx = asBx.Item2;

            //R(A)+=R(A+2);
            vm.PushValue(a + 2);
            vm.PushValue(a);
            vm.Arith(Consts.LUA_OPADD);
            vm.Replace(a);

            var isPositiveStep = vm.ToNumber(a + 2) >= 0;
            if(
                (isPositiveStep && vm.Compare(a,a+1,Consts.LUA_OPLE)) ||
                (!isPositiveStep && vm.Compare(a+1,a,Consts.LUA_OPLE)))
            {
                //pc+=sBx;R(A+3) = R(A)
                vm.AddPC(sBx);
                vm.Copy(a, a + 3);
            }
        }
    }
}
