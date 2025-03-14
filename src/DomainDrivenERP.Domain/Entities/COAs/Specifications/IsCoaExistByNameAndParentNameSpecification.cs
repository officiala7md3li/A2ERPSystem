using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public static class IsCoaExistByNameAndParentNameSpecification
{
    public static BaseSpecification<Accounts> IsCoaExistByNameAndParentNameSpec(string coaName, string coaParentName)
    {
        var spec = new BaseSpecification<Accounts>(coa => coa.HeadName == coaName && coa.ParentAccount.HeadName == coaParentName);
        spec.AddInclude(coa => coa.ParentAccount);
        return spec;
    }
}
