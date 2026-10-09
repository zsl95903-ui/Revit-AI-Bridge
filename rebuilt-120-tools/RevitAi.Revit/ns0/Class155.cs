using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ parameterId = {parameterId}, displayName = {displayName}, internalName = {internalName}, parameterType = {parameterType}, storageType = {storageType}, supportedRules = {supportedRules}, description = {description} }", Type = "<Anonymous Type>")]
internal sealed class Class155<T, U, V, W, X, Y, Z>
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

	public T parameterId => gparam_0;

	public U displayName => gparam_1;

	public V internalName => gparam_2;

	public W parameterType => gparam_3;

	public X storageType => gparam_4;

	public Y supportedRules => gparam_5;

	public Z description => gparam_6;

	[DebuggerHidden]
	public Class155(T gparam_7, U gparam_8, V gparam_9, W gparam_10, X gparam_11, Y gparam_12, Z gparam_13)
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
		Class155<T, U, V, W, X, Y, Z> @class = obj as Class155<T, U, V, W, X, Y, Z>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((-1646274822 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ parameterId = {0}, displayName = {1}, internalName = {2}, parameterType = {3}, storageType = {4}, supportedRules = {5}, description = {6} }}";
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
