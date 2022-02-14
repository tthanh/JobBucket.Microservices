using System.Threading.Tasks;

namespace JB.Blog.Services
{
    public interface IDatabaseInitializer
    {
        Task Initialize();
    }
}