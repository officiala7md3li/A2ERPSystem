using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public class GetCOAWithChildrenSpecification
{
    public static BaseSpecification<Accounts> GetCOAWithChildrenSpec(Guid coaId)
    {
        var spec = new BaseSpecification<Accounts>(a => a.Id == coaId);
        spec.AddInclude(c => c.ChildAccounts);
        return spec;
    }
}
