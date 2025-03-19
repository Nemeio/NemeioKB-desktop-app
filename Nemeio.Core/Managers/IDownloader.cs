using System.Threading.Tasks;

namespace Nemeio.Core.Managers
{
    public interface IDownloader
    {
        string LastError  { get; }
        
        Task<bool> Download();
    }
}
