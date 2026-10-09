using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface ICreationService
{
	object? CreateLevel(object document, double elevation, string name);

	object? CreateGrid(object document, object start, object end, string? name = null);

	object? CreateDimension(object document, object view, IList<object> references, object position);

	object? CreateRoomAtPoint(object document, object level, object point, object? transaction = null);

	object? CreateRoom(object document, double x, double y, int levelId);

	IEnumerable<object> CreateAllRoomsInLevel(object document, object level, object? transaction = null);

	IEnumerable<object> CreateAllRoomsInLevel(object document, object level, object? phase, object? transaction = null);
}
