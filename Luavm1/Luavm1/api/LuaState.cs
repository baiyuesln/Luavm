﻿using System;
using LuaType = System.Int32;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

namespace Luavm1.api
{
    //定义luaState接口
    public partial interface LuaState
    {
        int LuaType { get; set; }
        int GetTop();
        int AbsIndex(int idx);
        bool CheckStack(int n);
        void Pop(int n);
        void Copy(int fromIdx, int toIdx);
        void PushValue(int idx);
        void Replace(int idx);
        void Insert(int idx);
        void Remove(int idx);
        void Rotate(int idx, int n);
        void SetTop(int idx);

        /* access functions (stack->c#) */
        string TypeName(LuaType tp);
        LuaType Type(int idx);
        bool IsNone(int idx);
        bool IsNil(int idx);
        bool IsNoneOrNil(int idx);
        bool IsBoolean(int idx);
        bool IsInteger(int idx);
        bool IsNumber(int idx);
        bool IsString(int idx);
        bool ToBoolean(int idx);
        long ToInteger(int idx);
        Tuple<long, bool> ToIntegerX(int idx);
        double ToNumber(int idx);
        Tuple<double, bool> ToNumberX(int idx);
        string ToString(int idx);
        Tuple<string, bool> ToStringX(int idx);

        /* push functions (c# -> stack) */
        void PushNil();
        void PushBoolean(bool b);
        void PushInteger(long n);
        void PushNumber(double n);
        void PushString(string s);

        //执行算术和按位运算
        void Arith(ArithOp op);

        //执行比较运算
        bool Compare(int idx1, int idx2, CompareOp op);

        //执行长度运算
        void Len(int idx);

        //执行字符串拼接运算
        void Concat(int n);

        //表操作API ---->get类
        void NewTable();

        void CreateTable(int nArr, int nRec);

        LuaType GetTable(int idx);

        LuaType GetField(int idx, string k);

        LuaType GetI(int idx, long i);

        //表操作API ---->set类
        void SetTable(int idx);

        void SetField(int idx, string k);

        void SetI(int idx, long n);
    }
}
