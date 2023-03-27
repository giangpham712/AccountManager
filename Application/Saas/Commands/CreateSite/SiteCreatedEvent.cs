using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class SiteCreatedEvent : INotification
    {
        public string Actor { get; set; }
        public Site Site { get; set; }
        public dynamic Command { get; set; }
    }
}