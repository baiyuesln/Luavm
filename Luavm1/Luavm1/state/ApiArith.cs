using System;
using Luavm1.api;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

namespace Luavm1.state
{
    //创建容纳整数参数方法和浮点数参数方法的委托
    internal delegate long IntegerFunc(long a, long b);
    internal delegate double FloatFunc(double a,double b);

    //定义操作符结构体，包含两个成员变量
    struct Operator
    {
        public IntegerFunc integerFunc;
        public FloatFunc floatFunc;
    }

    public partial struct LuaState
    {
        //创建静态Operator结构示例数组，相当于给每个对应的结构体中的委托上绑定对应的方法
        private static Operator[] operators = new Operator[]
        {
            new Operator
            {
                integerFunc = iadd,
                floatFunc = fadd
            },
            new Operator
            {
                integerFunc = isub,
                floatFunc = fsub
            },
            new Operator
            {
                integerFunc = imul,
                floatFunc = fmul
            },
            new Operator
            {
                integerFunc = imod,
                floatFunc = fmod
            },
            new Operator
            {
                integerFunc = null,
                floatFunc = pow
            },
            new Operator
            {
                integerFunc = null,
                floatFunc = div
            },
            new Operator
            {
                integerFunc = iidiv,
                floatFunc = fidiv
            },
            new Operator
            {
                integerFunc = band,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = bor,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = bxor,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = shl,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = shr,
                floatFunc = null
            },
            new Operator
            {
                integerFunc = inum,
                floatFunc = fnum
            },
            new Operator
            {
                integerFunc = bnot,
                floatFunc = null
            }
        };
        #region 定义算术运算和位运算方法
        private static long iadd(long a, long b)
        {
            return a + b;
        }

        private static double fadd(double a, double b)
        {
            return a + b;
        }

        private static long isub(long a, long b)
        {
            return a - b;
        }

        private static double fsub(double a, double b)
        {
            return a - b;
        }

        private static long imul(long a, long b)
        {
            return a * b;
        }

        private static double fmul(double a, double b)
        {
            return a * b;
        }

        private static long imod(long a, long b)
        {
            return number.Math.IMod(a, b);
        }

        private static double fmod(double a, double b)
        {
            return number.Math.FMod(a, b);
        }

        private static double pow(double a, double b)
        {
            return System.Math.Pow(a, b);
        }

        private static double div(double a, double b)
        {
            return a / b;
        }

        private static long iidiv(long a, long b)
        {
            return number.Math.IFloorDiv(a, b);
        }

        private static double fidiv(double a, double b)
        {
            return number.Math.FFloorDiv(a, b);
        }

        private static long band(long a, long b)
        {
            return a & b;
        }

        private static long bor(long a, long b)
        {
            return a | b;
        }

        private static long bxor(long a, long b)
        {
            return a ^ b;
        }

        private static long shl(long a, long b)
        {
            return number.Math.ShiftLeft(a, b);
        }

        private static long shr(long a, long b)
        {
            return number.Math.ShiftRight(a, b);
        }

        private static long inum(long a, long _)
        {
            return -a;
        }

        private static double fnum(double a, double _)
        {
            return -a;
        }

        private static long bnot(long a, long _)
        {
            //书上是^a
            return ~a;
        }
        #endregion

        /// <summary>
        /// 根据情况从栈中弹出1-2个操作数，然后按索引取出相应的
        /// operator实例，调用_arith方法计算
        /// </summary>
        /// <param name="op"></param>
        /// <exception cref="Exception"></exception>
        public void Arith(ArithOp op)
        {
            LuaValue a, b;
            //首先弹出一个值
            b = new LuaValue(stack.pop());
            //如果操作码不是 - 和 ~ ，再弹出一个值，否则一样
            if (op != Consts.LUA_OPUNM && op != Consts.LUA_OPBNOT)
            {
                a = new LuaValue(stack.pop());
            }
            else
            {
                a = b;
            }

            //按索引取出相应的operator实例
            var opr = operators[op];
            //调用_arith方法计算
            var result = _arith(a,b,opr);
            if(result != null)
            {
                stack.push(result);
            }
            else
            {
                throw new Exception("arithmetic error!");
            }
        }

        /// <summary>
        /// 对取出来的操作数通过选择operator中委托的方法
        /// 进行计算，得出结果
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        object _arith(LuaValue a,LuaValue b,Operator op)
        {
            //如果浮点数委托为null，那可能是位运算之类，需要全部转为int，再计算
            if(op.floatFunc == null)
            {
                Tuple<long,bool> v = LuaValue.convertToInteger(a.value);
                if (v.Item2)
                {
                    Tuple<long,bool> v2 = LuaValue.convertToInteger(b.value);
                    if(v2.Item2)
                    {
                        return op.integerFunc(v.Item1, v2.Item1);
                    }
                }
            }
            else
            {
                if(op.integerFunc != null)
                {
                    //如果两个操作数都是int，就用int计算
                    if (a.value.GetType().Name.Equals("Int64") && b.value.GetType().Name.Equals("Int64"))
                    {
                        var x = long.Parse(a.value.ToString());
                        var y = long.Parse(b.value.ToString());
                        return op.integerFunc(x, y);
                    }
                }

                //剩下的情况都转为float计算
                var v = LuaValue.convertToFloat(a.value);
                if (v.Item2)
                {
                    var v2 = LuaValue.convertToFloat(b.value);
                    if(v2.Item2)
                    {
                        return op.floatFunc(v.Item1, v2.Item1);
                    }
                }
            }
            return null;
        }
    }

    
}
