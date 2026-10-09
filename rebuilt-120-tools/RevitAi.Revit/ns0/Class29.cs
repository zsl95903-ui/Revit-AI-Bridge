using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ items = {items}, count = {count}, total_count = {total_count}, collision_count = {collision_count}, collision_rate = {collision_rate}, total_checked = {total_checked}, has_more = {has_more}, caches = {caches}, cache_info = {cache_info}, source_link_instance_id = {source_link_instance_id} ... }", Type = "<Anonymous Type>")]
internal sealed class Class29<T, U, V, W, X, Y, Z, T7, T8, T9, T10>
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

	public T items => gparam_0;

	public U count => gparam_1;

	public V total_count => gparam_2;

	public W collision_count => gparam_3;

	public X collision_rate => gparam_4;

	public Y total_checked => gparam_5;

	public Z has_more => gparam_6;

	public T7 caches => gparam_7;

	public T8 cache_info => gparam_8;

	public T9 source_link_instance_id => gparam_9;

	public T10 target_link_instance_id => gparam_10;

	[DebuggerHidden]
	public Class29(T gparam_11, U gparam_12, V gparam_13, W gparam_14, X gparam_15, Y gparam_16, Z gparam_17, T7 gparam_18, T8 gparam_19, T9 gparam_20, T10 gparam_21)
	{
		gparam_0 = gparam_11;
		gparam_1 = gparam_12;
		gparam_2 = gparam_13;
		gparam_3 = gparam_14;
		gparam_4 = gparam_15;
		gparam_5 = gparam_16;
		gparam_6 = gparam_17;
		gparam_7 = gparam_18;
		gparam_8 = gparam_19;
		gparam_9 = gparam_20;
		gparam_10 = gparam_21;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class29<T, U, V, W, X, Y, Z, T7, T8, T9, T10> @class = obj as Class29<T, U, V, W, X, Y, Z, T7, T8, T9, T10>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7) && EqualityComparer<T8>.Default.Equals(gparam_8, @class.gparam_8) && EqualityComparer<T9>.Default.Equals(gparam_9, @class.gparam_9) && EqualityComparer<T10>.Default.Equals(gparam_10, @class.gparam_10));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((((((907842520 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7)) * -1521134295 + EqualityComparer<T8>.Default.GetHashCode(gparam_8)) * -1521134295 + EqualityComparer<T9>.Default.GetHashCode(gparam_9)) * -1521134295 + EqualityComparer<T10>.Default.GetHashCode(gparam_10);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ items = {0}, count = {1}, total_count = {2}, collision_count = {3}, collision_rate = {4}, total_checked = {5}, has_more = {6}, caches = {7}, cache_info = {8}, source_link_instance_id = {9}, target_link_instance_id = {10} }}";
		object[] array = new object[11];
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
		return string.Format(null, format, array);
	}
}
