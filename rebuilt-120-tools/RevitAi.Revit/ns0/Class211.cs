using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ floor_element_id = {floor_element_id}, material_element_id = {material_element_id}, location = {location}, latitude = {latitude}, longitude = {longitude}, width_feet = {width_feet}, height_feet = {height_feet}, map_source = {map_source}, zoom_level = {zoom_level} }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class211<T, U, V, W, X, Y, Z, T7, T8>
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

	public T floor_element_id => gparam_0;

	public U material_element_id => gparam_1;

	public V location => gparam_2;

	public W latitude => gparam_3;

	public X longitude => gparam_4;

	public Y width_feet => gparam_5;

	public Z height_feet => gparam_6;

	public T7 map_source => gparam_7;

	public T8 zoom_level => gparam_8;

	[DebuggerHidden]
	public Class211(T gparam_9, U gparam_10, V gparam_11, W gparam_12, X gparam_13, Y gparam_14, Z gparam_15, T7 gparam_16, T8 gparam_17)
	{
		gparam_0 = gparam_9;
		gparam_1 = gparam_10;
		gparam_2 = gparam_11;
		gparam_3 = gparam_12;
		gparam_4 = gparam_13;
		gparam_5 = gparam_14;
		gparam_6 = gparam_15;
		gparam_7 = gparam_16;
		gparam_8 = gparam_17;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class211<T, U, V, W, X, Y, Z, T7, T8> @class = obj as Class211<T, U, V, W, X, Y, Z, T7, T8>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7) && EqualityComparer<T8>.Default.Equals(gparam_8, @class.gparam_8));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((((215600772 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7)) * -1521134295 + EqualityComparer<T8>.Default.GetHashCode(gparam_8);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ floor_element_id = {0}, material_element_id = {1}, location = {2}, latitude = {3}, longitude = {4}, width_feet = {5}, height_feet = {6}, map_source = {7}, zoom_level = {8} }}";
		object[] array = new object[9];
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
		return string.Format(null, format, array);
	}
}
