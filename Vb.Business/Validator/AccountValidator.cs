using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountValidator : AbstractValidator<AccountRequest>
{
	public CreateAccountValidator()
	{
		RuleFor(x => x.IBAN).NotEmpty().MaximumLength(34);
		RuleFor(x => x.Balance).NotEmpty();
		RuleFor(x => x.CurrencyType).NotEmpty().MaximumLength(3).WithName("Currency Type");
		RuleFor(x => x.Name).MaximumLength(100);

	}
}