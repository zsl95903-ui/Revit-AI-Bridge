using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns7;

namespace ns0;

[CompilerGenerated]
internal sealed class Class6<T, U>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	public T type => gparam_0;

	public U image_url => gparam_1;

	[DebuggerHidden]
	public Class6(T gparam_2, U gparam_3)
	{
		gparam_0 = gparam_2;
		gparam_1 = gparam_3;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class6<T, U> @class = obj as Class6<T, U>;
		if (this != @class)
		{
			if (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0))
			{
				return EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1);
			}
			return false;
		}
		return true;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (1691006143 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ type = {0}, image_url = {1} }}";
		object[] array = new object[2];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		return string.Format(null, format, array);
	}
}
