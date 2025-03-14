using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.Coas.Commands.CreateFirstLevelCoa;
public class CreateFirstLevelCoaCommand : ICommand<Accounts>
{
    public CreateFirstLevelCoaCommand(string headName, ChartOfAccountsType type)
    {
        HeadName = headName;
        Type = type;
    }

    public string HeadName { get; }
    public ChartOfAccountsType Type { get; }
}
