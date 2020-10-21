using System.Reflection;

using WPFLocalizeExtension.Extensions;

namespace Shadowsocks.WPF.Localization
{
    public static class LocalizationProvider
    {
        public static T GetLocalizedValue<T>(string key) =>
            LocExtension.GetLocalizedValue<T>($"{Assembly.GetCallingAssembly().GetName().Name}:Strings:{key}");
    }
}
