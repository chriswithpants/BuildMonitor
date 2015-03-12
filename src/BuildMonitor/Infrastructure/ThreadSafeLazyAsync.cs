using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildMonitor.Infrastructure
{
    public class ThreadSafeLazyAsync<T>
    {
        private readonly Func<Task<T>> _initialize;
        private readonly SemaphoreSlim _initializeSemaphore = new SemaphoreSlim(1, 1);
        private T _value;
        private bool _hasValue;

        public ThreadSafeLazyAsync(Func<Task<T>> initialize)
        {
            _initialize = initialize;
        }

        public async Task<T> GetValue()
        {
            if (_hasValue) return _value;

            await _initializeSemaphore.WaitAsync();
            try
            {
                if (_hasValue) return _value;

                _value = await Task.Run(async () => await _initialize());
                _hasValue = true;

                return _value;
            }
            finally
            {
                _initializeSemaphore.Release();
            }
        }

        public async Task Refresh()
        {
            await _initializeSemaphore.WaitAsync();
            try
            {
                _value = await Task.Run(async () => await _initialize());
                _hasValue = true;
            }
            finally
            {
                _initializeSemaphore.Release();
            }
        }
    }
}