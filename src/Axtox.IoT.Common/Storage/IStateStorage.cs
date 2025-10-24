using System;

namespace Axtox.IoT.Common.Storage
{
    /// <summary>
    /// Defines a contract for storing and loading state objects. This interface provides a generic mechanism
    /// for state persistence, allowing different storage implementations (e.g., file system, database, cloud storage).
    /// </summary>
    /// <remarks>
    /// This approach is similar to Microsoft Orleans' persistent state pattern, where state can be stored
    /// in various backing stores without changing the application logic.
    /// 
    /// Implementations can include:
    /// <list type="bullet">
    ///     <item><description><see cref="FileSystemJsonStateStorage"/> - Stores state as JSON files on the file system</description></item>
    ///     <item><description>Database storage - Stores state in SQL or NoSQL databases</description></item>
    ///     <item><description>In-memory storage - Stores state in memory for testing or caching scenarios</description></item>
    ///     <item><description>Cloud storage - Stores state in Azure Blob Storage, AWS S3, etc.</description></item>
    /// </list>
    /// </remarks>
    public interface IStateStorage
    {
        /// <summary>
        /// Saves the specified state object to the underlying storage medium.
        /// </summary>
        /// <param name="state">The state object to save. Must implement <see cref="IState"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="state"/> is null.</exception>
        void Save(IState state);

        /// <summary>
        /// Loads a state object from the underlying storage medium using the specified key.
        /// </summary>
        /// <param name="key">The unique identifier of the state to load.</param>
        /// <param name="stateType">The type of the state object to deserialize. Must implement <see cref="IState"/>.</param>
        /// <returns>The loaded state object, or null if no state exists for the given key.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="stateType"/> does not implement <see cref="IState"/>.</exception>
        IState Load(Guid key, Type stateType);
    }
}
