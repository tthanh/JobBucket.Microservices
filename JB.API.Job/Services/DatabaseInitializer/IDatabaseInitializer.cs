using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}