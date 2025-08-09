using Microsoft.Extensions.Localization;
using Server.Model;

namespace Server.Configuration
{
    public class MultipleStringLocalizer<T> : IMultipleStringLocalizer
    {
        private readonly IStringLocalizer<T> customLocalizer;
        private readonly IStringLocalizer<Resource> genericLocalizer;

        public MultipleStringLocalizer(IStringLocalizer<T> customLocalizer, IStringLocalizer<Resource> genericLocalizer)
        {
            this.genericLocalizer = genericLocalizer;
            this.customLocalizer = customLocalizer;
        }

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            var allStrings = new List<LocalizedString>();
            allStrings.AddRange(genericLocalizer.GetAllStrings(includeAncestorCultures));
            allStrings.AddRange(customLocalizer.GetAllStrings(includeAncestorCultures));
            return allStrings;
        }

        private LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            var localizedString = genericLocalizer[name];
            if (!localizedString.ResourceNotFound)
            {
                return localizedString;
            }

            localizedString = customLocalizer[name];
            if (!localizedString.ResourceNotFound)
            {
                return localizedString;
            }

            return new LocalizedString(name, "", resourceNotFound: true);
        }
    }
}
