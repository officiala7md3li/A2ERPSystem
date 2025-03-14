using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public class GetCOAChildsSpecification
{
    public static BaseSpecification<Accounts> GetCOAChildsSpec(string parentCoaId)
    {
        var spec = new BaseSpecification<Accounts>(a => a.ParentHeadCode == parentCoaId);
        return spec;
    }
}
