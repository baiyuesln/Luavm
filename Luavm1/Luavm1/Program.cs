using Luavm1.binchunk;
using Luavm1.vm;
using System;
using System.IO;
using Luavm1.api;

namespace Luavm1
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ////第四,五章代码测试
            //var ls = Luavm1.state.LuaState().New();
    
            //ls.PushInteger(1);
            //ls.PushString("2.0");
            //ls.PushString("3.0");
            //ls.PushNumber(4.0);
            //printStack(ls);

            //ls.Arith(Consts.LUA_OPADD);
            //printStack(ls);
            //ls.Arith(Consts.LUA_OPBNOT);
            //printStack(ls);
            //ls.Len(2);
            //printStack(ls);
            //ls.Concat(3);
            //printStack(ls);
            ////ls.PushBoolean(ls.Compare(1, 2, Consts.LUA_OPLE));  无法比较  int string
            //printStack(ls);

            //ls.PushBoolean(true);
            //printStack(ls);
            //ls.PushInteger(10);
            //printStack(ls);
            //ls.PushNil();
            //printStack(ls);
            //ls.PushString("hello");
            //printStack(ls);
            //ls.PushValue(-4);
            //printStack(ls);
            //ls.Replace(3);
            //printStack(ls);
            //ls.SetTop(6);
            //printStack(ls);
            //ls.Remove(-3);
            //printStack(ls);
            //ls.SetTop(-5);
            //printStack(ls);
            //ls.PushNumber(34.2);
            //printStack(ls);
            //ls.PushString("SS");
            //printStack(ls);
            //2,3章代码测试
            //if (args.Length <= 0) return;
            try
            {
                // 尝试打开并读取指定路径的文件（二进制文件）
                var fs = File.OpenRead("D:\\LUACS\\lua\\ch02\\luac.out");
                var data = new byte[fs.Length];
                // 从文件流中读取数据到字节数组
                fs.Read(data, 0, data.Length);
                var proto = BinaryChunk.Undump(data);
                list(proto);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //把栈的内容打印出来
        internal static void printStack(LuaState ls)
        {
            var top = ls.GetTop();
            for (int i = 1; i <= top; i++)
            {
                var t = ls.Type(i);
                switch(t)
                {
                    case Consts.LUA_TBOOLEAN:
                        Console.Write($"[{ls.ToBoolean(i)}]");
                        break;
                    case Consts.LUA_TNUMBER:
                        Console.Write($"[{ls.ToNumber(i)}]");
                        break;
                    case Consts.LUA_TSTRING:
                        Console.Write($"[{ls.ToString(i)}]");
                        break;
                    default:
                        Console.Write($"[{ls.TypeName(t)}]");
                        break;
                }
            }
            Console.WriteLine();
        }

        //把函数原型信息打印打控制台
        private static void list(Prototype f)
        {
            printHeader(f);
            printCode(f);
            printDetail(f);
            foreach (var p in f.Protos)
            {
                list(p);
            }
        }

        private static void printHeader(Prototype f)
        {
            var funcType = "main";
            if (f.LineDefined > 0)
            {
                funcType = "function";
            }

            var varargFlag = "";
            if (f.IsVararg > 0)
            {
                varargFlag = "+";
            }

            Console.Write("\n{0} <{1}:{2},{3}> ({4} instruction)\n", funcType, f.Source, f.LineDefined,
                f.LastLineDefined, f.Code.Length);
            Console.Write("{0,1} params, {2} slots, {3} upvalues, ", f.NumParams, varargFlag, f.MaxStackSize,
                f.Upvalues.Length);
            Console.Write("{0} locals, {1} constants, {2} functions\n", f.LocVars.Length, f.Constants.Length,
                f.Protos.Length);
        }

        private static void printCode(Prototype f)
        {
            for (var pc = 0; pc < f.Code.Length; pc++)
            {
                var c = f.Code[pc];
                var line = "-";
                if (f.LineInfo.Length > 0)
                {
                    line = f.LineInfo[pc].ToString();
                }

                var i = new Instruction(c);
                Console.Write("\t{0}\t[{1}]\t{2:x8} \t", pc + 1, line, i.OpName());
                printOperands(i);
                Console.WriteLine();
                //Console.Write("\t{0}\t[{1}]\t0x{2:x8}\n", pc + 1, line, c);
            }
        }

        //打印指令的操作数
        private static void printOperands(Instruction i)
        {
            int a, b, c, ax, bx, sbx;
            switch (i.OpMode())
            {
                case OpCodes.IABC:
                    var abc = i.ABC();
                    Console.Write($"{abc.Item1:D}",abc.Item1);
                    if (i.BMode() != OpCodes.OpArgN)
                    {
                        if (abc.Item2 > 0xFF)
                        {
                            Console.Write($" {-1 - (abc.Item2&0xFF):D}");
                        }
                        else
                        {
                            Console.Write($" {abc.Item2:D}");
                        }
                    }
                    if (i.CMode() != OpCodes.OpArgN)
                    {
                        if (abc.Item3 > 0xFF)
                        {
                            Console.Write($"{-1 - (abc.Item3 & 0xFF):D}");
                        }
                        else
                        {
                            Console.Write($" {abc.Item3:D}");
                        }
                    }
                    break;
                case OpCodes.IABx:
                    var aBx = i.ABx();
                    Console.Write($" {aBx.Item1:D}");
                    if (i.BMode() == OpCodes.OpArgK)
                    {
                        Console.Write($"{-1 - aBx.Item2:D}");
                    }
                    else if (i.BMode() == OpCodes.OpArgU)
                    {
                        Console.Write($" {aBx.Item2:D}");
                    }

                    break;
                case OpCodes.IAsBx:
                    var asBx = i.AsBx();
                    Console.Write($"{asBx.Item1:D} {asBx.Item2:D}");
                    break;
                case OpCodes.IAx:
                    ax = i.Ax();
                    Console.Write($"{-1 - ax:D}");
                    break;
            }
        }

        private static void printDetail(Prototype f)
        {
            Console.Write("constants ({0}):\n", f.Constants.Length);
            for (var i = 0; i < f.Constants.Length; i++)
            {
                var k = f.Constants[i];
                Console.Write("\t{0}\t{1}\n", i + 1, constantToString(k));
            }

            Console.Write("locals ({0}):\n", f.LocVars.Length);
            for (var i = 0; i < f.LocVars.Length; i++)
            {
                var locVar = f.LocVars[i];
                Console.Write("\t{0}\t{1}\t{2}\t{3}\n", i, locVar.VarName, locVar.StartPC + 1, locVar.EndPC + 1);
            }

            Console.Write("upvalues ({0}):\n", f.Upvalues.Length);
            for (var i = 0; i < f.Upvalues.Length; i++)
            {
                var upval = f.Upvalues[i];
                Console.Write("\t{0}\t{1}\t{2}\t{3}\n", i, upvalName(f, i), upval.Instack, upval.Idx);
            }
        }

        //把常量表转化
        private static object constantToString(object k)
        {
            if (k == null)
            {
                return "nil";
            }

            switch (k.GetType().Name)
            {
                case "Boolean": return (bool)k;
                case "Double": return (double)k;
                case "Long": return (long)k;
                case "String": return (string)k;
                default: return "?";
            }
        }

        //根据索引从调试信息里找出Upvalue的名字
        private static string upvalName(Prototype f, int idx)
        {
            return f.UpvalueNames.Length > 0 ? f.UpvalueNames[idx] : "-";
        }
    }
}
