using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateContactValidator : AbstractValidator<ContactRequest>
{
	public CreateContactValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.CustomerId).NotEmpty();
		RuleFor(x => x.ContactType).NotEmpty().MaximumLength(10);
		RuleFor(x => x.Information).NotEmpty().MaximumLength(100);
		RuleFor(x => x.IsDefault).NotEmpty();
	}
}