using PLCWebControl.Models;
using System.Threading.Tasks;

namespace PLCWebControl.Services
{
    public interface ITcpService
    {
        void Connect();
        StateDataModel GetLastData();
        void SendData(CommandDataModel data);
    }
}
