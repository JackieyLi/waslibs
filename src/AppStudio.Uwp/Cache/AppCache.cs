﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace AppStudio.Uwp.Cache
{
    public static class AppCache
    {
        private static Dictionary<string, string> _memoryCache = new Dictionary<string, string>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method, so nesting generic types is necessary.")]
        public static async Task<CachedContent<T>> GetItemsAsync<T>(string key)
        {
            string json = null;
            if (_memoryCache.ContainsKey(key))
            {
                json = _memoryCache[key];
            }
            else
            {
                json = await UserStorage.ReadTextFromFile(key);
                _memoryCache[key] = json;
            }
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    CachedContent<T> records = JsonConvert.DeserializeObject<CachedContent<T>>(json);
                    return records;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return null;
        }

        public static async Task<T> GetItemAsync<T>(string key)
        {
            string json = null;
            if (_memoryCache.ContainsKey(key))
            {
                json = _memoryCache[key];
            }
            else
            {
                json = await UserStorage.ReadTextFromFile(key);
                _memoryCache[key] = json;
            }
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    T data = JsonConvert.DeserializeObject<T>(json);
                    return data;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return default(T);
        }

        public static async Task<List<T>> GetItemsByPrefixAsync<T>(string prefix)
        {
            List<T> results = new List<T>();

            List<string> keys = _memoryCache.Keys.Where(k => k.StartsWith(prefix)).ToList();
            List<string> inFileKeys = await UserStorage.GetMatchingFilesByPrefix(prefix, keys);

            keys.AddRange(inFileKeys);

            foreach (string key in keys)
            {
                T data = await GetItemAsync<T>(key);
                if (data != null)
                {
                    results.Add(data);
                }
            }


            return results;
        }

        public static async Task AddItemsAsync<T>(string key, CachedContent<T> data)
        {
            await AddItemsAsync<T>(key, data, true);
        }

        public static async Task AddItemsAsync<T>(string key, CachedContent<T> data, bool useStorageCache)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);

                if (useStorageCache)
                {
                    await UserStorage.WriteText(key, json);
                }

                if (_memoryCache.ContainsKey(key))
                {
                    _memoryCache[key] = json;
                }
                else
                {
                    _memoryCache.Add(key, json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        public static async Task AddItemAsync<T>(string key, T data)
        {
            await AddItemAsync<T>(key, data, true);
        }

        public static async Task AddItemAsync<T>(string key, T data, bool useStorageCache)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);

                if (useStorageCache)
                {
                    await UserStorage.WriteText(key, json);
                }

                if (_memoryCache.ContainsKey(key))
                {
                    _memoryCache[key] = json;
                }
                else
                {
                    _memoryCache.Add(key, json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
