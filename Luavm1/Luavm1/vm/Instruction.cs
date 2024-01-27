using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luavm1.vm
{
    //指令
    public class Instruction
    {
        //构造
        public Instruction(uint b)
        {
            self = b;
        }

        private uint self;

        //从指令中提取操作码
        public int Opcode()
        {
            //与00111111按位与操作，提取低6位
            return (int)(self & 0x3F);
        }

        //从iABC模式指令中提取参数
        public Tuple<int,int,int> ABC()
        {
            return Tuple.Create(
                // 表示将 self 向右位移6位,将结果转换为整数，保留结果的低8位,形成一个8位的整数.
                (int)(self >> 6) & 0xFF,
                //  23 9 9
                (int)(self >> 23) & 0x1FF,
                (int)(self >> 14) & 0x1FF
            );
        }

        //从iABx模式指令中提取参数
        public Tuple<int,int> ABx()
        {
            return Tuple.Create(
                (int)((self >> 6) & 0xFF),
                (int)(self >> 14));
        }

        //2^18 - 1，即最大的18位无符号整数
        public const int MAXARG_Bx = (1 << 18) - 1;
        //2^(18-1) - 1，即最大的17位带符号整数
        public const int MAXARG_sBx = MAXARG_Bx >> 1;

        //从iAsBx模式中提取参数
        public Tuple<int,int> AsBx()
        {
            var tuple = ABx();
            return Tuple.Create(tuple.Item1, tuple.Item2 - MAXARG_sBx);
        }

        //从iAx模式指令中提取参数
        public int Ax()
        {
            return (int)(self >> 6);
        }

        //返回指令的操作码名字
        public string OpName()
        {
            return OpCodes.opcodes[Opcode()].name;
        }

        //返回指令的编码模式
        public byte OpMode()
        {
            return OpCodes.opcodes[Opcode()].opMode;
        }

        //返回指令操作数B的使用模式
        public byte BMode()
        {
            return OpCodes.opcodes[Opcode()].argBMode;
        }
        
        //返回指令操作数C的使用模式
        public byte CMode()
        {
            return OpCodes.opcodes[Opcode()].argCMode;
        }
    }
}
