﻿using System.Threading.Tasks;

namespace Insurance.IApiProviders.Providers
{
    /// <summary>
    /// Define generic contract of the external Insurance providers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TCollection"></typeparam>
    public interface IApiProvider<T, TCollection>
    {
        Task<TCollection> GetAllAsync();
        Task<T> GetAsync(string id);
        Task AddAsync(T t);
    }
}