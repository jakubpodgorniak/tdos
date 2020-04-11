using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace TDOS.Game.Configuration.Helpers
{
    public class ConfigurationWatcher : IDisposable
    {
        public ConfigurationWatcher(
            string configurationFilePath,
            Action<Configuration> onConfigurationChanged)
        {
            var directory = Path.GetDirectoryName(configurationFilePath);
            var fileName = Path.GetFileName(configurationFilePath);

            this.watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = fileName,
                EnableRaisingEvents = true
            };

            watcher.Changed += Watcher_Changed;

            this.configurationFilePath = configurationFilePath;
            this.onConfigurationChanged = onConfigurationChanged;
        }

        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing)
        {
            if (disposing)
            {
                watcher.Dispose();
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            int counter = 0;

            while (true)
            {
                try
                {
                    onConfigurationChanged(JsonConvert.DeserializeObject<Configuration>(
                        File.ReadAllText(configurationFilePath)));

                    break;
                }
                catch (IOException) when (counter < 5)
                {
                    counter++;

                    Thread.Sleep(100);
                }
            }
        }

        private readonly FileSystemWatcher watcher;
        private readonly string configurationFilePath;
        private readonly Action<Configuration> onConfigurationChanged;
    }
}
