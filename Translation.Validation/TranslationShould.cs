using Nemeio.Core.DataModels.Locale;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Translation.Validation
{
    [TestFixture]
    public class TranslationShould
    {
        private static IEnumerable<string> Languages = new List<string>() { "fr-FR.xml", "en-US.xml" };

        [Test]
        [TestCaseSource(nameof(Languages))]
        public void TranslateFile_Exists(string filename)
        {
            var fileAssemblyPath = $"Nemeio.Core.Resources.{filename}";

            var languageExists = ResourceExists(fileAssemblyPath);
            if (!languageExists)
            {
                throw new FileNotFoundException($"<{filename}> doesn't exists");
            }
        }

        [Test]
        [TestCaseSource(nameof(Languages))]
        public void TranslateFile_IsValid(string filename)
        {
            var fileAssemblyPath = $"Nemeio.Core.Resources.{filename}";

            var xmlSerializer = new XmlSerializer(typeof(Language));

            using (var stream = GetResourceStream(fileAssemblyPath))
            {
                var loadedLanguage = (Language)xmlSerializer.Deserialize(stream);
            }
        }

        [Test]
        [TestCaseSource(nameof(Languages))]
        public void TranslateFile_HasAllKeys(string filename)
        {
            var fileAssemblyPath = $"Nemeio.Core.Resources.{filename}";

            var xmlSerializer = new XmlSerializer(typeof(Language));

            using (var stream = GetResourceStream(fileAssemblyPath))
            {
                var stringIds = Enum.GetValues(typeof(StringId));
                var stringIdsState = new Dictionary<StringId, bool>();
                var loadedLanguage = (Language)xmlSerializer.Deserialize(stream);

                var stringIdsList = GetStringIds();

                foreach (var id in stringIdsList)
                {
                    if (loadedLanguage.Entries == null)
                    {
                        stringIdsState.Add(id, false);
                    }
                    else
                    {
                        var currentStringIdState = loadedLanguage.Entries.Any(x => x.Key == id);

                        stringIdsState.Add(id, currentStringIdState);
                    }
                }

                bool hasError = false;

                Console.WriteLine($">> Analyse for {filename}");
                Console.WriteLine($"Some keys are missing");
                foreach (var state in stringIdsState)
                {
                    if (!state.Value)
                    {
                        hasError = true;

                        Console.WriteLine($"- {state.Key}");
                    }
                }

                if (hasError)
                {
                    throw new InvalidDataException($"Something goes wrong with file {filename}");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(Languages))]
        public void TranslateFile_HasAllValueFilled(string filename)
        {
            var fileAssemblyPath = $"Nemeio.Core.Resources.{filename}";

            var xmlSerializer = new XmlSerializer(typeof(Language));

            using (var stream = GetResourceStream(fileAssemblyPath))
            {
                var hasError = false;
                var stringIdsEmptyKey = new List<StringId>();
                var loadedLanguage = (Language)xmlSerializer.Deserialize(stream);

                foreach (var entry in loadedLanguage.Entries)
                {
                    if (entry.TranslateValue == null)
                    {
                        hasError = true;
                        stringIdsEmptyKey.Add(entry.Key);
                    }
                    else if (entry.TranslateValue == string.Empty)
                    {
                        hasError = true;
                        stringIdsEmptyKey.Add(entry.Key);
                    }
                }

                Console.WriteLine($"On file {filename} there keys are empty:");
                foreach (var key in stringIdsEmptyKey)
                {
                    Console.WriteLine($"- {key}");
                }

                if (hasError)
                {
                    throw new InvalidDataException($"Something goes wrong with file {filename}");
                }
            }
        }

        private List<StringId> GetStringIds()
        {
            var result = new List<StringId>();
            var stringIds = Enum.GetValues(typeof(StringId));

            foreach (var id in stringIds)
            {
                var currentStringId = (StringId)id;

                result.Add(currentStringId);
            }

            return result;
        }

        private Assembly GetAssembly()
        {
            return typeof(Language).Assembly;
        }

        private bool ResourceExists(string resourceName)
        {
            var resourceNames = GetAssembly().GetManifestResourceNames();

            return resourceNames.Contains(resourceName);
        }

        private Stream GetResourceStream(string filename)
        {
            return GetAssembly().GetManifestResourceStream(filename);
        }
    }
}
