using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public class GetCOAChildsSpecification
{
    public static BaseSpecification<Accounts> GetCOAChildsSpec(Guid parentAccountId)
    {
        var spec = new BaseSpecification<Accounts>(a => a.ParentAccountId == parentAccountId);
        return spec;
    }
}
