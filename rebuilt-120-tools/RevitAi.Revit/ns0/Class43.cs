using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ column_id = {column_id}, column_name = {column_name} }", Type = "<Anonymous Type>")]
internal sealed class Class43<T, U>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	public T column_id => gparam_0;

	public U column_name => gparam_1;

	[DebuggerHidden]
	public Class43(T gparam_2, U gparam_3)
	{
		gparam_0 = gparam_2;
		gparam_1 = gparam_3;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class43<T, U> @class = obj as Class43<T, U>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (1024115782 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ column_id = {0}, column_name = {1} }}";
		object[] array = new object[2];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		return string.Format(null, format, array);
	}
}
