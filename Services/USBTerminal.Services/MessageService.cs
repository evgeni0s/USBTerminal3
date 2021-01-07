using USBTerminal.Services.Interfaces;

namespace USBTerminal.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
