﻿using LiteDB;
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
            get { lock (_sync) return GetValue(ref _currentLanguage); }
            set { lock (_sync) _currentLanguage = value; }
        }

        public IEnumerable<CultureInfo> AllLanguages => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pl-PL")
        };

        public string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");

        private bool _isInitialized = false;
        private static readonly object _sync = new object();

        public void Persist()
        {
            lock (_sync)
            {
                if (!_isInitialized)
                    throw new InvalidOperationException("Settings not initialized. Initialize first.");

                using (var db = new LiteDatabase(DbPath))
                {
                    var settings = new GlobalSettingsStorage
                    {
                        CurrentLanguageShortName = CurrentLanguage.Name
                    };

                    var collection = db.GetCollection<GlobalSettingsStorage>(nameof(GlobalSettingsStorage));
                    if (collection.Count() != 0)
                    {
                        var id = collection.FindOne(Query.All()).Id;
                        settings.Id = id;
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
                lock (_sync)
                {
                    if (!_isInitialized)
                    {
                        using (var db = new LiteDatabase(DbPath))
                        {
                            var collection = db.GetCollection<GlobalSettingsStorage>(nameof(GlobalSettingsStorage));
                            var documents = collection.FindAll().ToList();
                            var savedSettings = documents.Count == 0 ? new GlobalSettingsStorage() : documents[0];

                            _currentLanguage = new CultureInfo(savedSettings.CurrentLanguageShortName);
                        }
                        _isInitialized = true;
                    }
                }
            }

            return value;
        }
    }

    internal class GlobalSettingsStorage
    {
        [BsonId]
        public int Id { get; set; }
        public string CurrentLanguageShortName { get; set; }

        public GlobalSettingsStorage()
        {
            CurrentLanguageShortName = "en-US";
        }
    }
}