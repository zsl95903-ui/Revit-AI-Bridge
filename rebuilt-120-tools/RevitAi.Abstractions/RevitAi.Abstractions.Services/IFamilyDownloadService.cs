using System.Threading.Tasks;

namespace RevitAi.Abstractions.Services;

public interface IFamilyDownloadService
{
	Task<FamilyDownloadResult> DownloadFamilyFileAsync(string familyName);
}
