using System.Reflection;
using System.Text.RegularExpressions;

// 纯反射打印 RevitAPI 真实签名，避免重写工具时反复猜测 API。
// 用法：
//   dotnet run --project tools/ApiDump -- "类型全名" "成员正则" ...      // 按类型查成员
//   dotnet run --project tools/ApiDump -- --search "成员正则"           // 全库搜索成员（跨所有类型）
// 注意：MSBuild/PowerShell 5.1 无法加载 .NET 8+ 的 RevitAPI.dll（BadImageFormat），
// 所以这里用 dotnet 运行 + AssemblyResolve 回调从 Revit 安装目录解析依赖。
const string RevitDirectory = @"C:\Program Files\Autodesk\Revit 2027";

AppDomain.CurrentDomain.AssemblyResolve += (_, eventArgs) =>
{
    var simpleName = new AssemblyName(eventArgs.Name).Name;
    var path = Path.Combine(RevitDirectory, simpleName + ".dll");
    return File.Exists(path) ? Assembly.LoadFrom(path) : null;
};

var assembly = Assembly.LoadFrom(Path.Combine(RevitDirectory, "RevitAPI.dll"));

if (args.Length >= 2 && args[0] == "--search")
{
    var searchRegex = new Regex(args[1], RegexOptions.IgnoreCase);
    foreach (var type in SafeTypes(assembly).OrderBy(type => type.FullName, StringComparer.Ordinal))
    {
        string[] members;
        try
        {
            members = type
                .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(member => searchRegex.IsMatch(member.Name))
                .Select(member => member.ToString() ?? string.Empty)
                .Distinct()
                .OrderBy(text => text, StringComparer.Ordinal)
                .ToArray();
        }
        catch (Exception)
        {
            continue;
        }

        if (members.Length == 0)
        {
            continue;
        }

        Console.WriteLine($"--- {type.FullName} ---");
        foreach (var member in members)
        {
            Console.WriteLine("  " + member);
        }
    }

    return;
}

var queries = args.Length > 0
    ? Chunk(args, 2).Select(pair => (Type: pair[0], Pattern: pair.Length > 1 ? pair[1] : ".")).ToArray()
    : new (string Type, string Pattern)[]
    {
        ("Autodesk.Revit.DB.CompoundStructure", "Layer"),
        ("Autodesk.Revit.DB.Material", "Transparency|MaterialClass|Color|Create"),
        ("Autodesk.Revit.DB.Document", "NewSpot"),
    };

foreach (var (typeName, pattern) in queries)
{
    var type = assembly.GetType(typeName);
    Console.WriteLine($"=== {typeName} (/{pattern}/) ===");
    if (type is null)
    {
        Console.WriteLine("  (type missing)");
        continue;
    }

    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
    var members = type
        .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        .Where(member => regex.IsMatch(member.Name))
        .Select(member => member.ToString() ?? string.Empty)
        .Distinct()
        .OrderBy(text => text, StringComparer.Ordinal);

    foreach (var member in members)
    {
        Console.WriteLine("  " + member);
    }
}

static IEnumerable<Type> SafeTypes(Assembly assembly)
{
    try
    {
        return assembly.GetTypes();
    }
    catch (ReflectionTypeLoadException ex)
    {
        return ex.Types.Where(type => type is not null).Select(type => type!);
    }
}

static IEnumerable<string[]> Chunk(string[] items, int size)
{
    for (var i = 0; i + size <= items.Length; i += size)
    {
        yield return items.Skip(i).Take(size).ToArray();
    }
}
