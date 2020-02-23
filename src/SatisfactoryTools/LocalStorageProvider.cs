namespace SatisfactoryTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazored.LocalStorage;
    using Storage;

    public class LocalStorageProvider : IStorageProvider
    {
        private readonly ILocalStorageService localStorage;

        public LocalStorageProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            localStorage.Changed += this.LocalStorage_OnChanged;
            localStorage.Changing += this.LocalStorage_OnChanging;
        }

        public event EventHandler<StorageChangedEventArgs> Changed;

        public event EventHandler<StorageChangingEventArgs> Changing;

        public event EventHandler Cleared;

        public async Task ClearAsync()
        {
            await this.localStorage.ClearAsync().ConfigureAwait(false);
            this.Cleared?.Invoke(this, EventArgs.Empty);
        }

        public Task<bool> ContainKeyAsync(string key)
        {
            return this.localStorage.ContainKeyAsync(key);
        }

        public Task<T> GetItemAsync<T>(string key)
        {
            return this.localStorage.GetItemAsync<T>(key);
        }

        public Task<string> KeyAsync(int index)
        {
            return this.localStorage.KeyAsync(index);
        }

        public Task<int> LengthAsync()
        {
            return this.localStorage.LengthAsync();
        }

        public Task RemoveItemAsync<T>(string key)
        {
            return this.localStorage.RemoveItemAsync(key);
        }

        public Task SetItemAsync<T>(string key, T data)
        {
            return this.localStorage.SetItemAsync(key, data);
        }

        public Task<bool> ContainsKeyAsync(string key)
        {
            return this.localStorage.ContainKeyAsync(key);
        }

        private void LocalStorage_OnChanged(object sender, ChangedEventArgs e)
        {
            this.Changed?.Invoke(
                sender,
                new StorageChangedEventArgs
                {
                    OldValue = e.OldValue,
                    Key = e.Key,
                    NewValue = e.NewValue
                });
        }

        private void LocalStorage_OnChanging(object sender, ChangingEventArgs e)
        {
            this.Changing?.Invoke(
                sender,
                new StorageChangingEventArgs
                {
                    OldValue = e.OldValue,
                    Key = e.Key,
                    NewValue = e.NewValue,
                    Cancel = e.Cancel
                });
        }
    }
}