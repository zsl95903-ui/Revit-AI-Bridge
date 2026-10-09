using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ mode = {mode}, cuttingElementCount = {cuttingElementCount}, elementToCutCount = {elementToCutCount}, checkedPairs = {checkedPairs}, correctOrderPairs = {correctOrderPairs}, notJoinedPairs = {notJoinedPairs}, wrongOrderPairs = {wrongOrderPairs}, fixedPairs = {fixedPairs}, failedPairs = {failedPairs}, results = {results} ... }", Type = "<Anonymous Type>")]
internal sealed class Class135<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12, T13, T14>
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

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Z gparam_6;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T7 gparam_7;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T8 gparam_8;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T9 gparam_9;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T10 gparam_10;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T11 gparam_11;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T12 gparam_12;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T13 gparam_13;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T14 gparam_14;

	public T mode => gparam_0;

	public U cuttingElementCount => gparam_1;

	public V elementToCutCount => gparam_2;

	public W checkedPairs => gparam_3;

	public X correctOrderPairs => gparam_4;

	public Y notJoinedPairs => gparam_5;

	public Z wrongOrderPairs => gparam_6;

	public T7 fixedPairs => gparam_7;

	public T8 failedPairs => gparam_8;

	public T9 results => gparam_9;

	public T10 has_more => gparam_10;

	public T11 returned_count => gparam_11;

	public T12 total_count => gparam_12;

	public T13 cache_id => gparam_13;

	public T14 cache_info => gparam_14;

	[DebuggerHidden]
	public Class135(T gparam_15, U gparam_16, V gparam_17, W gparam_18, X gparam_19, Y gparam_20, Z gparam_21, T7 gparam_22, T8 gparam_23, T9 gparam_24, T10 gparam_25, T11 gparam_26, T12 gparam_27, T13 gparam_28, T14 gparam_29)
	{
		gparam_0 = gparam_15;
		gparam_1 = gparam_16;
		gparam_2 = gparam_17;
		gparam_3 = gparam_18;
		gparam_4 = gparam_19;
		gparam_5 = gparam_20;
		gparam_6 = gparam_21;
		gparam_7 = gparam_22;
		gparam_8 = gparam_23;
		gparam_9 = gparam_24;
		gparam_10 = gparam_25;
		gparam_11 = gparam_26;
		gparam_12 = gparam_27;
		gparam_13 = gparam_28;
		gparam_14 = gparam_29;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class135<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12, T13, T14> @class = obj as Class135<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12, T13, T14>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7) && EqualityComparer<T8>.Default.Equals(gparam_8, @class.gparam_8) && EqualityComparer<T9>.Default.Equals(gparam_9, @class.gparam_9) && EqualityComparer<T10>.Default.Equals(gparam_10, @class.gparam_10) && EqualityComparer<T11>.Default.Equals(gparam_11, @class.gparam_11) && EqualityComparer<T12>.Default.Equals(gparam_12, @class.gparam_12) && EqualityComparer<T13>.Default.Equals(gparam_13, @class.gparam_13) && EqualityComparer<T14>.Default.Equals(gparam_14, @class.gparam_14));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((((((((((-1565541477 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7)) * -1521134295 + EqualityComparer<T8>.Default.GetHashCode(gparam_8)) * -1521134295 + EqualityComparer<T9>.Default.GetHashCode(gparam_9)) * -1521134295 + EqualityComparer<T10>.Default.GetHashCode(gparam_10)) * -1521134295 + EqualityComparer<T11>.Default.GetHashCode(gparam_11)) * -1521134295 + EqualityComparer<T12>.Default.GetHashCode(gparam_12)) * -1521134295 + EqualityComparer<T13>.Default.GetHashCode(gparam_13)) * -1521134295 + EqualityComparer<T14>.Default.GetHashCode(gparam_14);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ mode = {0}, cuttingElementCount = {1}, elementToCutCount = {2}, checkedPairs = {3}, correctOrderPairs = {4}, notJoinedPairs = {5}, wrongOrderPairs = {6}, fixedPairs = {7}, failedPairs = {8}, results = {9}, has_more = {10}, returned_count = {11}, total_count = {12}, cache_id = {13}, cache_info = {14} }}";
		object[] array = new object[15];
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
		Z val7 = gparam_6;
		array[6] = ((val7 != null) ? val7.ToString() : null);
		T7 val8 = gparam_7;
		array[7] = ((val8 != null) ? val8.ToString() : null);
		T8 val9 = gparam_8;
		array[8] = ((val9 != null) ? val9.ToString() : null);
		T9 val10 = gparam_9;
		array[9] = ((val10 != null) ? val10.ToString() : null);
		T10 val11 = gparam_10;
		array[10] = ((val11 != null) ? val11.ToString() : null);
		T11 val12 = gparam_11;
		array[11] = ((val12 != null) ? val12.ToString() : null);
		T12 val13 = gparam_12;
		array[12] = ((val13 != null) ? val13.ToString() : null);
		T13 val14 = gparam_13;
		array[13] = ((val14 != null) ? val14.ToString() : null);
		T14 val15 = gparam_14;
		array[14] = ((val15 != null) ? val15.ToString() : null);
		return string.Format(null, format, array);
	}
}
