using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luavm1.vm
{
    public static class OpCodes
    {
        //编码模式
        internal const byte IABC = 0;
        internal const byte IABx = 1;
        internal const byte IAsBx = 2;
        internal const byte IAx = 3;

        //操作码
        internal const byte OP_MOVE = 0;
        internal const byte OP_LOADK = 1;
        internal const byte OP_LOADKX = 2;
        internal const byte OP_LOADBOOL = 3;
        internal const byte OP_LOADNIL = 4;
        internal const byte OP_GETUPVAL = 5;
        internal const byte OP_GETTABUP = 6;
        internal const byte OP_GETTABLE = 7;
        internal const byte OP_SETTABUP = 8;
        internal const byte OP_SETUPVAL = 9;
        internal const byte OP_SETTABLE = 10;
        internal const byte OP_NEWTABLE = 11;
        internal const byte OP_SELF = 12;
        internal const byte OP_ADD = 13;
        internal const byte OP_SUB = 14;
        internal const byte OP_MUL = 15;
        internal const byte OP_MOD = 16;
        internal const byte OP_POW = 17;
        internal const byte OP_DIV = 18;
        internal const byte OP_IDIV = 19;
        internal const byte OP_BAND = 20;
        internal const byte OP_BOR = 21;
        internal const byte OP_BXOR = 22;
        internal const byte OP_SHL = 23;
        internal const byte OP_SHR = 24;
        internal const byte OP_UNM = 25;
        internal const byte OP_BNOT = 26;
        internal const byte OP_NOT = 27;
        internal const byte OP_LEN = 28;
        internal const byte OP_CONCAT = 29;
        internal const byte OP_JMP = 30;
        internal const byte OP_EQ = 31;
        internal const byte OP_LT = 32;
        internal const byte OP_LE = 33;
        internal const byte OP_TEST = 34;
        internal const byte OP_TESTSET = 35;
        internal const byte OP_CALL = 36;
        internal const byte OP_TAILCALL = 37;
        internal const byte OP_RETURN = 38;
        internal const byte OP_FORLOOP = 39;
        internal const byte OP_FORPREP = 40;
        internal const byte OP_TFORCALL = 41;
        internal const byte OP_TFORLOOP = 42;
        internal const byte OP_SETLIST = 43;
        internal const byte OP_CLOSURE = 44;
        internal const byte OP_VARARG = 45;
        internal const byte OP_EXTRAARG = 46;

        //操作数
        internal const byte OpArgN = 0;
        internal const byte OpArgU = 1;
        internal const byte OpArgR = 2;
        internal const byte OpArgK = 3;

        //完整的指令表定
        internal static opcode[] opcodes =
        {
            //T      A         B           C         mode          name         action
            new opcode(0, 1, OpArgR, OpArgN, IABC, "MOVE    "), // R(A) := R(B)
            new opcode(0, 1, OpArgK, OpArgN, IABx, "LOADK   "), // R(A) := Kst(Bx)
            new opcode(0, 1, OpArgN, OpArgN, IABx, "LOADKX  "), // R(A) := Kst(extra arg)
            new opcode(0, 1, OpArgU, OpArgU, IABC, "LOADBOOL"), // R(A) := (bool)B; if (C) pc++
            new opcode(0, 1, OpArgU, OpArgN, IABC, "LOADNIL "), // R(A), R(A+1), ..., R(A+B) := nil
            new opcode(0, 1, OpArgU, OpArgN, IABC, "GETUPVAL"), // R(A) := UpValue[B]
            new opcode(0, 1, OpArgU, OpArgK, IABC, "GETTABUP"), // R(A) := UpValue[B][RK(C)]
            new opcode(0, 1, OpArgR, OpArgK, IABC /* */, "GETTABLE"), // R(A) := R(B)[RK(C)]
            new opcode(0, 0, OpArgK, OpArgK, IABC /* */, "SETTABUP"), // UpValue[A][RK(B)] := RK(C)
            new opcode(0, 0, OpArgU, OpArgN, IABC /* */, "SETUPVAL"), // UpValue[B] := R(A)
            new opcode(0, 0, OpArgK, OpArgK, IABC /* */, "SETTABLE"), // R(A)[RK(B)] := RK(C)
            new opcode(0, 1, OpArgU, OpArgU, IABC /* */, "NEWTABLE"), // R(A) := {} (size = B,C)
            new opcode(0, 1, OpArgR, OpArgK, IABC /* */, "SELF    "), // R(A+1) := R(B); R(A) := R(B)[RK(C)]
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "ADD     "), // R(A) := RK(B) + RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "SUB     "), // R(A) := RK(B) - RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "MUL     "), // R(A) := RK(B) * RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "MOD     "), // R(A) := RK(B) % RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "POW     "), // R(A) := RK(B) ^ RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "DIV     "), // R(A) := RK(B) / RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "IDIV    "), // R(A) := RK(B) // RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "BAND    "), // R(A) := RK(B) & RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "BOR     "), // R(A) := RK(B) | RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "BXOR    "), // R(A) := RK(B) ~ RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "SHL     "), // R(A) := RK(B) << RK(C)
            new opcode(0, 1, OpArgK, OpArgK, IABC /* */, "SHR     "), // R(A) := RK(B) >> RK(C)
            new opcode(0, 1, OpArgR, OpArgN, IABC /* */, "UNM     "), // R(A) := -R(B)
            new opcode(0, 1, OpArgR, OpArgN, IABC /* */, "BNOT    "), // R(A) := ~R(B)
            new opcode(0, 1, OpArgR, OpArgN, IABC /* */, "NOT     "), // R(A) := not R(B)
            new opcode(0, 1, OpArgR, OpArgN, IABC /* */, "LEN     "), // R(A) := length of R(B)
            new opcode(0, 1, OpArgR, OpArgR, IABC /* */, "CONCAT  "), // R(A) := R(B).. ... ..R(C)
            new opcode(0, 0, OpArgR, OpArgN, IAsBx, "JMP     "),
            new opcode(1, 0, OpArgK, OpArgK, IABC, "EQ      "),
            new opcode(1, 0, OpArgK, OpArgK, IABC, "LT      "),
            new opcode(1, 0, OpArgK, OpArgK, IABC, "LE      "),
            new opcode(1, 0, OpArgN, OpArgU, IABC, "TEST    "),
            new opcode(1, 1, OpArgR, OpArgU, IABC, "TESTSET "),
            new opcode(0, 1, OpArgU, OpArgU, IABC /* */, "CALL    "),
            new opcode(0, 1, OpArgU, OpArgU, IABC /* */, "TAILCALL"), // return R(A)(R(A+1), ... ,R(A+B-1))
            new opcode(0, 0, OpArgU, OpArgN, IABC /* */, "RETURN  "), // return R(A), ... ,R(A+B-2)
            new opcode(0, 1, OpArgR, OpArgN, IAsBx /**/, "FORLOOP "),
            new opcode(0, 1, OpArgR, OpArgN, IAsBx /**/, "FORPREP "), // R(A)-=R(A+2); pc+=sBx
            new opcode(0, 0, OpArgN, OpArgU, IABC /* */, "TFORCALL"),
            new opcode(0, 1, OpArgR, OpArgN, IAsBx /**/, "TFORLOOP"),
            new opcode(0, 0, OpArgU, OpArgU, IABC /* */, "SETLIST "),
            new opcode(0, 1, OpArgU, OpArgN, IABx /* */, "CLOSURE "),
            new opcode(0, 1, OpArgU, OpArgN, IABC /* */, "VARARG  "),
            new opcode(0, 0, OpArgU, OpArgU, IAx /*  */, "EXTRAARG"),

        };
    }

    /// <summary>
    /// 定义指令表结构体
    /// </summary>
    internal struct opcode
    {
        internal byte testFlag;
        internal byte setAFlag;
        internal byte argBMode;
        internal byte argCMode;
        internal byte opMode;
        internal string name;

        public opcode(byte testFlag, byte setAFlag, byte argBMode, byte argCMode, byte opMode, string name)
        {
            this.testFlag = testFlag;
            this.setAFlag = setAFlag;
            this.argBMode = argBMode;
            this.argCMode = argCMode;
            this.opMode = opMode;
            this.name = name;
        }
    }
}
