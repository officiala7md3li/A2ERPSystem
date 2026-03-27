using FluentValidation;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.CreateDebitNote;

internal sealed class CreateDebitNoteCommandValidator : AbstractValidator<CreateDebitNoteCommand>
{
    public CreateDebitNoteCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.NoteDate).NotEmpty().LessThanOrEqualTo(System.DateTime.UtcNow.AddDays(1));
    }
}
