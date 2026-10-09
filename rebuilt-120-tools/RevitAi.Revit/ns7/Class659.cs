using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ns7;

internal static class Class659
{
	internal sealed class Class660
	{
		private static readonly int[] int_0;

		private static readonly int[] int_1;

		private static readonly int[] int_2;

		private static readonly int[] int_3;

		private const int int_4 = 0;

		private const int int_5 = 1;

		private const int int_6 = 2;

		private const int int_7 = 3;

		private const int int_8 = 4;

		private const int int_9 = 5;

		private const int int_10 = 6;

		private const int int_11 = 7;

		private const int int_12 = 8;

		private const int int_13 = 9;

		private const int int_14 = 10;

		private const int int_15 = 11;

		private const int int_16 = 12;

		private int int_17;

		private int int_18;

		private int int_19;

		private int int_20;

		private int int_21;

		private bool bool_0;

		private Class661 class661_0;

		private Class662 class662_0;

		private Class664 class664_0;

		private Class663 class663_0;

		private Class663 class663_1;

		public Class660(byte[] byte_0)
		{
			class661_0 = new Class661();
			class662_0 = new Class662();
			int_17 = 2;
			class661_0.method_5(byte_0, 0, byte_0.Length);
		}

		private bool method_0()
		{
			int num = class662_0.method_5();
			while (num >= 258)
			{
				switch (int_17)
				{
				case 7:
				{
					int num2;
					while (((num2 = class663_0.method_1(class661_0)) & -256) == 0)
					{
						class662_0.method_0(num2);
						if (--num < 258)
						{
							return true;
						}
					}
					if (num2 >= 257)
					{
						int_19 = int_0[num2 - 257];
						int_18 = int_1[num2 - 257];
						goto case 8;
					}
					if (num2 < 0)
					{
						return false;
					}
					class663_1 = null;
					class663_0 = null;
					int_17 = 2;
					return true;
				}
				case 8:
					if (int_18 > 0)
					{
						int_17 = 8;
						int num4 = class661_0.method_0(int_18);
						if (num4 < 0)
						{
							return false;
						}
						class661_0.method_1(int_18);
						int_19 += num4;
					}
					int_17 = 9;
					goto case 9;
				case 9:
				{
					int num2 = class663_1.method_1(class661_0);
					if (num2 >= 0)
					{
						int_20 = int_2[num2];
						int_18 = int_3[num2];
						goto case 10;
					}
					return false;
				}
				case 10:
					if (int_18 > 0)
					{
						int_17 = 10;
						int num3 = class661_0.method_0(int_18);
						if (num3 < 0)
						{
							return false;
						}
						class661_0.method_1(int_18);
						int_20 += num3;
					}
					class662_0.method_2(int_19, int_20);
					num -= int_19;
					int_17 = 7;
					break;
				}
			}
			return true;
		}

		private bool method_1()
		{
			switch (int_17)
			{
			case 2:
			{
				if (bool_0)
				{
					int_17 = 12;
					return false;
				}
				int num = class661_0.method_0(3);
				if (num < 0)
				{
					return false;
				}
				class661_0.method_1(3);
				if ((num & 1) != 0)
				{
					bool_0 = true;
				}
				switch (num >> 1)
				{
				case 0:
					class661_0.method_2();
					int_17 = 3;
					break;
				case 1:
					class663_0 = Class663.class663_0;
					class663_1 = Class663.class663_1;
					int_17 = 7;
					break;
				case 2:
					class664_0 = new Class664();
					int_17 = 6;
					break;
				}
				return true;
			}
			case 3:
				if ((int_21 = class661_0.method_0(16)) < 0)
				{
					return false;
				}
				class661_0.method_1(16);
				int_17 = 4;
				goto case 4;
			case 4:
				if (class661_0.method_0(16) < 0)
				{
					return false;
				}
				class661_0.method_1(16);
				int_17 = 5;
				goto case 5;
			case 5:
			{
				int num2 = class662_0.method_3(class661_0, int_21);
				int_21 -= num2;
				if (int_21 == 0)
				{
					int_17 = 2;
					return true;
				}
				return !class661_0.IsNeedingInput;
			}
			case 6:
				if (!class664_0.method_0(class661_0))
				{
					return false;
				}
				class663_0 = class664_0.method_1();
				class663_1 = class664_0.method_2();
				int_17 = 7;
				goto case 7;
			case 7:
			case 8:
			case 9:
			case 10:
				return method_0();
			default:
				return false;
			case 12:
				return false;
			}
		}

		public int method_2(byte[] byte_0, int int_22, int int_23)
		{
			int num = 0;
			do
			{
				if (int_17 != 11)
				{
					int num2 = class662_0.method_7(byte_0, int_22, int_23);
					int_22 += num2;
					num += num2;
					int_23 -= num2;
					if (int_23 == 0)
					{
						return num;
					}
				}
			}
			while (method_1() || (class662_0.method_6() > 0 && int_17 != 11));
			return num;
		}

