using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Hashes
{
    public static class CityHash
    {
        // Copyright (c) 2011 Google, Inc.
        //
        // Permission is hereby granted, free of charge, to any person obtaining a copy
        // of this software and associated documentation files (the "Software"), to deal
        // in the Software without restriction, including without limitation the rights
        // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is
        // furnished to do so, subject to the following conditions:
        //
        // The above copyright notice and this permission notice shall be included in
        // all copies or substantial portions of the Software.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
        // THE SOFTWARE.
        //
        // CityHash, by Geoff Pike and Jyrki Alakuijala
        //
        // This file provides CityHash64() and related functions.
        // https://raw.githubusercontent.com/Tamely/SaturnSwapper/master/UAssetAPI/CityHash.cs

        private const ulong K0 = 14097894508562428199uL;

        private const ulong K1 = 13011662864482103923uL;

        private const ulong K2 = 11160318154034397263uL;

        public unsafe static ulong Hash(byte[] buffer)
        {
            uint num = (uint)buffer.Length;
            fixed (byte* ptr = buffer)
            {
                byte* ptr2 = ptr;
                switch (num)
                {
                    case 17u:
                    case 18u:
                    case 19u:
                    case 20u:
                    case 21u:
                    case 22u:
                    case 23u:
                    case 24u:
                    case 25u:
                    case 26u:
                    case 27u:
                    case 28u:
                    case 29u:
                    case 30u:
                    case 31u:
                    case 32u:
                        return HashLen17to32(ptr2, num);
                    case 0u:
                    case 1u:
                    case 2u:
                    case 3u:
                    case 4u:
                    case 5u:
                    case 6u:
                    case 7u:
                    case 8u:
                    case 9u:
                    case 10u:
                    case 11u:
                    case 12u:
                    case 13u:
                    case 14u:
                    case 15u:
                    case 16u:
                        return HashLen0to16(ptr2, num);
                    case 33u:
                    case 34u:
                    case 35u:
                    case 36u:
                    case 37u:
                    case 38u:
                    case 39u:
                    case 40u:
                    case 41u:
                    case 42u:
                    case 43u:
                    case 44u:
                    case 45u:
                    case 46u:
                    case 47u:
                    case 48u:
                    case 49u:
                    case 50u:
                    case 51u:
                    case 52u:
                    case 53u:
                    case 54u:
                    case 55u:
                    case 56u:
                    case 57u:
                    case 58u:
                    case 59u:
                    case 60u:
                    case 61u:
                    case 62u:
                    case 63u:
                    case 64u:
                        return HashLen33to64(ptr2, num);
                    default:
                        {
                            ulong num2 = Fetch64(ptr2 + num - 40);
                            ulong num3 = Fetch64(ptr2 + num - 16) + Fetch64(ptr2 + num - 56);
                            ulong num4 = HashLen16(Fetch64(ptr2 + num - 48) + num, Fetch64(ptr2 + num - 24));
                            (ulong, ulong) tuple = WeakHashLen32WithSeeds(ptr2 + num - 64, num, num4);
                            (ulong, ulong) tuple2 = WeakHashLen32WithSeeds(ptr2 + num - 32, num3 + 13011662864482103923uL, num2);
                            num2 = (ulong)((long)num2 * -5435081209227447693L) + Fetch64(ptr2);
                            num = (uint)((num - 1) & -64);
                            do
                            {
                                num2 = Rotate(num2 + num3 + tuple.Item1 + Fetch64(ptr2 + 8), 37) * 13011662864482103923uL;
                                num3 = Rotate(num3 + tuple.Item2 + Fetch64(ptr2 + 48), 42) * 13011662864482103923uL;
                                num2 ^= tuple2.Item2;
                                num3 += tuple.Item1 + Fetch64(ptr2 + 40);
                                num4 = Rotate(num4 + tuple2.Item1, 33) * 13011662864482103923uL;
                                tuple = WeakHashLen32WithSeeds(ptr2, tuple.Item2 * 13011662864482103923uL, num2 + tuple2.Item1);
                                tuple2 = WeakHashLen32WithSeeds(ptr2 + 32, num4 + tuple2.Item2, num3 + Fetch64(ptr2 + 16));
                                num4 ^= num2;
                                num2 ^= num4;
                                num4 ^= num2;
                                ptr2 += 64;
                                num -= 64;
                            }
                            while (num != 0);
                            return HashLen16((ulong)((long)HashLen16(tuple.Item1, tuple2.Item1) + (long)ShiftMix(num3) * -5435081209227447693L) + num4, HashLen16(tuple.Item2, tuple2.Item2) + num2);
                        }
                }
            }
        }

        public static ulong Hash(string content)
        {
            return Hash(Encoding.ASCII.GetBytes(content));
        }

        private unsafe static ulong HashLen0to16(byte* s, uint len)
        {
            switch (len)
            {
                default:
                    {
                        ulong num5 = (ulong)(-7286425919675154353L + len * 2);
                        ulong num6 = Fetch64(s) + 11160318154034397263uL;
                        ulong num7 = Fetch64(s + len - 8);
                        ulong u = Rotate(num7, 37) * num5 + num6;
                        ulong v = (Rotate(num6, 25) + num7) * num5;
                        return HashLen16(u, v, num5);
                    }
                case 4u:
                case 5u:
                case 6u:
                case 7u:
                    {
                        ulong mul = (ulong)(-7286425919675154353L + len * 2);
                        ulong num4 = Fetch32(s);
                        return HashLen16(len + (num4 << 3), Fetch32(s + len - 4), mul);
                    }
                case 1u:
                case 2u:
                case 3u:
                    {
                        byte num = *s;
                        byte b = s[len >> 1];
                        byte b2 = s[len - 1];
                        int num2 = num + (b << 8);
                        uint num3 = len + (uint)(b2 << 2);
                        return ShiftMix((ulong)(((uint)num2 * -7286425919675154353L) ^ (num3 * -4348849565147123417L))) * 11160318154034397263uL;
                    }
                case 0u:
                    return 11160318154034397263uL;
            }
        }

        private unsafe static ulong HashLen17to32(byte* s, uint len)
        {
            ulong num = (ulong)(-7286425919675154353L + len * 2);
            ulong num2 = Fetch64(s) * 13011662864482103923uL;
            ulong num3 = Fetch64(s + 8);
            ulong num4 = Fetch64(s + len - 8) * num;
            ulong num5 = Fetch64(s + len - 16) * 11160318154034397263uL;
            return HashLen16(Rotate(num2 + num3, 43) + Rotate(num4, 30) + num5, num2 + Rotate(num3 + 11160318154034397263uL, 18) + num4, num);
        }

        private unsafe static ulong HashLen33to64(byte* s, uint len)
        {
            ulong num = (ulong)(-7286425919675154353L + len * 2);
            ulong num2 = Fetch64(s) * 11160318154034397263uL;
            ulong num3 = Fetch64(s + 8);
            ulong num4 = Fetch64(s + len - 24);
            ulong num5 = Fetch64(s + len - 32);
            long num6 = (long)Fetch64(s + 16) * -7286425919675154353L;
            ulong num7 = Fetch64(s + 24) * 9;
            ulong num8 = Fetch64(s + len - 8);
            ulong num9 = Fetch64(s + len - 16) * num;
            ulong num10 = Rotate(num2 + num8, 43) + (Rotate(num3, 30) + num4) * 9;
            ulong num11 = ((num2 + num8) ^ num5) + num7 + 1;
            ulong num12 = Bswap_64((num10 + num11) * num) + num9;
            ulong num13 = Rotate((ulong)num6 + num7, 42) + num4;
            ulong num14 = (Bswap_64((num11 + num12) * num) + num8) * num;
            ulong num15 = (ulong)(num6 + (long)num7) + num4;
            num2 = Bswap_64((num13 + num15) * num + num14) + num3;
            num3 = ShiftMix((num15 + num2) * num + num5 + num9) * num;
            return num3 + num13;
        }


        private unsafe static uint Fetch32(byte* p)
        {
            return *(uint*)p;
        }


        private unsafe static ulong Fetch64(byte* p)
        {
            return *(ulong*)p;
        }


        private static ulong Rotate(ulong val, int shift)
        {
            if (shift != 0)
            {
                return (val >> shift) | (val << 64 - shift);
            }
            return val;
        }


        private static ulong ShiftMix(ulong val)
        {
            return val ^ (val >> 47);
        }

        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            ulong num = (u ^ v) * mul;
            num ^= num >> 47;
            ulong num2 = (v ^ num) * mul;
            return (num2 ^ (num2 >> 47)) * mul;
        }

        private static ulong Bswap_64(ulong value)
        {
            value = ((value << 8) & 0xFF00FF00FF00FF00uL) | ((value >> 8) & 0xFF00FF00FF00FFuL);
            value = ((value << 16) & 0xFFFF0000FFFF0000uL) | ((value >> 16) & 0xFFFF0000FFFFuL);
            return (value << 32) | (value >> 32);
        }

        private static (ulong, ulong) WeakHashLen32WithSeeds(ulong w, ulong x, ulong y, ulong z, ulong a, ulong b)
        {
            a += w;
            b = Rotate(b + a + z, 21);
            ulong num = a;
            a += x;
            a += y;
            b += Rotate(a, 44);
            return (a + z, b + num);
        }


        private unsafe static (ulong, ulong) WeakHashLen32WithSeeds(byte* s, ulong a, ulong b)
        {
            return WeakHashLen32WithSeeds(Fetch64(s), Fetch64(s + 8), Fetch64(s + 16), Fetch64(s + 24), a, b);
        }


        private static ulong HashLen16(ulong u, ulong v)
        {
            return Hash128To64((u, v));
        }


        private static ulong Hash128To64((ulong, ulong) x)
        {
            ulong num = (Uint128Low64(x) ^ Uint128High64(x)) * 11376068507788127593uL;
            num ^= num >> 47;
            long num2 = (long)(Uint128High64(x) ^ num) * -7070675565921424023L;
            return ((ulong)num2 ^ ((ulong)num2 >> 47)) * 11376068507788127593uL;
        }


        private static ulong Uint128Low64((ulong, ulong) x)
        {
            return x.Item1;
        }


        private static ulong Uint128High64((ulong, ulong) x)
        {
            return x.Item2;
        }
    }
}