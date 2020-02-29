namespace SatisfactoryTools.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Storage;

    public class TestStorageProvider : IStorageProvider
    {
        private readonly JsonSerializerOptions serializerOptions;
        private readonly NameValueCollection values = new NameValueCollection();

        public event EventHandler<StorageChangedEventArgs> Changed;

        public event EventHandler<StorageChangingEventArgs> Changing;

        public event EventHandler Cleared;

        public TestStorageProvider(JsonSerializerOptions serializerOptions)
        {
            this.serializerOptions = serializerOptions;
        }

        public Task ClearAsync()
        {
            return this.ChangeAsync<object>(null, null, () =>
            {
                this.values.Clear();
                this.Cleared?.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            });
        }

        public Task<bool> ContainKeyAsync(string key)
        {
            return Task.FromResult(this.values.AllKeys.Any(x => x == key));
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            string value = this.values[key];

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            await using var memory = new MemoryStream(Encoding.UTF8.GetBytes(value));
            return await JsonSerializer.DeserializeAsync<T>(memory, this.serializerOptions);
        }

        public Task<string> KeyAsync(int index)
        {
            return Task.FromResult(this.values.GetKey(index));
        }

        public Task<int> LengthAsync()
        {
            return Task.FromResult(this.values.Count);
        }

        public Task RemoveItemAsync<T>(string key)
        {
            return this.ChangeAsync<T>(key, default, () =>
            {
                this.values.Remove(key);
                return Task.CompletedTask;
            });
        }

        public Task SetItemAsync<T>(string key, T data)
        {
            return this.ChangeAsync(key, data, async () =>
            {
                await using var memory = new MemoryStream();
                await JsonSerializer.SerializeAsync(memory, data).ConfigureAwait(false);
                memory.Position = 0;

                using var reader = new StreamReader(memory);
                string json = reader.ReadToEnd();
                this.values.Set(key, json);
            });
        }

        private async Task ChangeAsync<T>(string key, T data, Func<Task> changer)
        {
            if (this.Changing != null || this.Changed != null)
            {
                T oldValue = key == null ? default : await this.GetItemAsync<T>(key).ConfigureAwait(false);

                var args = new StorageChangingEventArgs {Key = key, OldValue = oldValue, NewValue = data};

                this.Changing?.Invoke(this, args);

                if (!args.Cancel)
                {
                    await changer().ConfigureAwait(false);

                    this.Changed?.Invoke(this, args);
                }
            }
            else
            {
                await changer().ConfigureAwait(false);
            }
        }
    }
}