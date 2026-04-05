using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Commands.CreateUnitOfMeasure;

public record CreateUnitOfMeasureCommand(
    string Code,
    string NameEn,
    string NameAr,
    UomType Type,
    decimal ConversionFactor = 1,
    Guid? BaseUomId = null) : ICommand<UnitOfMeasure>;
