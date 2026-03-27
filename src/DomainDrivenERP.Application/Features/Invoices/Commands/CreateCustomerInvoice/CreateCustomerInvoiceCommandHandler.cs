using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;

internal sealed class CreateCustomerInvoiceCommandHandler
    : ICommandHandler<CreateCustomerInvoiceCommand, CreateCustomerInvoiceResult>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly ICustomerRespository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerInvoiceCommandHandler(
        ICustomerInvoiceRepository invoiceRepository,
        ICustomerRespository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCustomerInvoiceResult>> Handle(
        CreateCustomerInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId.ToString());
        if (customer is null)
            return Result.Failure<CreateCustomerInvoiceResult>(
                new Error("CustomerInvoice.CustomerNotFound", $"Customer '{request.CustomerId}' not found."));

        // 2. Create invoice via Domain Factory
        var result = CustomerInvoice.Create(
            request.CustomerId,
            request.CompanyId,
            request.CurrencyId,
            request.InvoiceDate,
            request.TaxOrderSetting,
            request.StackingMode,
            request.Notes);

        if (result.IsFailure)
            return Result.Failure<CreateCustomerInvoiceResult>(result.Error);

        var invoice = result.Value;

        // 3. Persist
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCustomerInvoiceResult(
            invoice.Id,
            invoice.CustomerId,
            invoice.InvoiceDate,
            invoice.Status.ToString()));
    }
}
