using MediatR;

namespace AccountManager.Application.Templates.Commands.DeleteTemplate
{
    public class DeleteTemplateCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public string Type { get; set; }
    }
}