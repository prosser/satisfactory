namespace SatisfactoryTools.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Storage;

    public class FileSystemStorageProvider : IStorageProvider, IDisposable
    {
        private const string DefaultSaveFileName = "SatisfactoryTools.json";

        private const string UnknownValuesKey = "unknownValues";

        private readonly string saveFilesDirectory;

        private readonly JsonSerializerOptions serializerOptions;

        private readonly Dictionary<string, string> untyped = new Dictionary<string, string>();

        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        private readonly FileSystemWatcher watcher;

        public FileSystemStorageProvider(JsonSerializerOptions serializerOptions)
        {
            this.serializerOptions = serializerOptions;
            this.saveFilesDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SatisfactoryTools");

            Directory.CreateDirectory(this.saveFilesDirectory);
            this.watcher = new FileSystemWatcher(this.saveFilesDirectory, this.SaveFileName);
            this.watcher.Changed += this.WatcherOnChanged;
        }

        public string SaveFileName { get; } = DefaultSaveFileName;

        public void Dispose()
        {
            this.watcher.Changed -= this.WatcherOnChanged;
            this.watcher.Dispose();
        }

        public event EventHandler<StorageChangedEventArgs> Changed;

        public event EventHandler<StorageChangingEventArgs> Changing;

        public async Task ClearAsync()
        {
            this.untyped.Clear();
            this.values.Clear();
            this.Cleared?.Invoke(this, EventArgs.Empty);
            await this.SaveAsync(Path.Combine(this.saveFilesDirectory, this.SaveFileName)).ConfigureAwait(false);
        }

        public event EventHandler Cleared;

        public Task<bool> ContainKeyAsync(string key)
        {
            return Task.FromResult(this.values.ContainsKey(key) || this.untyped.ContainsKey(key));
        }

        public Task<T> GetItemAsync<T>(string key)
        {
            T value = default;

            try
            {
                if (this.values.TryGetValue(key, out object obj))
                {
                    value = (T)obj;
                }
                else if (this.untyped.TryGetValue(key, out string json))
                {
                    value = JsonSerializer.Deserialize<T>(json, this.serializerOptions);
                    this.untyped.Remove(key);
                    this.values.Add(key, value);
                }
            }
            catch (JsonException)
            {
            }

            return Task.FromResult(value);
        }

        public Task<string> KeyAsync(int index)
        {
            throw new NotSupportedException();
        }

        public Task<int> LengthAsync()
        {
            return Task.FromResult(this.values.Count);
        }

        public Task RemoveItemAsync<T>(string key)
        {
            this.values.Remove(key);
            this.untyped.Remove(key);
            return Task.CompletedTask;
        }

        public async Task SetItemAsync<T>(string key, T data)
        {
            T oldValue = await this.GetItemAsync<T>(key).ConfigureAwait(false);
            var args = new StorageChangingEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = data
            };
            this.Changing?.Invoke(this, args);

            if (args.Cancel)
            {
                return;
            }

            this.untyped.Remove(key);
            this.values[key] = data;
            await this.SaveAsync(Path.Combine(this.saveFilesDirectory, this.SaveFileName)).ConfigureAwait(false);
            this.Changed?.Invoke(this, args);
        }

        private async Task LoadAsync(string fullPath)
        {
            this.untyped.Clear();

            string fileJson = await File.ReadAllTextAsync(fullPath, Encoding.UTF8).ConfigureAwait(false);

            Dictionary<string, string> loadedValues =
                JsonSerializer.Deserialize<Dictionary<string, string>>(fileJson, this.serializerOptions);

            foreach ((string key, string json) in loadedValues)
            {
                if (key == UnknownValuesKey)
                {
                    try
                    {
                        foreach ((string untypedKey, string untypedValue) in JsonSerializer
                            .Deserialize<Dictionary<string, string>>(json, this.serializerOptions))
                        {
                            this.untyped.Add(untypedKey, untypedValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else if (this.values.TryGetValue(key, out object existingValue))
                {
                    object newValue =
                        JsonSerializer.Deserialize(json, existingValue.GetType(), this.serializerOptions);
                    var args = new StorageChangingEventArgs
                    {
                        Key = key,
                        OldValue = existingValue,
                        NewValue = json
                    };
                    this.Changing?.Invoke(this, args);

                    if (args.Cancel)
                    {
                        continue;
                    }

                    this.values[key] = newValue;
                    this.Changed?.Invoke(this, args);
                }
                else
                {
                    this.untyped[key] = json;
                }
            }
        }

        private async Task SaveAsync(string fullPath)
        {
            if (this.untyped.Count > 0)
            {
                this.values[UnknownValuesKey] = JsonSerializer.Serialize(this.untyped, this.serializerOptions);
            }

            await File.WriteAllTextAsync(fullPath, JsonSerializer.Serialize(this.values, this.serializerOptions))
                .ConfigureAwait(false);

            this.values.Remove(UnknownValuesKey);
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (string.Equals(e.Name, this.SaveFileName, StringComparison.OrdinalIgnoreCase))
            {
                this.LoadAsync(e.FullPath).RunSynchronously();
            }
        }
    }
}