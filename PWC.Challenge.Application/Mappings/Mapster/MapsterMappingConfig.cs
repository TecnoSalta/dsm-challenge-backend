using Mapster;
using System.Reflection;

namespace PWC.Challenge.Application.Mappings.Mapster;


public static class MapsterMappingConfig
{
    public static void RegisterMappings(Assembly assembly)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Escanear todos los ensamblados para encontrar configuraciones de mapeo
        config.Scan(assembly);
    }
}
