using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public class Writer : BinaryWriter
    {
        public long Position
        {
            get
            {
                return base.BaseStream.Position;
            }
            set
            {
                base.BaseStream.Position = value;
            }
        }

        public Writer(string file)
            : base(File.OpenWrite(file))
        {
        }

        public Writer(byte[] data)
            : base(new MemoryStream(data))
        {
        }

        public byte[] ToByteArray(long length)
        {
            if (length == -1)
            {
                length = base.BaseStream.Length;
            }
            byte[] array = new byte[length];
            Position = 0L;
            base.BaseStream.Read(array, 0, array.Length);
            return array;
        }

        public void WriteBytes(byte[] buf)
        {
            Write(buf, 0, buf.Length);
        }

        public void WriteByte(byte b)
        {
            Write(new byte[1] { b }, 0, 1);
        }

        public void Write<T>(T value, bool writeLengthForString = true, bool swap = false)
        {
            if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
            {
                byte[] bytes = Encoding.ASCII.GetBytes((string)(object)value);
                if (writeLengthForString)
                {
                    Write(bytes.Length);
                }
                WriteBytes(bytes);
                return;
            }
            byte[] array = new byte[Unsafe.SizeOf<T>()];
            Unsafe.WriteUnaligned(ref array[0], value);
            if (swap)
            {
                array = array.Reverse().ToArray();
            }
            WriteBytes(array);
        }

        public void WriteArray<T>(T[] array, bool writeLength = true)
        {
            if (writeLength)
            {
                Write(array.Length);
            }
            foreach (T value in array)
            {
                Write(value);
            }
        }

        public void WriteArray<T>(Array array)
        {
            T[] array2 = new T[array.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = (T)array.GetValue(i);
            }
            WriteArray(array2);
        }

        public void WriteMap<TKey, TValue>(Dictionary<TKey, TValue> map)
        {
            Write(map.Count);
            foreach (KeyValuePair<TKey, TValue> item in map)
            {
                Write(item.Key);
                if ((object)item.Value is Array array)
                {
                    Write(value: true);
                    WriteArray<object>(array);
                }
                else
                {
                    Write(value: false);
                    Write(item.Value);
                }
            }
        }

        public void WriteBoolean(bool value, bool longBool)
        {
            Write<bool>(value);
            WriteBytes(new byte[3]);
        }

        public void WriteFString(string fstring)
        {
            Write<int>(fstring.Length + 1);
            WriteBytes(Encoding.ASCII.GetBytes(fstring));
            WriteByte(0);
        }
    }
}