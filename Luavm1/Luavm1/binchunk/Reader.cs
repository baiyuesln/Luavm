using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luavm1.binchunk
{
    internal class Reader
    {
        //存放将要被解析的二进制Chunk数据
        public byte[] data;

        //从字节流读取一个字节
        public byte readByte()
        {
            var b = data[0];
            //忽略字节数组中的前1个元素（1个字节），并返回剩余的元素。
            data = data.Skip(1).ToArray();
            return b;
        }

        //小端方式从字节流读取一个cint存储类型（映射到C#为Uint32
        uint readUint32()
        {
            var bytes = new byte[4];
            //复制data从0开始4个字节到bytes中
            Array.ConstrainedCopy(data,0, bytes, 0, 4);
            data = data.Skip(4).ToArray();
            //判断如果当前系统字节序不是小端的话执行
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            //将字节数组的内容解释为一个 32 位无符号整数，并返回相应的 uint 值
            return BitConverter.ToUInt32(bytes,0);

        }

        //小端方式从字节流读取一个size_t存储类型（映射到C#为ulong
        private ulong readUint64()
        {
            var bytes = new byte[8];
            Array.ConstrainedCopy(data, 0, bytes, 0, 8);
            data = data.Skip(8).ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt64(bytes, 0);
        }

        //借助readUint64()方法从字节流里读取一个Lua整数（8字节映射为long)
        long readLuaInteger()
        {
            return (long)readUint64();
        }

        //借助readUint64()方法从字节流里读取一个Lua浮点数（8字节映射为double)
        double readLuaNumber()
        {
            //用于将 64 位有符号整数的比特表示转换为相应的双精度浮点数。
            return BitConverter.Int64BitsToDouble((long)readUint64());
        }

        //从字节流中读出字符串
        private string readString()
        {
            var size = (uint)readByte();
            if(size == 0)
            {
                return "";
            }
            //0xFF  255
            if(size == 0xFF)
            {
                size = (uint)readUint64();
            }
            var bytes = readBytes(size-1);
            return Bytes2String(bytes);
        }

        //从字节流里读出n个字节
        private byte[] readBytes(uint n)
        {
            var bytes = new byte[n];
            Array.ConstrainedCopy(data, 0, bytes, 0, (int)n);
            data = data.Skip((int)n).ToArray(); 
            return bytes;
        }

        //从字节流里读取并检查二进制chunk头部的各个字段，如果有误，提示
        public void checkHeader()
        {
            if(Bytes2String(readBytes(4))!= BinaryChunk.LUA_SIGNATURE)
            {
                Console.WriteLine("not a precompiled chunk!");
            }

            if(readByte()!=BinaryChunk.LUAC_VERSION)
            {
                Console.WriteLine("version mismatch!");
            }

            if(readByte() != BinaryChunk.LUAC_FORMAT)
            {
                Console.WriteLine("format mismatch!");
            }

            if(Bytes2String(readBytes(6))!= BinaryChunk.LUAC_DATA)
            {
                Console.WriteLine("corrupted!");
            }

            if(readByte()!=BinaryChunk.CINT_SIZE)
            {
                Console.WriteLine("int size mismatch!");
            }

            var b = readByte();
            if(b!=BinaryChunk.CSIZET_SIZE_32 && b != BinaryChunk.CSIZET_SIZE_64)
            {
                Console.WriteLine("size_t size mismatch!");
            }

            if(readByte()!=BinaryChunk.INSTRUCTION_SIZE)
            {
                Console.WriteLine("instruction size mismatch!");
            }

            if(readByte()!=BinaryChunk.LUA_INTEGER_SIZE)
            {
                Console.WriteLine("lua_Integer size mismatch!");
            }

            if(readByte()!=BinaryChunk.LUA_NUMBER_SIZE)
            {
                Console.WriteLine("lua_number size mismatch!");
            }

            if (readLuaInteger() != BinaryChunk.LUAC_INT)
            {
                Console.WriteLine("endianness mismatch!");
            }

            if(!readLuaNumber().Equals(BinaryChunk.LUAC_NUM))
            {
                Console.WriteLine("float format misatch!");
            }
        }

        //从字节流里读取函数原型
        public Prototype readProto(string parentSource)
        {
            var source = readString();
            if (source == "")
            {
                source = parentSource;
            }
            return new Prototype
            {
                Source = source,
                LineDefined = readUint32(),
                LastLineDefined = readUint32(),
                NumParams = readByte(),
                IsVararg = readByte(),
                MaxStackSize = readByte(),
                Code = readCode(),
                Constants = readConstants(),
                Upvalues = readUpvalues(),
                Protos = readProtos(source),
                LineInfo = readLineInfo(),
                LocVars = readLocVars(),
                UpvalueNames = readUpvalueNames()
            };
        }

        //从字节流里读取子函数原型表
        private Prototype[] readProtos(string parentSoruce)
        {
            var protos = new Prototype[readUint32()];
            for (var i = 0; i < protos.Length; i++)
            {
                protos[i] = readProto(parentSoruce);
            }
            return protos;
        }

        //从字节流里读取指令表
        private uint[] readCode()
        {
            var code = new uint[readUint32()];
            for(var i = 0; i < code.Length; i++)
            {
                code[i] = readUint32(); 
            }
            return code;
        }

        //从字节流里读取一个常量
        private object readConstant()
        {
            switch(readByte())
            {
                case BinaryChunk.TAG_NIL: return null;
                case BinaryChunk.TAG_BOOLEAN: return readByte() != 0;
                case BinaryChunk.TAG_INTEGER: return readLuaInteger();
                case BinaryChunk.TAG_NUMBER: return readLuaNumber();
                case BinaryChunk.TAG_SHORT_STR: return readString();
                case BinaryChunk.TAG_LONG_STR: return readString();
                default:throw new Exception("corrupted!");
            }
        }

        //从字节流里读取一个常量表
        private object[] readConstants()
        {
            var constants = new object[readUint32()];
            for (var i = 0; i < constants.Length; i++)
            {
                constants[i] = readConstant();
            }

            return constants;
        }


        //从字节流中读取Upvalue表
        private Upvalue[] readUpvalues()
        {
            var upvalues = new Upvalue[readUint32()];
            for(var i = 0;i < upvalues.Length; i++)
            {
                upvalues[i] = new Upvalue
                {
                    Instack = readByte(),
                    Idx = readByte()
                };
            }
            return upvalues;
        }

        //从字节流中读取行号表
        private uint[] readLineInfo()
        {
            var lineInfo = new uint[readUint32()];
            for (var i = 0; i < lineInfo.Length; i++)
            {
                lineInfo[i] = readUint32();
            }

            return lineInfo;
        }

        //从字节流中读取局部变量表
        private LocVar[] readLocVars()
        {
            var locVars = new LocVar[readUint32()];
            for (var i = 0; i < locVars.Length; i++)
            {
                locVars[i] = new LocVar
                {
                    VarName = readString(),
                    StartPC = readUint32(),
                    EndPC = readUint32()
                };
            }

            return locVars;
        }

        //从字节流中读取Upvalue名列表
        private string[] readUpvalueNames()
        {
            var names = new string[readUint32()];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = readString();
            }

            return names;
        }

        //

        public static string Bytes2String(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
