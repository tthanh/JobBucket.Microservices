using System.Threading.Tasks;

namespace JB.Authentication.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}