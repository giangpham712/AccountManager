using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AccountManager.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ValidationException = FluentValidation.ValidationException;

namespace AccountManager.Application.Infrastructure
{
    public class RequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly IHttpContextAccessor _contextAccessor;

        public RequestBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor contextAccessor)
        {
            _validators = validators;
            _contextAccessor = contextAccessor;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var currentUserName = _contextAccessor.HttpContext.User?.Identity.Name;

            var command = request as ICommand;
            if (!currentUserName.IsNullOrWhiteSpace() && command != null) command.User = currentUserName;

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0) throw new ValidationException(failures);

            return next();
        }
    }
}