using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ tagId = {tagId}, tagName = {tagName}, tagType = {tagType}, taggedElementId = {taggedElementId}, taggedElementType = {taggedElementType}, taggedElementName = {taggedElementName} }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class150<T, U, V, W, X, Y>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly W gparam_3;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly X gparam_4;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Y gparam_5;

	public T tagId => gparam_0;

	public U tagName => gparam_1;

	public V tagType => gparam_2;

	public W taggedElementId => gparam_3;

	public X taggedElementType => gparam_4;

	public Y taggedElementName => gparam_5;

	[DebuggerHidden]
	public Class150(T gparam_6, U gparam_7, V gparam_8, W gparam_9, X gparam_10, Y gparam_11)
	{
		gparam_0 = gparam_6;
		gparam_1 = gparam_7;
		gparam_2 = gparam_8;
		gparam_3 = gparam_9;
		gparam_4 = gparam_10;
		gparam_5 = gparam_11;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class150<T, U, V, W, X, Y> @class = obj as Class150<T, U, V, W, X, Y>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (((((287856904 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ tagId = {0}, tagName = {1}, tagType = {2}, taggedElementId = {3}, taggedElementType = {4}, taggedElementName = {5} }}";
		object[] array = new object[6];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		V val3 = gparam_2;
		array[2] = ((val3 != null) ? val3.ToString() : null);
		W val4 = gparam_3;
		array[3] = ((val4 != null) ? val4.ToString() : null);
		X val5 = gparam_4;
		array[4] = ((val5 != null) ? val5.ToString() : null);
		Y val6 = gparam_5;
		array[5] = ((val6 != null) ? val6.ToString() : null);
		return string.Format(null, format, array);
	}
}
