using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public class Reader : BinaryReader
    {
        public long Length => base.BaseStream.Length;

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

        public Reader(string file)
            : base(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        public Reader(byte[] data)
            : base(new MemoryStream(data))
        {
        }

        public new byte[] ReadBytes(int length)
        {
            byte[] array = new byte[length];
            Read(array, 0, length);
            return array;
        }

        public T Read<T>()
        {
            if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
            {
                int length = Read<int>();
                return (T)(object)Encoding.ASCII.GetString(ReadBytes(length));
            }
            return Unsafe.ReadUnaligned<T>(ref ReadBytes(Unsafe.SizeOf<T>())[0]);
        }

        public T[] ReadArray<T>()
        {
            T[] array = new T[Read<int>()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Read<T>();
            }
            return array;
        }

        public T[] ReadArray<T>(int length)
        {
            T[] array = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Read<T>();
            }
            return array;
        }

        public static T ReadStruct<T>(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[Marshal.SizeOf<T>()];
            Buffer.BlockCopy(buffer, offset, bytes, 0, Marshal.SizeOf<T>());
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            return data;
        }

        public Dictionary<TKey, TValue> ReadMap<TKey, TValue>()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            int num = Read<int>();
            for (int i = 0; i < num; i++)
            {
                TKey key = Read<TKey>();
                bool flag = Read<bool>();
                dictionary.Add(key, flag ? ((TValue)(object)ReadArray<TValue>()) : Read<TValue>());
            }
            return dictionary;
        }
    }
}