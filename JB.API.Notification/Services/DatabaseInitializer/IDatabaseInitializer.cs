using System.Threading.Tasks;

namespace JB.Notification.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}