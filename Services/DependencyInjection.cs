using Microsoft.Extensions.DependencyInjection;

namespace DND.Services;

/// <summary>
/// Registers every DND storage and autosave service with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the local persistence and autosave services used by the character sheet.
    /// </summary>
    public static IServiceCollection AddDndServices(this IServiceCollection services)
    {
        services.AddSingleton<IStorageStatusService, StorageStatusService>();
        services.AddScoped<ICharacterStore, IndexedDbCharacterStore>();
        services.AddScoped<ICharacterAutosaveService, CharacterAutosaveService>();
        services.AddScoped<ISheetSectionStateService, SheetSectionStateService>();
        services.AddScoped<ICharacterDuplicator, CharacterDuplicator>();
        return services;
    }
}
