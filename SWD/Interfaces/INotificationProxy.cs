namespace SWD.Interfaces
{
    public interface INotificationProxy
    {
        Task SendOutboundNotice(string to, string subject, string body);
    }
}
