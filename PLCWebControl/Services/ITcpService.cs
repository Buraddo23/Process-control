using PLCWebControl.Models;

namespace PLCWebControl.Services
{
    public interface ITcpService
    {
        void Connect();
        StateDataModel GetLastData();
        void SendData(CommandDataModel data);
    }
}
