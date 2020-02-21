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

    using SatisfactoryTools.Storage;

    public class TestStorageProvider : IStorageProvider
    {
        private readonly NameValueCollection values = new NameValueCollection();

        public event EventHandler<StorageChangedEventArgs> Changed;

        public event EventHandler<StorageChangingEventArgs> Changing;

        public Task ClearAsync()
        {
            this.values.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainKeyAsync(string key)
        {
            return Task.FromResult(this.values.AllKeys.Any(x => x == key));
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            await using MemoryStream memory = new MemoryStream(Encoding.UTF8.GetBytes(this.values[key]));
            return await JsonSerializer.DeserializeAsync<T>(memory, SerializerOptions.JsonSerializerOptions);
        }

        public Task<string> KeyAsync(int index)
        {
            return Task.FromResult(this.values.GetKey(index));
        }

        public Task<int> LengthAsync()
        {
            return Task.FromResult(this.values.Count);
        }

        public Task RemoveItemAsync(string key)
        {
            this.values.Remove(key);
            return Task.CompletedTask;
        }

        public async Task SetItemAsync(string key, object data)
        {
            await using MemoryStream memory = new MemoryStream();
            await JsonSerializer.SerializeAsync(memory, data).ConfigureAwait(false);
            memory.Position = 0;
            using StreamReader reader = new StreamReader(memory);
            string json = reader.ReadToEnd();
            this.values.Set(key, json);
        }
    }
}