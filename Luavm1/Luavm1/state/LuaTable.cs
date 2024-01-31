using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using LuaType = System.Int32;
using Math = Luavm1.number.Math;

namespace Luavm1.state
{
    /// <summary>
    /// 使用数组和哈希表的混合方式来实现Lua表
    /// </summary>
    public struct LuaTable
    {
        private LuaValue[] arr;
        private Dictionary<object, LuaValue> _map;


        /// <summary>
        /// newLuaTable()函数创建一个空的表。
        /// 该函数接受两个参数，用于预估表的用途和容量。
        /// 如果参数nArr大于0，说明表可能是当作数组使用的，
        /// 先创建数组部分；
        /// 如果参数nRec大于0，说明表可能是当作记录使用的，
        /// 先创建哈希表部分。
        /// </summary>
        public static LuaTable newLuaTable(int nArr,int nRec)
        {
            var t = new LuaTable();
            if(nArr > 0)
            {
                t.arr = new LuaValue[nArr];
            }
            if(nRec > 0)
            {
                t._map = new Dictionary<object, LuaValue>(nRec);
            }

            return t;
        }

        /// <summary>
        /// 根据键从表里查找值。
        /// 如果键是整数（或者能够转换为整数的浮点数），
        /// 且在数组索引范围之内，直接按索引访问数组部分就可以了；
        /// 否则从哈希表查找值。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LuaValue get(LuaValue key)
        {
            key = _floatToInteger(key);
            if (key.isInteger())
            {
                var idx = key.toInteger();
                if (idx >= 1 && idx <= arr.Length)
                {
                    return new LuaValue(arr[idx - 1]);
                }
            }

            return new LuaValue(_map[key.value]);
        }

        //尝试把浮点数类型的键转换成整数
        LuaValue _floatToInteger(LuaValue key)
        {
            if (key.isFloat())
            {
                var f = key.toFloat();
                return new LuaValue(Math.FloatToInteger(f).Item1);
            }

            return key;
        }

        public int len()
        {
            return arr.Length;
        }

        //往表里存入键值对
        public void put(LuaValue key,LuaValue val)
        {
            if(key == null)
            {
                throw new Exception("table index is nil!");
            }

            if(key.isFloat() && double.IsNaN(key.toFloat()))
            {
                throw new Exception("table index is NaN!");
            }

            if (key.isInteger())
            {
                var idx = key.toInteger();
                if(idx >= 1)
                {
                    var arrlen = arr.Length;
                    if (idx <= arrlen)
                    {
                        arr[idx - 1] = val;
                        if(idx == arrlen && val.value == null)
                        {
                            _shrinkArray();
                        }

                        return;
                    }

                    if(idx == arrlen + 1)
                    {
                        _map.Remove(idx);
                        if(val != null)
                        {
                            var b = arr.ToList();
                            b.Add(val);
                            arr = b.ToArray();
                            _expandArray();
                        }
                        return;
                    }
                }
            }

            if(val != null)
            {
                if(_map == null)
                {
                    _map = new Dictionary<object, LuaValue>(8);
                }

                _map.Add(key.value, val);
            }
            else
            {
                _map.Remove(key.value);
            }
        }

        //动态扩展数组
        private void _expandArray()
        {
            for(var idx = arr.Length + 1; true; idx++)
            {
                if (_map.ContainsKey(idx))
                {
                    var val = _map.Values.ElementAt(idx);
                    _map.Remove(idx);
                    var b = arr.ToList();
                    b.Add(val);
                    arr = b.ToArray();
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 向数组里放入nil值会制造洞，如果洞在数组末尾的话，
        /// 调用_shrinkArray()函数把尾部的洞全部删除。
        /// </summary>
        void _shrinkArray()
        {
            for(var i = arr.Length - 1; i >= 0; i--)
            {
                if (arr[i] == null)
                {
                    Array.Copy(arr, 0, arr, 0, i);
                }
            }
        }
    }
}
