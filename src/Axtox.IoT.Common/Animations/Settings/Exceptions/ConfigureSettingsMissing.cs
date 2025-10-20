using System;

namespace Axtox.IoT.Common.Animations.Settings.Exceptions
{
    public class ConfigureSettingsMissing : Exception
    {
        public ConfigureSettingsMissing(string message) : base(message) { }
    }
}
