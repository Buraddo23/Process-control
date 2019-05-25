using PLCWebControl.Models;
using System.Threading.Tasks;

namespace PLCWebControl.Services
{
    interface ITcpService
    {
        void Connect();
        StateDataModel GetLastData();
        void SendData(CommandDataModel data);
    }
}
