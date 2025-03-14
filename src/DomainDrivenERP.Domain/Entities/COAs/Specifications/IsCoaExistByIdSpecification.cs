using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public static class IsCoaExistByIdSpecification
{
    public static BaseSpecification<Accounts> IsCoaExistByIdSpec(string coaId)
    {
        var spec = new BaseSpecification<Accounts>(a => a.HeadCode == coaId);
        return spec;
    }
}
