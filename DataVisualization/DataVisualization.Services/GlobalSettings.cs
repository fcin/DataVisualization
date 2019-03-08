using DataVisualization.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileMode = System.IO.FileMode;

namespace DataVisualization.Services
{
    public static class GlobalSettings
    {
        private static CultureInfo _currentLanguage;
        public static CultureInfo CurrentLanguage
        {
            get { lock (Sync) return _currentLanguage; }
            set { lock (Sync) _currentLanguage = value; }
        }

        public static IEnumerable<CultureInfo> AllLanguages => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pl-PL")
        };

        public static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");

        private static int _pointsCount;
        public static int PointsCount
        {
            get { lock (Sync) return _pointsCount; }
            set { lock (Sync) _pointsCount = value; }
        }

        public static bool LoadedSuccessfully = true;
        public static Exception LoadedWithException = null;

        private static bool _isLoaded;
        private static readonly object Sync = new object();
        private static readonly string SettingsFilePath;

        static GlobalSettings()
        {
            var settingsRelativeFilePath = ConfigurationManager.AppSettings["SettingsRelativeFilePath"];
            SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsRelativeFilePath);
        }

        public static void Save()
        {
            lock (Sync)
            {
                if (!_isLoaded)
                    throw new InvalidOperationException("Load settings first");

                var serializer = new XmlSerializer(typeof(GlobalSettingsStorage));

                using (var file = File.Create(SettingsFilePath))
                {
                    var settings = new GlobalSettingsStorage
                    {
                        CurrentLanguageShortName = CurrentLanguage.Name,
                        PointsCount = PointsCount
                    };

                    serializer.Serialize(file, settings);
                }
            }
        }

        public static bool TryLoad()
        {
            lock (Sync)
            {
                try
                {
                    var fileExists = File.Exists(SettingsFilePath);

                    var deserializer = new XmlSerializer(typeof(GlobalSettingsStorage));
                    using (var reader = File.Open(SettingsFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        GlobalSettingsStorage settings = null;
                        if (!fileExists)
                        {
                            settings = new GlobalSettingsStorage();
                            deserializer.Serialize(reader, settings);
                        }
                        else
                        {
                            settings = (GlobalSettingsStorage)deserializer.Deserialize(reader);
                        }

                        _currentLanguage = new CultureInfo(settings.CurrentLanguageShortName);
                        _pointsCount = settings.PointsCount;
                    }

                    _isLoaded = true;
                }
                catch (InvalidOperationException ex) when (ex.InnerException is XmlException)
                {
                    LoadedSuccessfully = false;
                    LoadedWithException = new SettingsLoadingException("XML settings file corrupted", ex);
                    return false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    LoadedSuccessfully = false;
                    LoadedWithException = new SettingsLoadingException("You do not have access to the settings file.", ex);
                    return false;
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is DirectoryNotFoundException || ex is IOException)
                {
                    LoadedSuccessfully = false;
                    LoadedWithException = new SettingsLoadingException("Failed to load settings file", ex);
                    return false;
                }

                return true;
            }
        }
    }

    [Serializable]
    public sealed class GlobalSettingsStorage
    {
        public string CurrentLanguageShortName { get; set; }
        public int PointsCount { get; set; }

        public GlobalSettingsStorage()
        {
            CurrentLanguageShortName = ConfigurationManager.AppSettings["DefaultLanguage"];
            PointsCount = int.Parse(ConfigurationManager.AppSettings["DefaultPointsCount"]);
        }
    }
}