		static Class660()
		{
			int[] array = new int[29] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258 };
			int_0 = array;
			int[] array2 = new int[29] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0 };
			int_1 = array2;
			int[] array3 = new int[30] { 1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577 };
			int_2 = array3;
			int[] array4 = new int[30] { 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13 };
			int_3 = array4;
		}
	}

	internal sealed class Class661
	{
		private byte[] byte_0;

		private int int_0;

		private int int_1;

		private uint uint_0;

		private int int_2;

		public int AvailableBits => int_2;

		public int AvailableBytes => int_1 - int_0 + (int_2 >> 3);

		public bool IsNeedingInput => int_0 == int_1;

		public int method_0(int int_3)
		{
			if (int_2 < int_3)
			{
				if (int_0 == int_1)
				{
					return -1;
				}
				uint_0 |= (uint)(((byte_0[int_0++] & 0xFF) | ((byte_0[int_0++] & 0xFF) << 8)) << int_2);
				int_2 += 16;
			}
			return (int)(uint_0 & ((1 << int_3) - 1));
		}

		public void method_1(int int_3)
		{
			uint_0 >>= int_3;
			int_2 -= int_3;
		}

		public void method_2()
		{
			uint_0 >>= int_2 & 7;
			int_2 &= -8;
		}

		public int method_3(byte[] byte_1, int int_3, int int_4)
		{
			int num = 0;
			while (int_2 > 0 && int_4 > 0)
			{
				byte_1[int_3++] = (byte)uint_0;
				uint_0 >>= 8;
				int_2 -= 8;
				int_4--;
				num++;
			}
			if (int_4 == 0)
			{
				return num;
			}
			int num2 = int_1 - int_0;
			if (int_4 > num2)
			{
				int_4 = num2;
			}
			Array.Copy(byte_0, int_0, byte_1, int_3, int_4);
			int_0 += int_4;
			if (((int_0 - int_1) & 1) != 0)
			{
				uint_0 = (uint)(byte_0[int_0++] & 0xFF);
				int_2 = 8;
			}
			return num + int_4;
		}

		public void method_4()
		{
			int_2 = 0;
			int_1 = 0;
			int_0 = 0;
			uint_0 = 0u;
		}

		public void method_5(byte[] byte_1, int int_3, int int_4)
		{
			if (int_0 < int_1)
			{
				throw new InvalidOperationException();
			}
			int num = int_3 + int_4;
			if (0 <= int_3 && int_3 <= num && num <= byte_1.Length)
			{
				if ((int_4 & 1) != 0)
				{
					uint_0 |= (uint)((byte_1[int_3++] & 0xFF) << int_2);
					int_2 += 8;
				}
				byte_0 = byte_1;
				int_0 = int_3;
				int_1 = num;
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	internal sealed class Class662
	{
		private const int int_0 = 32768;

		private const int int_1 = 32767;

		private byte[] byte_0 = new byte[32768];

		private int int_2;

		private int int_3;

		public void method_0(int int_4)
		{
			if (int_3++ == 32768)
			{
				throw new InvalidOperationException();
			}
			byte_0[int_2++] = (byte)int_4;
			int_2 &= 32767;
		}

		private void method_1(int int_4, int int_5)
		{
			while (int_5-- > 0)
			{
				byte_0[int_2++] = byte_0[int_4++];
				int_2 &= 32767;
				int_4 &= 0x7FFF;
			}
		}

		public void method_2(int int_4, int int_5)
		{
			if ((int_3 += int_4) > 32768)
			{
				throw new InvalidOperationException();
			}
			int num = (int_2 - int_5) & 0x7FFF;
			int num2 = 32768 - int_4;
			if (num <= num2 && int_2 < num2)
			{
				if (int_4 <= int_5)
				{
					Array.Copy(byte_0, num, byte_0, int_2, int_4);
					int_2 += int_4;
				}
				else
				{
					while (int_4-- > 0)
					{
						byte_0[int_2++] = byte_0[num++];
					}
				}
			}
			else
			{
				method_1(num, int_4);
			}
		}

		public int method_3(Class661 class661_0, int int_4)
		{
			int_4 = Math.Min(Math.Min(int_4, 32768 - int_3), class661_0.AvailableBytes);
			int num = 32768 - int_2;
			int num2;
			if (int_4 > num)
			{
				num2 = class661_0.method_3(byte_0, int_2, num);
				if (num2 == num)
				{
					num2 += class661_0.method_3(byte_0, 0, int_4 - num);
				}
			}
			else
			{
				num2 = class661_0.method_3(byte_0, int_2, int_4);
			}
			int_2 = (int_2 + num2) & 0x7FFF;
			int_3 += num2;
			return num2;
		}

		public void method_4(byte[] byte_1, int int_4, int int_5)
		{
			if (int_3 > 0)
			{
				throw new InvalidOperationException();
			}
			if (int_5 > 32768)
			{
				int_4 += int_5 - 32768;
				int_5 = 32768;
			}
			Array.Copy(byte_1, int_4, byte_0, 0, int_5);
			int_2 = int_5 & 0x7FFF;
		}

		public int method_5()
		{
			return 32768 - int_3;
		}

		public int method_6()
		{
			return int_3;
		}

		public int method_7(byte[] byte_1, int int_4, int int_5)
		{
			int num = int_2;
			if (int_5 > int_3)
			{
				int_5 = int_3;
			}
			else
			{
				num = (int_2 - int_3 + int_5) & 0x7FFF;
			}
			int num2 = int_5;
			int num3 = int_5 - num;
			if (num3 > 0)
			{
				Array.Copy(byte_0, 32768 - num3, byte_1, int_4, num3);
				int_4 += num3;
				int_5 = num;
			}
			Array.Copy(byte_0, num - int_5, byte_1, int_4, int_5);
			int_3 -= num2;
			if (int_3 < 0)
			{
				throw new InvalidOperationException();
			}
			return num2;
		}

		public void method_8()
		{
			int_2 = 0;
			int_3 = 0;
		}
	}

	internal sealed class Class663
	{
		private const int int_0 = 15;

		private short[] short_0;

		public static readonly Class663 class663_0;

		public static readonly Class663 class663_1;

		static Class663()
		{
			byte[] array = new byte[288];
			int num = 0;
			while (num < 144)
			{
				array[num++] = 8;
			}
			while (num < 256)
			{
				array[num++] = 9;
			}
			while (num < 280)
			{
				array[num++] = 7;
			}
			while (num < 288)
			{
				array[num++] = 8;
			}
			class663_0 = new Class663(array);
			array = new byte[32];
			num = 0;
			while (num < 32)
			{
				array[num++] = 5;
			}
			class663_1 = new Class663(array);
		}

		public Class663(byte[] byte_0)
		{
			method_0(byte_0);
		}

		private void method_0(byte[] byte_0)
		{
			int[] array = new int[16];
			int[] array2 = new int[16];
			foreach (int num in byte_0)
			{
				if (num > 0)
				{
					array[num]++;
				}
			}
			int num2 = 0;
			int num3 = 512;
			for (int j = 1; j <= 15; j++)
			{
				array2[j] = num2;
				num2 += array[j] << 16 - j;
				if (j >= 10)
				{
					int num4 = array2[j] & 0x1FF80;
					int num5 = num2 & 0x1FF80;
					num3 += num5 - num4 >> 16 - j;
				}
			}
			short_0 = new short[num3];
			int num6 = 512;
			for (int num7 = 15; num7 >= 10; num7--)
			{
				int num8 = num2 & 0x1FF80;
				num2 -= array[num7] << 16 - num7;
				for (int k = num2 & 0x1FF80; k < num8; k += 128)
				{
					short_0[Class666.smethod_0(k)] = (short)((-num6 << 4) | num7);
					num6 += 1 << num7 - 9;
				}
			}
			for (int l = 0; l < byte_0.Length; l++)
			{
				int num9 = byte_0[l];
				if (num9 == 0)
				{
					continue;
				}
				num2 = array2[num9];
				int num10 = Class666.smethod_0(num2);
				if (num9 <= 9)
				{
					do
					{
						short_0[num10] = (short)((l << 4) | num9);
						num10 += 1 << num9;
					}
					while (num10 < 512);
				}
				else
				{
					int num11 = short_0[num10 & 0x1FF];
					int num12 = 1 << (num11 & 0xF);
					num11 = -(num11 >> 4);
					do
					{
						short_0[num11 | (num10 >> 9)] = (short)((l << 4) | num9);
						num10 += 1 << num9;
					}
					while (num10 < num12);
				}
				array2[num9] = num2 + (1 << 16 - num9);
			}
		}

		public int method_1(Class661 class661_0)
		{
			int num;
			int num2;
			if ((num = class661_0.method_0(9)) >= 0)
			{
				if ((num2 = short_0[num]) >= 0)
				{
					class661_0.method_1(num2 & 0xF);
					return num2 >> 4;
				}
				int num3 = -(num2 >> 4);
				int int_ = num2 & 0xF;
				if ((num = class661_0.method_0(int_)) >= 0)
				{
					num2 = short_0[num3 | (num >> 9)];
					class661_0.method_1(num2 & 0xF);
					return num2 >> 4;
				}
				int availableBits = class661_0.AvailableBits;
				num = class661_0.method_0(availableBits);
				num2 = short_0[num3 | (num >> 9)];
				if ((num2 & 0xF) <= availableBits)
				{
					class661_0.method_1(num2 & 0xF);
					return num2 >> 4;
				}
				return -1;
			}
			int availableBits2 = class661_0.AvailableBits;
			num = class661_0.method_0(availableBits2);
			num2 = short_0[num];
			if (num2 >= 0 && (num2 & 0xF) <= availableBits2)
			{
				class661_0.method_1(num2 & 0xF);
				return num2 >> 4;
			}
			return -1;
		}
	}

	internal sealed class Class664
	{
		private const int int_0 = 0;

		private const int int_1 = 1;

		private const int int_2 = 2;

		private const int int_3 = 3;

		private const int int_4 = 4;

		private const int int_5 = 5;

		private static readonly int[] int_6;

		private static readonly int[] int_7;

		private byte[] byte_0;

		private byte[] byte_1;

		private Class663 class663_0;

		private int int_8;

		private int int_9;

		private int int_10;

		private int int_11;

		private int int_12;

		private int int_13;

		private byte byte_2;

		private int int_14;

		private static readonly int[] int_15;

		public bool method_0(Class661 class661_0)
		{
			while (true)
			{
				switch (int_8)
				{
				case 5:
				{
					int num = int_7[int_13];
					int num2 = class661_0.method_0(num);
					if (num2 >= 0)
					{
						class661_0.method_1(num);
						num2 += int_6[int_13];
						while (num2-- > 0)
						{
							byte_1[int_14++] = byte_2;
						}
						if (int_14 == int_12)
						{
							return true;
						}
						goto IL_00a0;
					}
					return false;
				}
				case 4:
				{
					int num3;
					while (((num3 = class663_0.method_1(class661_0)) & -16) == 0)
					{
						byte_1[int_14++] = (byte_2 = (byte)num3);
						if (int_14 == int_12)
						{
							return true;
						}
					}
					if (num3 >= 0)
					{
						if (num3 >= 17)
						{
							byte_2 = 0;
						}
						int_13 = num3 - 16;
						int_8 = 5;
						goto case 5;
					}
					return false;
				}
				case 3:
					while (int_14 < int_11)
					{
						int num4 = class661_0.method_0(3);
						if (num4 >= 0)
						{
							class661_0.method_1(3);
							byte_0[int_15[int_14]] = (byte)num4;
							int_14++;
							continue;
						}
						return false;
					}
					class663_0 = new Class663(byte_0);
					byte_0 = null;
					int_14 = 0;
					int_8 = 4;
					goto case 4;
				case 2:
					int_11 = class661_0.method_0(4);
					if (int_11 >= 0)
					{
						int_11 += 4;
						class661_0.method_1(4);
						byte_0 = new byte[19];
						int_14 = 0;
						int_8 = 3;
						goto case 3;
					}
					return false;
				case 1:
					int_10 = class661_0.method_0(5);
					if (int_10 >= 0)
					{
						int_10++;
						class661_0.method_1(5);
						int_12 = int_9 + int_10;
						byte_1 = new byte[int_12];
						int_8 = 2;
						goto case 2;
					}
					return false;
				case 0:
					int_9 = class661_0.method_0(5);
					if (int_9 >= 0)
					{
						int_9 += 257;
						class661_0.method_1(5);
						int_8 = 1;
						goto case 1;
					}
					return false;
				}
				continue;
				IL_00a0:
				int_8 = 4;
			}
		}

		public Class663 method_1()
		{
			byte[] destinationArray = new byte[int_9];
			Array.Copy(byte_1, 0, destinationArray, 0, int_9);
			return new Class663(destinationArray);
		}

		public Class663 method_2()
		{
			byte[] destinationArray = new byte[int_10];
			Array.Copy(byte_1, int_9, destinationArray, 0, int_10);
			return new Class663(destinationArray);
		}

		static Class664()
		{
			int[] array = new int[3] { 3, 3, 11 };
			int_6 = array;
			int[] array2 = new int[3] { 2, 3, 7 };
			int_7 = array2;
			int[] array3 = new int[19] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
			int_15 = array3;
		}
	}

	internal sealed class Class665
	{
		private const int int_0 = 4;

		private const int int_1 = 8;

		private const int int_2 = 16;

		private const int int_3 = 20;

		private const int int_4 = 28;

		private const int int_5 = 30;

		private int int_6 = 16;

		private long long_0;

		private Class669 class669_0;

		private Class668 class668_0;

		public long TotalOut => long_0;

		public bool IsFinished
		{
			get
			{
				if (int_6 == 30)
				{
					return class669_0.IsFlushed;
				}
				return false;
			}
		}

		public bool IsNeedingInput => class668_0.method_8();

		public Class665()
		{
			class669_0 = new Class669();
			class668_0 = new Class668(class669_0);
		}

		public void method_0()
		{
			int_6 |= 12;
		}

		public void method_1(byte[] byte_0)
		{
			class668_0.method_7(byte_0);
		}

		public int method_2(byte[] byte_0)
		{
			int num = 0;
			int num2 = byte_0.Length;
			int num3 = num2;
			while (true)
			{
				int num4 = class669_0.method_4(byte_0, num, num2);
				num += num4;
				long_0 += num4;
				num2 -= num4;
				if (num2 == 0 || int_6 == 30)
				{
					break;
				}
				if (class668_0.method_6((int_6 & 4) != 0, (int_6 & 8) != 0))
				{
					continue;
				}
				if (int_6 != 16)
				{
					if (int_6 == 20)
					{
						for (int num5 = 8 + (-class669_0.BitCount & 7); num5 > 0; num5 -= 10)
						{
							class669_0.method_3(2, 10);
						}
						int_6 = 16;
					}
					else if (int_6 == 28)
					{
						class669_0.method_2();
						int_6 = 30;
					}
					continue;
				}
				return num3 - num2;
			}
			return num3 - num2;
		}
	}

	internal sealed class Class666
	{
		public sealed class Class667
		{
			public short[] short_0;

			public byte[] byte_0;

			public int int_0;

			public int int_1;

			private short[] short_1;

			private int[] int_2;

			private int int_3;

			private Class666 class666_0;

			public Class667(Class666 class666_1, int int_4, int int_5, int int_6)
			{
				class666_0 = class666_1;
				int_0 = int_5;
				int_3 = int_6;
				short_0 = new short[int_4];
				int_2 = new int[int_6];
			}

			public void method_0(int int_4)
			{
				class666_0.class669_0.method_3(short_1[int_4] & 0xFFFF, byte_0[int_4]);
			}

			public void method_1(short[] short_2, byte[] byte_1)
			{
				short_1 = short_2;
				byte_0 = byte_1;
			}

			public void method_2()
			{
				int[] array = new int[int_3];
				int num = 0;
				short_1 = new short[short_0.Length];
				for (int i = 0; i < int_3; i++)
				{
					array[i] = num;
					num += int_2[i] << 15 - i;
				}
				for (int j = 0; j < int_1; j++)
				{
					int num2 = byte_0[j];
					if (num2 > 0)
					{
						short_1[j] = smethod_0(array[num2 - 1]);
						array[num2 - 1] += 1 << 16 - num2;
					}
				}
			}

			private void method_3(int[] int_4)
			{
				byte_0 = new byte[short_0.Length];
				int num = int_4.Length / 2;
				int num2 = (num + 1) / 2;
				int num3 = 0;
				for (int i = 0; i < int_3; i++)
				{
					int_2[i] = 0;
				}
				int[] array = new int[num];
				array[num - 1] = 0;
				for (int num4 = num - 1; num4 >= 0; num4--)
				{
					if (int_4[2 * num4 + 1] != -1)
					{
						int num5 = array[num4] + 1;
						if (num5 > int_3)
						{
							num5 = int_3;
							num3++;
						}
						array[int_4[2 * num4]] = (array[int_4[2 * num4 + 1]] = num5);
					}
					else
					{
						int num6 = array[num4];
						int_2[num6 - 1]++;
						byte_0[int_4[2 * num4]] = (byte)array[num4];
					}
				}
				if (num3 == 0)
				{
					return;
				}
				int num7 = int_3 - 1;
				while (true)
				{
					if (int_2[--num7] != 0)
					{
						do
						{
							int_2[num7]--;
							int_2[++num7]++;
							num3 -= 1 << int_3 - 1 - num7;
						}
						while (num3 > 0 && num7 < int_3 - 1);
						if (num3 <= 0)
						{
							break;
						}
					}
				}
				int_2[int_3 - 1] += num3;
				int_2[int_3 - 2] -= num3;
				int num8 = 2 * num2;
				for (int num9 = int_3; num9 != 0; num9--)
				{
					int num10 = int_2[num9 - 1];
					while (num10 > 0)
					{
						int num11 = 2 * int_4[num8++];
						if (int_4[num11 + 1] == -1)
						{
							byte_0[int_4[num11]] = (byte)num9;
							num10--;
						}
					}
				}
			}

			public void method_4()
			{
				int num = short_0.Length;
				int[] array = new int[num];
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < num; i++)
				{
					int num4 = short_0[i];
					if (num4 != 0)
					{
						int num5 = num2++;
						int num6;
						while (num5 > 0 && short_0[array[num6 = (num5 - 1) / 2]] > num4)
						{
							array[num5] = array[num6];
							num5 = num6;
						}
						array[num5] = i;
						num3 = i;
					}
				}
				while (num2 < 2)
				{
					int num7 = ((num3 < 2) ? (++num3) : 0);
					array[num2++] = num7;
				}
				int_1 = Math.Max(num3 + 1, int_0);
				int num8 = num2;
				int[] array2 = new int[4 * num2 - 2];
				int[] array3 = new int[2 * num2 - 1];
				int num9 = num8;
				for (int j = 0; j < num2; j++)
				{
					int num10 = (array2[2 * j] = array[j]);
					array2[2 * j + 1] = -1;
					array3[j] = short_0[num10] << 8;
					array[j] = j;
				}
				do
				{
					int num11 = array[0];
					int num12 = array[--num2];
					int num13 = 0;
					int num14;
					for (num14 = 1; num14 < num2; num14 = num14 * 2 + 1)
					{
						if (num14 + 1 < num2 && array3[array[num14]] > array3[array[num14 + 1]])
						{
							num14++;
						}
						array[num13] = array[num14];
						num13 = num14;
					}
					int num15 = array3[num12];
					while ((num14 = num13) > 0 && array3[array[num13 = (num14 - 1) / 2]] > num15)
					{
						array[num14] = array[num13];
					}
					array[num14] = num12;
					int num16 = array[0];
					num12 = num9++;
					array2[2 * num12] = num11;
					array2[2 * num12 + 1] = num16;
					int num17 = Math.Min(array3[num11] & 0xFF, array3[num16] & 0xFF);
					num15 = (array3[num12] = array3[num11] + array3[num16] - num17 + 1);
					num13 = 0;
					for (num14 = 1; num14 < num2; num14 = num13 * 2 + 1)
					{
						if (num14 + 1 < num2 && array3[array[num14]] > array3[array[num14 + 1]])
						{
							num14++;
						}
						array[num13] = array[num14];
						num13 = num14;
					}
					while ((num14 = num13) > 0 && array3[array[num13 = (num14 - 1) / 2]] > num15)
					{
						array[num14] = array[num13];
					}
					array[num14] = num12;
				}
				while (num2 > 1);
				method_3(array2);
			}

			public int method_5()
			{
				int num = 0;
				for (int i = 0; i < short_0.Length; i++)
				{
					num += short_0[i] * byte_0[i];
				}
				return num;
			}

			public void method_6(Class667 class667_0)
			{
				int num = -1;
				int num2 = 0;
				while (num2 < int_1)
				{
					int num3 = 1;
					int num4 = byte_0[num2];
					int num5;
					int num6;
					if (num4 == 0)
					{
						num5 = 138;
						num6 = 3;
					}
					else
					{
						num5 = 6;
						num6 = 3;
						if (num != num4)
						{
							class667_0.short_0[num4]++;
							num3 = 0;
						}
					}
					num = num4;
					num2++;
					while (num2 < int_1 && num == byte_0[num2])
					{
						num2++;
						if (++num3 >= num5)
						{
							break;
						}
					}
					if (num3 < num6)
					{
						class667_0.short_0[num] += (short)num3;
					}
					else if (num != 0)
					{
						class667_0.short_0[16]++;
					}
					else if (num3 <= 10)
					{
						class667_0.short_0[17]++;
					}
					else
					{
						class667_0.short_0[18]++;
					}
				}
			}

			public void method_7(Class667 class667_0)
			{
				int num = -1;
				int num2 = 0;
				while (num2 < int_1)
				{
					int num3 = 1;
					int num4 = byte_0[num2];
					int num5;
					int num6;
					if (num4 == 0)
					{
						num5 = 138;
						num6 = 3;
					}
					else
					{
						num5 = 6;
						num6 = 3;
						if (num != num4)
						{
							class667_0.method_0(num4);
							num3 = 0;
						}
					}
					num = num4;
					num2++;
					while (num2 < int_1 && num == byte_0[num2])
					{
						num2++;
						if (++num3 >= num5)
						{
							break;
						}
					}
					if (num3 < num6)
					{
						while (num3-- > 0)
						{
							class667_0.method_0(num);
						}
					}
					else if (num != 0)
					{
						class667_0.method_0(16);
						class666_0.class669_0.method_3(num3 - 3, 2);
					}
					else if (num3 <= 10)
					{
						class667_0.method_0(17);
						class666_0.class669_0.method_3(num3 - 3, 3);
					}
					else
					{
						class667_0.method_0(18);
						class666_0.class669_0.method_3(num3 - 11, 7);
					}
				}
			}
		}

		private const int int_0 = 16384;

		private const int int_1 = 286;

		private const int int_2 = 30;

		private const int int_3 = 19;

		private const int int_4 = 16;

		private const int int_5 = 17;

		private const int int_6 = 18;

		private const int int_7 = 256;

		private static readonly int[] int_8;

		private static readonly byte[] byte_0;

		private Class669 class669_0;

		private Class667 class667_0;

		private Class667 class667_1;

		private Class667 class667_2;

		private short[] short_0;

		private byte[] byte_1;

		private int int_9;

		private int int_10;

		private static readonly short[] short_1;

		private static readonly byte[] byte_2;

		private static readonly short[] short_2;

		private static readonly byte[] byte_3;

		public static short smethod_0(int int_11)
		{
			return (short)((byte_0[int_11 & 0xF] << 12) | (byte_0[(int_11 >> 4) & 0xF] << 8) | (byte_0[(int_11 >> 8) & 0xF] << 4) | byte_0[int_11 >> 12]);
		}

		static Class666()
		{
			int[] array = new int[19] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
			int_8 = array;
			byte[] array2 = new byte[16] { 0x00, 0x08, 0x04, 0x0c, 0x02, 0x0a, 0x06, 0x0e, 0x01, 0x09, 0x05, 0x0d, 0x03, 0x0b, 0x07, 0x0f };
			byte_0 = array2;
			short_1 = new short[286];
			byte_2 = new byte[286];
			int num = 0;
			while (num < 144)
			{
				short_1[num] = smethod_0(48 + num << 8);
				byte_2[num++] = 8;
			}
			while (num < 256)
			{
				short_1[num] = smethod_0(256 + num << 7);
				byte_2[num++] = 9;
			}
			while (num < 280)
			{
				short_1[num] = smethod_0(-256 + num << 9);
				byte_2[num++] = 7;
			}
			while (num < 286)
			{
				short_1[num] = smethod_0(-88 + num << 8);
				byte_2[num++] = 8;
			}
			short_2 = new short[30];
			byte_3 = new byte[30];
			for (num = 0; num < 30; num++)
			{
				short_2[num] = smethod_0(num << 11);
				byte_3[num] = 5;
			}
		}

		public Class666(Class669 class669_1)
		{
			class669_0 = class669_1;
			class667_0 = new Class667(this, 286, 257, 15);
			class667_1 = new Class667(this, 30, 1, 15);
			class667_2 = new Class667(this, 19, 4, 7);
			short_0 = new short[16384];
			byte_1 = new byte[16384];
		}

		public void method_0()
		{
			int_9 = 0;
			int_10 = 0;
		}

		private int method_1(int int_11)
		{
			if (int_11 == 255)
			{
				return 285;
			}
			int num = 257;
			while (int_11 >= 8)
			{
				num += 4;
				int_11 >>= 1;
			}
			return num + int_11;
		}

		private int method_2(int int_11)
		{
			int num = 0;
			while (int_11 >= 4)
			{
				num += 2;
				int_11 >>= 1;
			}
			return num + int_11;
		}

		public void method_3(int int_11)
		{
			class667_2.method_2();
			class667_0.method_2();
			class667_1.method_2();
			class669_0.method_3(class667_0.int_1 - 257, 5);
			class669_0.method_3(class667_1.int_1 - 1, 5);
			class669_0.method_3(int_11 - 4, 4);
			for (int i = 0; i < int_11; i++)
			{
				class669_0.method_3(class667_2.byte_0[int_8[i]], 3);
			}
			class667_0.method_7(class667_2);
			class667_1.method_7(class667_2);
		}

		public void method_4()
		{
			for (int i = 0; i < int_9; i++)
			{
				int num = byte_1[i] & 0xFF;
				int num2 = short_0[i];
				if (num2-- != 0)
				{
					int num3 = method_1(num);
					class667_0.method_0(num3);
					int num4 = (num3 - 261) / 4;
					if (num4 > 0 && num4 <= 5)
					{
						class669_0.method_3(num & ((1 << num4) - 1), num4);
					}
					int num5 = method_2(num2);
					class667_1.method_0(num5);
					num4 = num5 / 2 - 1;
					if (num4 > 0)
					{
						class669_0.method_3(num2 & ((1 << num4) - 1), num4);
					}
				}
				else
				{
					class667_0.method_0(num);
				}
			}
			class667_0.method_0(256);
		}

		public void method_5(byte[] byte_4, int int_11, int int_12, bool bool_0)
		{
			class669_0.method_3(bool_0 ? 1 : 0, 3);
			class669_0.method_2();
			class669_0.method_0(int_12);
			class669_0.method_0(~int_12);
			class669_0.method_1(byte_4, int_11, int_12);
			method_0();
		}

		public void method_6(byte[] byte_4, int int_11, int int_12, bool bool_0)
		{
			class667_0.short_0[256]++;
			class667_0.method_4();
			class667_1.method_4();
			class667_0.method_6(class667_2);
			class667_1.method_6(class667_2);
			class667_2.method_4();
			int num = 4;
			for (int num2 = 18; num2 > num; num2--)
			{
				if (class667_2.byte_0[int_8[num2]] > 0)
				{
					num = num2 + 1;
				}
			}
			int num3 = 14 + num * 3 + class667_2.method_5() + class667_0.method_5() + class667_1.method_5() + int_10;
			int num4 = int_10;
			for (int i = 0; i < 286; i++)
			{
				num4 += class667_0.short_0[i] * byte_2[i];
			}
			for (int j = 0; j < 30; j++)
			{
				num4 += class667_1.short_0[j] * byte_3[j];
			}
			if (num3 >= num4)
			{
				num3 = num4;
			}
			if (int_11 >= 0 && int_12 + 4 < num3 >> 3)
			{
				method_5(byte_4, int_11, int_12, bool_0);
			}
			else if (num3 == num4)
			{
				class669_0.method_3(2 + (bool_0 ? 1 : 0), 3);
				class667_0.method_1(short_1, byte_2);
				class667_1.method_1(short_2, byte_3);
				method_4();
				method_0();
			}
			else
			{
				class669_0.method_3(4 + (bool_0 ? 1 : 0), 3);
				method_3(num);
				method_4();
				method_0();
			}
		}

		public bool method_7()
		{
			return int_9 >= 16384;
		}

		public bool method_8(int int_11)
		{
			short_0[int_9] = 0;
			byte_1[int_9++] = (byte)int_11;
			class667_0.short_0[int_11]++;
			return method_7();
		}

		public bool method_9(int int_11, int int_12)
		{
			short_0[int_9] = (short)int_11;
			byte_1[int_9++] = (byte)(int_12 - 3);
			int num = method_1(int_12 - 3);
			class667_0.short_0[num]++;
			if (num >= 265 && num < 285)
			{
				int_10 += (num - 261) / 4;
			}
			int num2 = method_2(int_11 - 1);
			class667_1.short_0[num2]++;
			if (num2 >= 4)
			{
				int_10 += num2 / 2 - 1;
			}
			return method_7();
		}
	}

	internal sealed class Class668
	{
		private const int int_0 = 258;

		private const int int_1 = 3;

		private const int int_2 = 32768;

		private const int int_3 = 32767;

		private const int int_4 = 32768;

		private const int int_5 = 32767;

		private const int int_6 = 5;

		private const int int_7 = 262;

		private const int int_8 = 32506;

		private const int int_9 = 4096;

		private int int_10;

		private short[] short_0;

		private short[] short_1;

		private int int_11;

		private int int_12;

		private bool bool_0;

		private int int_13;

		private int int_14;

		private int int_15;

		private byte[] byte_0;

		private byte[] byte_1;

		private int int_16;

		private int int_17;

		private int int_18;

		private Class669 class669_0;

		private Class666 class666_0;

		public Class668(Class669 class669_1)
		{
			class669_0 = class669_1;
			class666_0 = new Class666(class669_1);
			byte_0 = new byte[65536];
			short_0 = new short[32768];
			short_1 = new short[32768];
			int_14 = 1;
			int_13 = 1;
		}

		private void method_0()
		{
			int_10 = (byte_0[int_14] << 5) ^ byte_0[int_14 + 1];
		}

		private int method_1()
		{
			int num = ((int_10 << 5) ^ byte_0[int_14 + 2]) & 0x7FFF;
			short num2 = (short_1[int_14 & 0x7FFF] = short_0[num]);
			short_0[num] = (short)int_14;
			int_10 = num;
			return num2 & 0xFFFF;
		}

		private void method_2()
		{
			Array.Copy(byte_0, 32768, byte_0, 0, 32768);
			int_11 -= 32768;
			int_14 -= 32768;
			int_13 -= 32768;
			for (int i = 0; i < 32768; i++)
			{
				int num = short_0[i] & 0xFFFF;
				short_0[i] = (short)((num >= 32768) ? (num - 32768) : 0);
			}
			for (int j = 0; j < 32768; j++)
			{
				int num2 = short_1[j] & 0xFFFF;
				short_1[j] = (short)((num2 >= 32768) ? (num2 - 32768) : 0);
			}
		}

		public void method_3()
		{
			if (int_14 >= 65274)
			{
				method_2();
			}
			while (int_15 < 262 && int_17 < int_18)
			{
				int num = 65536 - int_15 - int_14;
				if (num > int_18 - int_17)
				{
					num = int_18 - int_17;
				}
				Array.Copy(byte_1, int_17, byte_0, int_14 + int_15, num);
				int_17 += num;
				int_16 += num;
				int_15 += num;
			}
			if (int_15 >= 3)
			{
				method_0();
			}
		}

		private bool method_4(int int_19)
		{
			int num = 128;
			int num2 = 128;
			short[] array = short_1;
			int num3 = int_14;
			int num4 = int_14 + int_12;
			int num5 = Math.Max(int_12, 2);
			int num6 = Math.Max(int_14 - 32506, 0);
			int num7 = int_14 + 258 - 1;
			byte b = byte_0[num4 - 1];
			byte b2 = byte_0[num4];
			if (num5 >= 8)
			{
				num >>= 2;
			}
			if (num2 > int_15)
			{
				num2 = int_15;
			}
			do
			{
				if (byte_0[int_19 + num5] != b2 || byte_0[int_19 + num5 - 1] != b || byte_0[int_19] != byte_0[num3] || byte_0[int_19 + 1] != byte_0[num3 + 1])
				{
					continue;
				}
				int num8 = int_19 + 2;
				num3 += 2;
				while (byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && byte_0[++num3] == byte_0[++num8] && num3 < num7)
				{
				}
				if (num3 > num4)
				{
					int_11 = int_19;
					num4 = num3;
					num5 = num3 - int_14;
					if (num5 >= num2)
					{
						break;
					}
					b = byte_0[num4 - 1];
					b2 = byte_0[num4];
				}
				num3 = int_14;
			}
			while ((int_19 = array[int_19 & 0x7FFF] & 0xFFFF) > num6 && --num != 0);
			int_12 = Math.Min(num5, int_15);
			return int_12 >= 3;
		}

		private bool method_5(bool bool_1, bool bool_2)
		{
			if (int_15 < 262 && !bool_1)
			{
				return false;
			}
			do
			{
				if ((int_15 >= 262) | bool_1)
				{
					if (int_15 != 0)
					{
						if (int_14 >= 65274)
						{
							method_2();
						}
						int num = int_11;
						int num2 = int_12;
						if (int_15 >= 3)
						{
							int num3 = method_1();
							if (num3 != 0 && int_14 - num3 <= 32506 && method_4(num3) && int_12 <= 5 && int_12 == 3 && int_14 - int_11 > 4096)
							{
								int_12 = 2;
							}
						}
						if (num2 >= 3 && int_12 <= num2)
						{
							class666_0.method_9(int_14 - 1 - num, num2);
							num2 -= 2;
							do
							{
								int_14++;
								int_15--;
								if (int_15 >= 3)
								{
									method_1();
								}
							}
							while (--num2 > 0);
							int_14++;
							int_15--;
							bool_0 = false;
							int_12 = 2;
						}
						else
						{
							if (bool_0)
							{
								class666_0.method_8(byte_0[int_14 - 1] & 0xFF);
							}
							bool_0 = true;
							int_14++;
							int_15--;
						}
						continue;
					}
					if (bool_0)
					{
						class666_0.method_8(byte_0[int_14 - 1] & 0xFF);
					}
					bool_0 = false;
					class666_0.method_6(byte_0, int_13, int_14 - int_13, bool_2);
					int_13 = int_14;
					return false;
				}
				return true;
			}
			while (!class666_0.method_7());
			int num4 = int_14 - int_13;
			if (bool_0)
			{
				num4--;
			}
			bool flag = bool_2 && int_15 == 0 && !bool_0;
			class666_0.method_6(byte_0, int_13, num4, flag);
			int_13 += num4;
			return !flag;
		}

		public bool method_6(bool bool_1, bool bool_2)
		{
			bool flag;
			do
			{
				method_3();
				bool bool_3 = bool_1 && int_17 == int_18;
				flag = method_5(bool_3, bool_2);
			}
			while (class669_0.IsFlushed & flag);
			return flag;
		}

		public void method_7(byte[] byte_2)
		{
			byte_1 = byte_2;
			int_17 = 0;
			int_18 = byte_2.Length;
		}

		public bool method_8()
		{
			return int_18 == int_17;
		}
	}

	internal sealed class Class669
	{
		protected byte[] byte_0 = new byte[65536];

		private int int_0;

		private int int_1;

		private uint uint_0;

		private int int_2;

		public int BitCount => int_2;

		public bool IsFlushed => int_1 == 0;

		public void method_0(int int_3)
		{
			byte_0[int_1++] = (byte)int_3;
			byte_0[int_1++] = (byte)(int_3 >> 8);
		}

		public void method_1(byte[] byte_1, int int_3, int int_4)
		{
			Array.Copy(byte_1, int_3, byte_0, int_1, int_4);
			int_1 += int_4;
		}

		public void method_2()
		{
			if (int_2 > 0)
			{
				byte_0[int_1++] = (byte)uint_0;
				if (int_2 > 8)
				{
					byte_0[int_1++] = (byte)(uint_0 >> 8);
				}
			}
			uint_0 = 0u;
			int_2 = 0;
		}

		public void method_3(int int_3, int int_4)
		{
			uint_0 |= (uint)(int_3 << int_2);
			int_2 += int_4;
			if (int_2 >= 16)
			{
				byte_0[int_1++] = (byte)uint_0;
				byte_0[int_1++] = (byte)(uint_0 >> 8);
				uint_0 >>= 16;
				int_2 -= 16;
			}
		}

		public int method_4(byte[] byte_1, int int_3, int int_4)
		{
			if (int_2 >= 8)
			{
				byte_0[int_1++] = (byte)uint_0;
				uint_0 >>= 8;
				int_2 -= 8;
			}
			if (int_4 > int_1 - int_0)
			{
				int_4 = int_1 - int_0;
				Array.Copy(byte_0, int_0, byte_1, int_3, int_4);
				int_0 = 0;
				int_1 = 0;
			}
			else
			{
				Array.Copy(byte_0, int_0, byte_1, int_3, int_4);
				int_0 += int_4;
			}
			return int_4;
		}
	}

	internal sealed class Stream0 : MemoryStream
	{
		public void method_0(int int_0)
		{
			WriteByte((byte)(int_0 & 0xFF));
			WriteByte((byte)((int_0 >> 8) & 0xFF));
		}

		public void method_1(int int_0)
		{
			method_0(int_0);
			method_0(int_0 >> 16);
		}

		public int method_2()
		{
			return ReadByte() | (ReadByte() << 8);
		}

		public int method_3()
		{
			return method_2() | (method_2() << 16);
		}

		public Stream0()
		{
		}

		public Stream0(byte[] byte_0)
			: base(byte_0, writable: false)
		{
		}
	}

	public static string string_0;

	private static ICryptoTransform smethod_0(byte[] byte_0, byte[] byte_1, bool bool_0)
	{
		using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		return bool_0 ? aesCryptoServiceProvider.CreateDecryptor(byte_0, byte_1) : aesCryptoServiceProvider.CreateEncryptor(byte_0, byte_1);
	}

	public static Class658 smethod_1(byte[] byte_0)
	{
		if (byte_0 != null && byte_0.Length >= 4)
		{
			int num;
			using (Stream0 stream = new Stream0(byte_0))
			{
				num = stream.method_3();
			}
			if (num == 67324752)
			{
				return Class658.class658_0;
			}
			int num2 = num >> 24;
			num -= num2 << 24;
			if (num == 8223355)
			{
				return (Class658)num2;
			}
			return (Class658)(-2);
		}
		return (Class658)(-1);
	}

	public static byte[] smethod_2(byte[] byte_0)
	{
		Stream0 stream = new Stream0(byte_0);
		byte[] array = new byte[0];
		int num = stream.method_3();
		int num2 = num >> 24;
		if (num - (num2 << 24) == 8223355)
		{
			switch ((Class658)num2)
			{
			case Class658.class658_1:
			{
				int num3 = stream.method_3();
				array = new byte[num3];
				int num5;
				for (int i = 0; i < num3; i += num5)
				{
					int num4 = stream.method_3();
					num5 = stream.method_3();
					byte[] array4 = new byte[num4];
					stream.Read(array4, 0, array4.Length);
					new Class660(array4).method_2(array, i, num5);
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("version", num2, "Selected compression algorithm is not supported.");
			case Class658.class658_3:
			{
				byte[] array2 = new byte[16] { 0xd3, 0x2b, 0x45, 0x95, 0xa5, 0x76, 0xc6, 0xfb, 0x2b, 0x93, 0x1c, 0x50, 0xad, 0x9a, 0x77, 0x87 };
				byte[] byte_1 = array2;
				byte[] array3 = new byte[16] { 0xce, 0x5b, 0x5b, 0x3a, 0x9c, 0xaf, 0xb1, 0xbb, 0xbb, 0xfd, 0xfd, 0x25, 0x46, 0xa7, 0x69, 0x0c };
				byte[] byte_2 = array3;
				using (ICryptoTransform cryptoTransform = smethod_0(byte_1, byte_2, bool_0: true))
				{
					array = smethod_2(cryptoTransform.TransformFinalBlock(byte_0, 4, byte_0.Length - 4));
				}
				break;
			}
			}
			stream.Close();
			stream = null;
			return array;
		}
		throw new FormatException("Unknown Header");
	}

	public static byte[] smethod_3(byte[] byte_0)
	{
		return smethod_5(byte_0, Class658.class658_1, null, null);
	}

	public static byte[] smethod_4(byte[] byte_0, byte[] byte_1, byte[] byte_2)
	{
		return smethod_5(byte_0, Class658.class658_3, byte_1, byte_2);
	}

	private static byte[] smethod_5(byte[] byte_0, Class658 class658_0, byte[] byte_1, byte[] byte_2)
	{
		try
		{
			Stream0 stream = new Stream0();
			switch (class658_0)
			{
			case Class658.class658_1:
			{
				stream.method_1(25000571);
				stream.method_1(byte_0.Length);
				byte[] array3;
				for (int i = 0; i < byte_0.Length; i += array3.Length)
				{
					array3 = new byte[Math.Min(2097151, byte_0.Length - i)];
					Buffer.BlockCopy(byte_0, i, array3, 0, array3.Length);
					long position = stream.Position;
					stream.method_1(0);
					stream.method_1(array3.Length);
					Class665 @class = new Class665();
					@class.method_1(array3);
					while (!@class.IsNeedingInput)
					{
						byte[] array4 = new byte[512];
						int num = @class.method_2(array4);
						if (num <= 0)
						{
							break;
						}
						stream.Write(array4, 0, num);
					}
					@class.method_0();
					while (!@class.IsFinished)
					{
						byte[] array5 = new byte[512];
						int num2 = @class.method_2(array5);
						if (num2 <= 0)
						{
							break;
						}
						stream.Write(array5, 0, num2);
					}
					long position2 = stream.Position;
					stream.Position = position;
					stream.method_1((int)@class.TotalOut);
					stream.Position = position2;
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("algorithm", class658_0, "Selected compression algorithm is not supported.");
			case Class658.class658_3:
			{
				stream.method_1(58555003);
				byte[] array = smethod_5(byte_0, Class658.class658_1, null, null);
				using (ICryptoTransform cryptoTransform = smethod_0(byte_1, byte_2, bool_0: false))
				{
					byte[] array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
					stream.Write(array2, 0, array2.Length);
				}
				break;
			}
			}
			stream.Flush();
			stream.Close();
			return stream.ToArray();
		}
		catch (Exception ex)
		{
			string_0 = "ERR 2003: " + ex.Message;
			throw;
		}
	}
}
