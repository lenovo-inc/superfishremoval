using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Localization
{
    /// <summary>
    /// Responsible for loading strongly typed localizations from embedded XML translation files
    /// </summary>
    /// <remarks>
    /// The primary reason for not using standard localization (resx) is to avoid  satellite assemblies
    /// </remarks>
    internal static class LocalizationManager
    {
        /// <summary>
        /// Gets a set of translations for the most applicable user language
        /// </summary>
        /// <returns></returns>
        public static AppStrings Get()
        {
            if (_cachedAppStrings == null)
            {
                AppStrings appStrings = null;
                try
                {
                    LocalizationSet localizations = LocateIdealTranslationFile(translationFilePrefix: "AppStrings");
                    appStrings = MapTranslationItemsToStronglyTypedInstance<AppStrings>(localizations);
                }
                catch(Exception ex)
                {
                    Logging.Logger.Log(ex, "Unable to locate translations");
                }

                _cachedAppStrings = appStrings ?? new AppStrings(); // prevent from trying again
            }
            return _cachedAppStrings;
        }

        private static AppStrings _cachedAppStrings;

        /// <summary>
        /// Examines embedded translation files and looks for the most ideal one based on current culture
        /// </summary>
        /// <returns></returns>
        private static LocalizationSet LocateIdealTranslationFile(string translationFilePrefix)
        {
            LocalizationSet localizationSet = null;
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            string currentCultureName = currentCulture.Name;
            var languageMapping = new Dictionary<String, List<String>>()
            {
                {"zh-Hans", new List<string>()
                {
                    "zh", "zh-CHS", "zh-CHT", "zh-CN", "zh-SG",
                }},

                {"zh-Hant", new List<string>()
                {
                    "zh-HK", "zh-MO", "zh-TW",
                }},

                {"sr-Latn", new List<string>()
                {
                    "sr-Latn-RS", "sr-Cyrl-RS", "sr-Latn-ME", "sr-Cyrl-ME",
                }},
            };

            // Map "zh-CHS" to "zh-HANS"
            foreach(var mapping in languageMapping)
            {
                if(mapping.Value.Contains(currentCultureName, StringComparer.InvariantCultureIgnoreCase))
                {
                    currentCultureName = mapping.Key;
                    break;
                }
            }


            var listOfTranslationFiles = GetLocalizationFilePaths();
            if(listOfTranslationFiles == null || !listOfTranslationFiles.Any())
            {
                throw new Exception("No localization files found");
            }

            List<string> prioritizedLanguages = new List<string>()
            {
                currentCultureName, // "en-US"
                currentCulture.TwoLetterISOLanguageName, // "en"
                "en", // en is default
            };

            foreach(var priority in prioritizedLanguages)
            {
                string predictedFileName = String.Format("{0}_{1}.xml", translationFilePrefix, priority);
                try
                {
                    string foundMatchingTranslationFile = listOfTranslationFiles.FirstOrDefault(filePath =>
                        !String.IsNullOrWhiteSpace(filePath) &&
                        filePath.EndsWith(predictedFileName, StringComparison.InvariantCultureIgnoreCase));

                    if (!String.IsNullOrWhiteSpace(foundMatchingTranslationFile))
                    {
                        localizationSet = LoadStringsFromFile(foundMatchingTranslationFile);
                        if(localizationSet != null)
                        {
                            break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logging.Logger.Log(ex, "Unable to process localization file {0} ", predictedFileName);
                }
            }

            if(localizationSet == null)
            {
                throw new System.IO.FileNotFoundException("No valid translation files matching expected values");
            }
            return localizationSet;
        }

        /// <summary>
        /// Maps each item in a localized set into a strongly typed stringtable
        /// </summary>
        /// <typeparam name="T">The type that contains properties for each string</typeparam>
        /// <param name="translations"></param>
        /// <returns></returns>
        private static T MapTranslationItemsToStronglyTypedInstance<T>(LocalizationSet translations) where T : class
        {
            T result = (T)Activator.CreateInstance(typeof(T));

            var properties = typeof(T).GetProperties();
            foreach(var property in properties)
            {
                string name = property.Name;
                var matchingTranslatedItem = translations.ItemList.FirstOrDefault(item => 
                    item.Key != null &&
                    item.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                if (matchingTranslatedItem != null)
                {
                    property.SetMethod.Invoke(result, new object[] { matchingTranslatedItem.Value });
                }
            }


            return result;
        }

        /// <summary>
        /// Deserializes a translation xml file into an instance
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static LocalizationSet LoadStringsFromFile(string filePath)
        {
            LocalizationSet set = null;
            using (var stream = (System.Reflection.Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(filePath)))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    string xmlFileContents = reader.ReadToEnd();
                    var deserializedObject = Serializer.Deserialize<LocalizationSet>(xmlFileContents);
                    set = deserializedObject;

                }
            }
            return set;
        }

        /// <summary>
        /// Locates all xml files that may be translation files
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetLocalizationFilePaths()
        {
            var dataFiles = System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(s =>
                    s.StartsWith(typeof(LocalizationManager).Namespace) &&
                    s.EndsWith(".xml"));
            return dataFiles;
        }

    }

  
}
