using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildMonitor.Infrastructure;

namespace BuildMonitor.Api.Monitor
{
    public class BuildStatusLookup : IBuildStatusLookup
    {
        private readonly ThreadSafeLazyAsync<Dictionary<string, BuildStatusModel>> _buildStatuses;
        private readonly SemaphoreSlim _initialFetchSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IMonitorBroadcaster _broadcaster;

        public BuildStatusLookup(IMonitorBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;

            _buildStatuses = new ThreadSafeLazyAsync<Dictionary<string, BuildStatusModel>>(FetchBuildStatuses);
        }

        private async Task<Dictionary<string, BuildStatusModel>> FetchBuildStatuses()
        {
            await _initialFetchSemaphore.WaitAsync();
            try
            {
                return new Dictionary<string, BuildStatusModel>();
            }
            finally
            {
                _initialFetchSemaphore.Release();
            }
        }

        public async Task<BuildStatusModel> Get(string id)
        {
            var teams = await GetBuildStatuses();
            return teams.First(t => t.Id == id);
        }

        public async Task<BuildStatusModel> TryGet(string id)
        {
            var teams = await GetBuildStatuses();
            return teams.FirstOrDefault(t => t.Id == id);
        }

        public async Task<BuildStatusModel[]> GetBuildStatuses()
        {
            var result = await _buildStatuses.GetValue();
            return result.Values.ToArray();
        }

        public async Task AddOrUpdate(BuildStatusModel model)
        {
            await _semaphore.WaitAsync();
            try
            {
                var buildStatuses = await _buildStatuses.GetValue();

                if (buildStatuses.ContainsKey(model.Id))
                {
                    buildStatuses[model.Id] = model;
                }
                else
                {
                    buildStatuses.Add(model.Id, model);
                }

                await _broadcaster.BuildStatusUpdated(model);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
 