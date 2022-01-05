using System.Threading.Tasks;

namespace JB.User.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}