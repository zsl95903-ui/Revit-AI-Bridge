using System.Threading.Tasks;

namespace RevitAi.Abstractions.Services;

public interface IDeviceService
{
	Task<string?> GetDeviceIdAsync();
}
