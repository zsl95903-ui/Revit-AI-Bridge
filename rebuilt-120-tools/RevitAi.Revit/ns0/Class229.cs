using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ joinOrder = {joinOrder} }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class229<T>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	public T joinOrder => gparam_0;

	[DebuggerHidden]
	public Class229(T gparam_1)
	{
		gparam_0 = gparam_1;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class229<T> @class = obj as Class229<T>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return 1421011282 + EqualityComparer<T>.Default.GetHashCode(gparam_0);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ joinOrder = {0} }}";
		object[] array = new object[1];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		return string.Format(null, format, array);
	}
}
