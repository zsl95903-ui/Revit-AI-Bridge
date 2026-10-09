using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ millimeters = {millimeters}, meters = {meters} }", Type = "<Anonymous Type>")]
internal sealed class Class168<T, U>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	public T millimeters => gparam_0;

	public U meters => gparam_1;

	[DebuggerHidden]
	public Class168(T gparam_2, U gparam_3)
	{
		gparam_0 = gparam_2;
		gparam_1 = gparam_3;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class168<T, U> @class = obj as Class168<T, U>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (1949331697 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ millimeters = {0}, meters = {1} }}";
		object[] array = new object[2];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		return string.Format(null, format, array);
	}
}
