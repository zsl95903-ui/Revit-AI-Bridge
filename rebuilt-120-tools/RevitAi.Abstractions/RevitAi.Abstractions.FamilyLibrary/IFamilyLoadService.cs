using System.Threading.Tasks;

namespace RevitAi.Abstractions.FamilyLibrary;

public interface IFamilyLoadService
{
	Task<bool> LoadFamilyAsync(string filePath, string familyName);

	Task<bool> FamilyExistsAsync(string familyName);
}
