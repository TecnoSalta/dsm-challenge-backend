using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// Obtiene un valor del cache
        /// </summary>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Guarda un valor en el cache con expiración opcional
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un valor del cache
        /// </summary>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si una clave existe en el cache
        /// </summary>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina todas las claves que coincidan con un patrón
        /// </summary>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene o crea un valor del cache si no existe
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    }
}