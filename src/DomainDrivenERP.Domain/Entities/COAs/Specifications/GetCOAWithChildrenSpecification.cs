using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public class GetCOAWithChildrenSpecification
{
    public static BaseSpecification<Accounts> GetCOAWithChildrenSpec(string coaId)
    {
        var spec = new BaseSpecification<Accounts>(a => a.HeadCode == coaId);
        spec.AddInclude(c => c.ChildAccounts);
        return spec;
    }
}
