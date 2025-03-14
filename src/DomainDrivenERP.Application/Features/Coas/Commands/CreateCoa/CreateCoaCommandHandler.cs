using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Coas.Commands.CreateCoa;
internal class CreateCoaCommandHandler : ICommandHandler<CreateCoaCommand, Accounts>
{
    private readonly ICoaRepository _coaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCoaCommandHandler(ICoaRepository coaRepository, IUnitOfWork unitOfWork)
    {
        _coaRepository = coaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Accounts>> Handle(CreateCoaCommand request, CancellationToken cancellationToken)
    {
        bool isExist = await _coaRepository.IsCoaExist(request.CoaName, request.CoaParentName);
        if (isExist)
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", $"Accounts with name '{request.CoaName}' and parent name '{request.CoaParentName}' already exists."));
        }

        Accounts? parentCoa = await _coaRepository.GetCoaByName(request.CoaParentName, cancellationToken);
        if (parentCoa is null)
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", $"Parent Accounts with name '{request.CoaParentName}' does not exist."));
        }

        Result<Accounts> coaResult = Accounts.Create(request.CoaName, parentCoa);
        if (coaResult.IsFailure)
        {
            return Result.Failure<Accounts>(coaResult.Error);
        }

        await _coaRepository.CreateCoa(coaResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return coaResult;
    }
}
