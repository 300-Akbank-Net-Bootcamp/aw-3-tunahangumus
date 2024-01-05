using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountTransactionValidator : AbstractValidator<AccountTransactionRequest>
{
	public CreateAccountTransactionValidator()
	{
		RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(50);
		RuleFor(x => x.TransactionDate).NotEmpty();
		RuleFor(x => x.Amount).NotEmpty();
		RuleFor(x => x.Description).MaximumLength(300);
		RuleFor(x => x.TransferType).NotEmpty().MaximumLength(10);

	}
}