using System;

namespace Axtox.IoT.Common.Storage
{
    /// <summary>
    /// Represents a stateful object that can be persisted and retrieved from storage.
    /// </summary>
    /// <remarks>
    /// This interface defines the contract for objects whose state needs to be persisted across application restarts or sessions.
    /// Implementations can use various storage mechanisms (file system, databases, cloud storage, etc.) to save and load state.
    /// 
    /// This approach is similar to Microsoft Orleans' Grain Persistent State pattern, where state is automatically 
    /// persisted and restored based on a unique key identifier.
    /// </remarks>
    public interface IState
    {
        /// <summary>
        /// Gets the unique identifier for this state object.
        /// </summary>
        /// <remarks>
        /// The Key is used by storage implementations (e.g., <see cref="FileSystemJsonStateStorage"/>) 
        /// to uniquely identify and retrieve the persisted state. This key should remain constant 
        /// for the lifetime of the stateful object.
        /// </remarks>
        Guid Key { get; }
    }
}
