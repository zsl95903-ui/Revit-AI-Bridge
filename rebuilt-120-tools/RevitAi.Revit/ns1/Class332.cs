using Autodesk.Revit.DB;

namespace ns1;

internal static class Class332
{
	internal static class Class333
	{
		public static double smethod_0(double double_0, ForgeTypeId forgeTypeId_0)
		{
			return UnitUtils.ConvertToInternalUnits(double_0, forgeTypeId_0);
		}

		public static double smethod_1(double double_0, ForgeTypeId forgeTypeId_0)
		{
			return UnitUtils.ConvertFromInternalUnits(double_0, forgeTypeId_0);
		}
	}

	internal static class Class334
	{
		public static readonly ForgeTypeId forgeTypeId_0 = UnitTypeId.Millimeters;

		public static readonly ForgeTypeId forgeTypeId_1 = UnitTypeId.SquareMeters;

		public static readonly ForgeTypeId forgeTypeId_2 = UnitTypeId.CubicMeters;

		public static readonly ForgeTypeId forgeTypeId_3 = UnitTypeId.Degrees;
	}

	public static int smethod_0(this ElementId elementId_0)
	{
		return (int)elementId_0.Value;
	}
}
