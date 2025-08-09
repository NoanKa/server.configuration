using static Server.Configuration.Communicator;

namespace Server.Configuration
{
    public interface ICommunicator
    {
        IClient Builder();

    }
}
