using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateEftTransactionValidator : AbstractValidator<EftTransactionRequest>
{
	public CreateEftTransactionValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(50);
		RuleFor(x => x.TransactionDate).NotEmpty();
		RuleFor(x => x.Amount).NotEmpty();
		RuleFor(x => x.SenderAccount).NotEmpty().MaximumLength(50);
		RuleFor(x => x.SenderIban).NotEmpty().MaximumLength(50);
		RuleFor(x => x.SenderName).NotEmpty().MaximumLength(50);
	}
}