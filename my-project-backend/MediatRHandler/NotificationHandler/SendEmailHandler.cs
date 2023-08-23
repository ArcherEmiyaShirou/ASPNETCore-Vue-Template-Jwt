using Backend.Common.Utills;
using Backend.Contract.Entity.DTO;
using Backend.Service.Interface;
using MediatR;

namespace my_project_backend.MediatRHandler.NotificationHandler
{
    public class SendEmailHandler : INotificationHandler<MediatrData>
    {
        private readonly IEmailSerivce _emailSerivce;

        public SendEmailHandler(IEmailSerivce emailSerivce)
        {
            _emailSerivce = emailSerivce;
        }
        public Task Handle(MediatrData notification, CancellationToken cancellationToken)
        {
            if(notification.MessageType is not Const.Event_SendEmail || notification.Data is not Dictionary<string,string> data)
            {
                return Task.CompletedTask;
            }

            _emailSerivce.SendEmailAsync(data);

            return Task.CompletedTask;
        }
    }
}
