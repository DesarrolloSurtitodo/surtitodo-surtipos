namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;

class ModuleModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public string? Icon { get; init; }
}
