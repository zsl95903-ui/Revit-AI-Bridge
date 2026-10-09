using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns7;

namespace ns0;

[CompilerGenerated]
internal sealed class Class55<T, U, V, W>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly W gparam_3;

	public T p_code => gparam_0;

	public U p_order_id => gparam_1;

	public V p_user_id => gparam_2;

	public W p_device_id => gparam_3;

	[DebuggerHidden]
	public Class55(T gparam_4, U gparam_5, V gparam_6, W gparam_7)
	{
		gparam_0 = gparam_4;
		gparam_1 = gparam_5;
		gparam_2 = gparam_6;
		gparam_3 = gparam_7;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class55<T, U, V, W> @class = obj as Class55<T, U, V, W>;
		if (this != @class)
		{
			if (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2))
			{
				return EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3);
			}
			return false;
		}
		return true;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (((-1032052456 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ p_code = {0}, p_order_id = {1}, p_user_id = {2}, p_device_id = {3} }}";
		object[] array = new object[4];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		V val3 = gparam_2;
		array[2] = ((val3 != null) ? val3.ToString() : null);
		W val4 = gparam_3;
		array[3] = ((val4 != null) ? val4.ToString() : null);
		return string.Format(null, format, array);
	}
}
