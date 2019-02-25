using LiteDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataVisualization.Services
{
    public sealed class GlobalSettings
    {
        private CultureInfo _currentLanguage;
        public CultureInfo CurrentLanguage
        {
            get { lock (Sync) return GetValue(ref _currentLanguage); }
            set { lock (Sync) _currentLanguage = value; }
        }

        public IEnumerable<CultureInfo> AllLanguages => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pl-PL")
        };

        public string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");

        private int _pointsCount;
        public int PointsCount
        {
            get { lock (Sync) return GetValue(ref _pointsCount); }
            set { lock (Sync) _pointsCount = value; }
        }

        private bool _isInitialized;
        private static readonly object Sync = new object();

        public void Persist()
        {
            lock (Sync)
            {
                if (!_isInitialized)
                    throw new InvalidOperationException("Settings not initialized. Initialize first.");

                using (var db = new LiteDatabase(DbPath))
                {
                    var settings = new GlobalSettingsStorage
                    {
                        CurrentLanguageShortName = CurrentLanguage.Name,
                        PointsCount = PointsCount
                    };

                    var collection = db.GetCollection<GlobalSettingsStorage>(nameof(GlobalSettingsStorage));
                    if (collection.Count() != 0)
                    {
                        var document = collection.FindOne(Query.All());
                        settings.Id = document.Id;

                        if (document.Equals(settings))
                            return;

                        collection.Update(settings);
                    }
                    else
                    {
                        collection.Insert(settings);
                    }
                }
            }
        }

        private T GetValue<T>(ref T value)
        {
            if (!_isInitialized)
            {
                lock (Sync)
                {
                    if (!_isInitialized)
                    {
                        using (var db = new LiteDatabase(DbPath))
                        {
                            var collection = db.GetCollection<GlobalSettingsStorage>(nameof(GlobalSettingsStorage));
                            var documents = collection.FindAll().ToList();
                            var savedSettings = documents.Count == 0 ? new GlobalSettingsStorage() : documents[0];

                            _currentLanguage = new CultureInfo(savedSettings.CurrentLanguageShortName);
                            _pointsCount = savedSettings.PointsCount;
                        }
                        _isInitialized = true;
                    }
                }
            }

            return value;
        }
    }

    internal class GlobalSettingsStorage : IEquatable<GlobalSettingsStorage>
    {
        [BsonId]
        public int Id { get; set; }
        public string CurrentLanguageShortName { get; set; }
        public int PointsCount { get; set; }

        public GlobalSettingsStorage()
        {
            CurrentLanguageShortName = "en-US";
            PointsCount = 1000;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GlobalSettingsStorage);
        }

        public bool Equals(GlobalSettingsStorage other)
        {
            return other != null &&
                   Id == other.Id &&
                   CurrentLanguageShortName.Equals(other.CurrentLanguageShortName);
        }

        public override int GetHashCode()
        {
            var hashCode = -1608065743;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CurrentLanguageShortName);
            return hashCode;
        }
    }
}
