using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ mode = {mode}, roomId = {roomId}, roomName = {roomName}, levelId = {levelId}, levelName = {levelName}, location = {location}, location_unit = {location_unit} }", Type = "<Anonymous Type>")]
internal sealed class Class72<T, U, V, W, X, Y, Z>
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

	public T mode => gparam_0;

	public U roomId => gparam_1;

	public V roomName => gparam_2;

	public W levelId => gparam_3;

	public X levelName => gparam_4;

	public Y location => gparam_5;

	public Z location_unit => gparam_6;

	[DebuggerHidden]
	public Class72(T gparam_7, U gparam_8, V gparam_9, W gparam_10, X gparam_11, Y gparam_12, Z gparam_13)
	{
		gparam_0 = gparam_7;
		gparam_1 = gparam_8;
		gparam_2 = gparam_9;
		gparam_3 = gparam_10;
		gparam_4 = gparam_11;
		gparam_5 = gparam_12;
		gparam_6 = gparam_13;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class72<T, U, V, W, X, Y, Z> @class = obj as Class72<T, U, V, W, X, Y, Z>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((1620489256 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ mode = {0}, roomId = {1}, roomName = {2}, levelId = {3}, levelName = {4}, location = {5}, location_unit = {6} }}";
		object[] array = new object[7];
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
		return string.Format(null, format, array);
	}
}
