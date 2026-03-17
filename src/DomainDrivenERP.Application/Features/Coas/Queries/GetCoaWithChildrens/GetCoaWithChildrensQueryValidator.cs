using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DomainDrivenERP.Application.Features.Coas.Queries.GetCoaWithChildrens;
public class GetCoaWithChildrensQueryValidator : AbstractValidator<GetCoaWithChildrensQuery>
{
    public GetCoaWithChildrensQueryValidator()
    {
        RuleFor(a => a.CoaId).NotEmpty().WithMessage("Account Id can't be Empty");
    }
}
