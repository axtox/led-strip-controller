using Axtox.IoT.Common.Storage;
using Axtox.IoT.Common.System.Extensions;
using nanoFramework.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Axtox.IoT.Storage.FileSystem.Json
{
    public class FileSystemJsonStateStorage : IStateStorage
    {
        private const string StorageDirectoryName = "state-storage";

        private readonly string StorageFullDirectory = $"{StorageDirectoryName}";
        private readonly string DriveLetter = "I:\\";

        public FileSystemJsonStateStorage()
        {
            var drives = DriveInfo.GetDrives();
            if (drives.Length == 0)
                throw new InvalidOperationException("No drives available for storage.");

            DriveLetter = drives[0].Name;
            StorageFullDirectory = Path.Combine(DriveLetter, StorageDirectoryName);
        }

        public IState Load(Guid key, Type stateType)
        {
            if (!stateType.ImplementsInterface(typeof(IState)))
                throw new ArgumentException("stateType must implement IState interface.", nameof(stateType));
            try
            {
                if (!File.Exists(Path.Combine(StorageFullDirectory, $"{key}")))
                    return null;

                var stateJson = File.ReadAllText(Path.Combine(StorageFullDirectory, $"{key}"));
                return (IState)JsonConvert.DeserializeObject(stateJson, stateType);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading state for key {key}: {e.Message}");
                return null;
            }
        }

        public void Save(IState state)
        {
            try
            {
                if (!Directory.Exists(StorageFullDirectory))
                    Directory.CreateDirectory(StorageFullDirectory);

                var stateJson = JsonConvert.SerializeObject(state);
                File.WriteAllText(Path.Combine(StorageFullDirectory, $"{state.Key}"), stateJson);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving state for key {state.Key}: {e.Message}");
            }
        }
    }

}
