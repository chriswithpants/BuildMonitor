using System.Threading.Tasks;
using BuildMonitor.Infrastructure;

namespace BuildMonitor.Api.Monitor
{
    public interface IBuildStatusLookup : ILookupService
    {
        Task<BuildStatusModel> Get(string id);
        Task<BuildStatusModel> TryGet(string id);
        Task<BuildStatusModel[]> GetBuildStatuses();
        Task AddOrUpdate(BuildStatusModel model);
    }
}