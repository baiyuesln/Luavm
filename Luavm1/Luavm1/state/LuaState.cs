using System;
using System.Runtime.InteropServices;

namespace Luavm1.state
{
    //定义luaState结构体
    public partial struct LuaState : api.LuaState
    {
        private LuaStack stack;

        public int LuaType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //创建luaState实例
        public static api.LuaState New()
        {
            return new LuaState
            {
                stack = LuaStack.newLuaStack(20)
            };
        }

        #region  基础栈操纵方法
        //返回栈顶索引
        public int GetTop()
        {
            return stack.top;
        }

        //把索引转换为绝对索引
        public int AbsIndex(int idx)
        {
            return stack.absIndex(idx);
        }

        //检查栈剩余空间是否支持推入n个值
        public bool CheckStack(int n)
        {
            stack.check(n);
            //暂时忽略扩容失败
            return true;
        }

        //从栈顶弹出n个值
        public void Pop(int n)
        {
            SetTop(-n-1);
        }

        //把值从一个位置复制到另一个位置
        public void Copy(int fromIdx,int toIdx)
        {
            var val = stack.get(fromIdx);
            stack.set(toIdx, val);
        }

        //把指定索引处的值推入栈顶
        public void PushValue(int idx)
        {
            var val = stack.get(idx);
            stack.push(val);
        }

        //将栈顶值弹出，写入指定位置
        public void Replace(int idx)
        {
            var val = stack.pop();
            stack.set(idx,val);
        }
        
        //将栈顶值弹出并插入到指定位置
        public void Insert(int idx)
        {
            Rotate(idx, 1);
        }

        //删除指定索引处的值，然后将值上面的值全部下移一个位置
        public void Remove(int idx)
        {
            Rotate(idx,-1);
            Pop(1);
        }

        //将[idx,n]索引区间的值朝栈顶方向旋转n个位置，通过三次reverse实现
        public void Rotate(int idx, int n)
        {
            var t = stack.top - 1;
            var p = stack.absIndex(idx) - 1;
            int m;
            if(n >= 0)
            {
                m = t - n;
            }
            else
            {
                m = p - n - 1;
            }
            stack.reverse(p, m);
            stack.reverse(m + 1, t);
            stack.reverse(p, t);
        }

        /// <summary>
        /// 将栈顶索引设置为指定位置，
        /// 如果指定位置小于前栈顶索引，就把前面的弹出
        /// 如果大于，则推入多个nil
        /// </summary>
        /// <param name="idx"></param>
        public void SetTop(int idx)
        {
            var newTop = stack.absIndex(idx);
            if(newTop < 0)
            {
                throw new Exception("stack overflow!(SetTop)");
            }

            var n = stack.top - newTop;
            if (n > 0)
            {
                for(var i = 0;i < n; i++)
                {
                    stack.pop();
                }
            }
            else if(n<0)
            {
                for(var i = 0; i < n; i++)
                {
                    stack.push(null);
                }
            }
        }

        public void Arith(int op)
        {
            throw new NotImplementedException();
        }

        public bool Compare(int idx1, int idx2, int op)
        {
            throw new NotImplementedException();
        }

        public void Len(int idx)
        {
            throw new NotImplementedException();
        }

        public void Concat(int n)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
