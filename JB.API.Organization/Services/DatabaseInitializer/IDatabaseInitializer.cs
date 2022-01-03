using System.Threading.Tasks;

namespace JB.Organization.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}