using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ns0;

[CompilerGenerated]
internal sealed class Class653
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
	internal struct Struct6
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
	internal struct Struct7
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 128)]
	internal struct Struct8
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 160)]
	internal struct Struct9
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 264)]
	internal struct Struct10
	{
	}

	internal static readonly Struct10 struct10_0/* Not supported: data(14 5C E1 FF FF FF FF FF 40 5C E1 FF FF FF FF FF BE 5B E1 FF FF FF FF FF BC 5B E1 FF FF FF FF FF 09 5C E1 FF FF FF FF FF 30 5C E1 FF FF FF FF FF 0F 5C E1 FF FF FF FF FF 36 5C E1 FF FF FF FF FF 75 7B E1 FF FF FF FF FF 69 7B E1 FF FF FF FF FF 72 7B E1 FF FF FF FF FF 4E 76 E1 FF FF FF FF FF 58 76 E1 FF FF FF FF FF 60 7B E1 FF FF FF FF FF 5D 7B E1 FF FF FF FF FF 5A 7B E1 FF FF FF FF FF 08 7B E1 FF FF FF FF FF D1 7A E1 FF FF FF FF FF 30 76 E1 FF FF FF FF FF 0C 77 E1 FF FF FF FF FF 70 77 E1 FF FF FF FF FF 20 77 E1 FF FF FF FF FF EF 5B E1 FF FF FF FF FF ED 5B E1 FF FF FF FF FF EB 5B E1 FF FF FF FF FF F3 5B E1 FF FF FF FF FF F1 5B E1 FF FF FF FF FF F5 5B E1 FF FF FF FF FF DD 5B E1 FF FF FF FF FF 33 5C E1 FF FF FF FF FF F8 76 E1 FF FF FF FF FF 2C 5C E1 FF FF FF FF FF 0E 5C E1 FF FF FF FF FF) */;

	internal static readonly Struct6 struct6_0/* Not supported: data(2C 00 0C FF 20 00) */;

	internal static readonly Struct8 struct8_0/* Not supported: data(00 00 00 00 00 80 56 40 00 00 00 00 00 80 56 40 00 00 00 00 00 E0 70 40 00 00 00 00 00 E0 70 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 66 40 00 00 00 00 00 80 66 40 00 00 00 00 00 80 56 40 00 00 00 00 00 80 56 40 00 00 00 00 00 E0 70 40 00 00 00 00 00 E0 70 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 66 40 00 00 00 00 00 80 66 40) */;

	internal static readonly Struct8 struct8_1/* Not supported: data(00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 BF 00 00 00 00 00 00 F0 3F) */;

	internal static readonly Struct7 struct7_0/* Not supported: data(01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00) */;

	internal static readonly Struct7 struct7_1/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 F0 3F 00 00 00 00 00 00 00 40 00 00 00 00 00 00 08 40) */;

	internal static readonly Struct9 struct9_0/* Not supported: data(00 00 00 00 00 00 2E 40 00 00 00 00 00 00 34 40 00 00 00 00 00 00 39 40 00 00 00 00 00 00 40 40 00 00 00 00 00 00 44 40 00 00 00 00 00 00 49 40 00 00 00 00 00 40 50 40 00 00 00 00 00 C0 52 40 00 00 00 00 00 00 54 40 00 00 00 00 00 00 59 40 00 00 00 00 00 40 5F 40 00 00 00 00 00 C0 62 40 00 00 00 00 00 00 69 40 00 00 00 00 00 40 6F 40 00 00 00 00 00 C0 72 40 00 00 00 00 00 E0 75 40 00 00 00 00 00 00 79 40 00 00 00 00 00 20 7C 40 00 00 00 00 00 40 7F 40 00 00 00 00 00 C0 82 40) */;

	internal static uint smethod_0(string string_0)
	{
		uint num = default(uint);
		if (string_0 != null)
		{
			num = 2166136261u;
			for (int i = 0; i < string_0.Length; i++)
			{
				num = (string_0[i] ^ num) * 16777619;
			}
		}
		return num;
	}

	internal static ReadOnlySpan<U> smethod_1<T, U>(in T gparam_0, int int_0)
	{
		return MemoryMarshal.CreateReadOnlySpan(in Unsafe.As<T, U>(ref Unsafe.AsRef(in gparam_0)), int_0);
	}

	internal static ref U smethod_2<T, U>(ref T gparam_0, int int_0)
	{
		return ref Unsafe.Add(ref Unsafe.As<T, U>(ref gparam_0), int_0);
	}
}
