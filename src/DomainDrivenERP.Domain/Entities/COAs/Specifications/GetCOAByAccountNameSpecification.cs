using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Shared.Specifications;

namespace DomainDrivenERP.Domain.Entities.COAs.Specifications;
public static class GetCOAByAccountNameSpecification
{
    public static BaseSpecification<Accounts> GetCOAByAccountNameSpec(string headName)
    {
        var spec = new BaseSpecification<Accounts>(a => a.HeadName == headName);
        return spec;
    }
}
