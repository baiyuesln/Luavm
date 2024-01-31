using LuaVm = Luavm1.api.LuaState;

namespace Luavm1.vm
{
    internal static class InstTable
    {
        //创建表
        internal static void newTable(Instruction i ,LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1;
            var b = abc.Item2;
            var c = abc.Item3;
            a += 1;
            vm.CreateTable(Fpb.Fb2int(b), Fpb.Fb2int(c));
            vm.Replace(a);
        }

        //根据键从表里取值
        internal static void getTable(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1;
            var b = abc.Item2;
            var c = abc.Item3;

            a += 1;
            b += 1;
            vm.GetRK(c);
            vm.GetTable(b);
            vm.Replace(a);
        }

        //根据键往表里写入值
        internal static void setTable(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1;
            var b = abc.Item2;
            var c = abc.Item3;

            a += 1;

            vm.GetRK(b);
            vm.GetRK(c);
            vm.SetTable(a);
        }

        //按索引批量更新数组元素
        internal static void setList(Instruction i, LuaVm vm)
        {
            var abc = i.ABC();
            var a = abc.Item1;
            var b = abc.Item2;
            var c = abc.Item3;

            a += 1;
            if (c > 0)
            {
                c = c - 1;
            }
            else
            {
                c = new Instruction(vm.Fetch()).Ax();
            }

            var idx = (long)c * LFIELDS_PER_FLUSH;
            for (var j = 1; j <= b; j++)
            {
                idx++;
                vm.PushValue(a + j);
                vm.SetI(a, idx);
            }
        }

        private const int LFIELDS_PER_FLUSH = 50;
    }
}
