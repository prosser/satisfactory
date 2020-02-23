namespace SatisfactoryTools.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IStorageProvider
    {
        event EventHandler<StorageChangedEventArgs> Changed;

        event EventHandler<StorageChangingEventArgs> Changing;

        Task ClearAsync();

        event EventHandler Cleared;

        Task<bool> ContainKeyAsync(string key);

        Task<T> GetItemAsync<T>(string key);

        Task<string> KeyAsync(int index);

        Task<int> LengthAsync();

        Task RemoveItemAsync<T>(string key);

        Task SetItemAsync<T>(string key, T data);
    }
}