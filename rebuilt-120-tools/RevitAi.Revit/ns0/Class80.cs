using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ operator = {operator}, spotDimensionId = {spotDimensionId}, targetElementId = {targetElementId}, targetCategory = {targetCategory}, targetName = {targetName}, spotDimensionTypeId = {spotDimensionTypeId}, hasLeader = {hasLeader}, viewId = {viewId}, viewName = {viewName}, location = {location} ... }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class80<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11>
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

	public T @operator => gparam_0;

	public U spotDimensionId => gparam_1;

	public V targetElementId => gparam_2;

	public W targetCategory => gparam_3;

	public X targetName => gparam_4;

	public Y spotDimensionTypeId => gparam_5;

	public Z hasLeader => gparam_6;

	public T7 viewId => gparam_7;

	public T8 viewName => gparam_8;

	public T9 location => gparam_9;

	public T10 location_unit => gparam_10;

	public T11 spotDimValue => gparam_11;

	[DebuggerHidden]
	public Class80(T gparam_12, U gparam_13, V gparam_14, W gparam_15, X gparam_16, Y gparam_17, Z gparam_18, T7 gparam_19, T8 gparam_20, T9 gparam_21, T10 gparam_22, T11 gparam_23)
	{
		gparam_0 = gparam_12;
		gparam_1 = gparam_13;
		gparam_2 = gparam_14;
		gparam_3 = gparam_15;
		gparam_4 = gparam_16;
		gparam_5 = gparam_17;
		gparam_6 = gparam_18;
		gparam_7 = gparam_19;
		gparam_8 = gparam_20;
		gparam_9 = gparam_21;
		gparam_10 = gparam_22;
		gparam_11 = gparam_23;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class80<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11> @class = obj as Class80<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7) && EqualityComparer<T8>.Default.Equals(gparam_8, @class.gparam_8) && EqualityComparer<T9>.Default.Equals(gparam_9, @class.gparam_9) && EqualityComparer<T10>.Default.Equals(gparam_10, @class.gparam_10) && EqualityComparer<T11>.Default.Equals(gparam_11, @class.gparam_11));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (((((((((((-1761043104 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7)) * -1521134295 + EqualityComparer<T8>.Default.GetHashCode(gparam_8)) * -1521134295 + EqualityComparer<T9>.Default.GetHashCode(gparam_9)) * -1521134295 + EqualityComparer<T10>.Default.GetHashCode(gparam_10)) * -1521134295 + EqualityComparer<T11>.Default.GetHashCode(gparam_11);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ operator = {0}, spotDimensionId = {1}, targetElementId = {2}, targetCategory = {3}, targetName = {4}, spotDimensionTypeId = {5}, hasLeader = {6}, viewId = {7}, viewName = {8}, location = {9}, location_unit = {10}, spotDimValue = {11} }}";
		object[] array = new object[12];
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
		return string.Format(null, format, array);
	}
}
