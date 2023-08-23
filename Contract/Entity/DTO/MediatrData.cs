using MediatR;

namespace Backend.Contract.Entity.DTO
{
    public class MediatrData : INotification
    {
        public MediatrData(string messageType, object data)
        {
            MessageType = messageType;
            Data = data;
        }

        public string MessageType { get; set; } = string.Empty;
        public object Data { get; set; } = default!;
    }
}
