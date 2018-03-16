using System;
using System.IO;

namespace DataVisualization.Services
{
    internal class Settings
    {
        private Settings() { }

        public static Settings Instance { get; } = new Settings();

        public string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");
    }
}
