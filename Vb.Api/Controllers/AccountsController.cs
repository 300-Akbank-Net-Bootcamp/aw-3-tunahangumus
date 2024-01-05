using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Schema;

namespace VbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
	private readonly IMediator mediator;
	public AccountsController(IMediator mediator)
	{
		this.mediator = mediator;
	}

	[HttpGet]
	public async Task<ApiResponse<List<AccountResponse>>> Get()
	{
		var operation = new GetAllAccountQuery();
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpGet("{id}")]
	public async Task<ApiResponse<AccountResponse>> Get(int id)
	{
		var operation = new GetAccountByIdQuery(id);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpGet("{IBAN?}/{Name?}/{CustomerName?}")]
	public async Task<ApiResponse<List<AccountResponse>>> GetByParameter(string IBAN,string Name,string CustomerName)
	{
		var operation = new GetAccountByParameterQuery(IBAN,Name,CustomerName);
		var result = await mediator.Send(operation);
		return result;
	}


	[HttpPost]
	public async Task<ApiResponse<AccountResponse>> Post([FromBody] AccountRequest Account)
	{
		var operation = new CreateAccountCommand(Account);
		var result = await mediator.Send(operation);
		return result;
	}
	[HttpPut("{id}")]
	public async Task<ApiResponse> Put(int id, [FromBody] AccountRequest Account)
	{
		var operation = new UpdateAccountCommand(id, Account);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpDelete("{id}")]
	public async Task<ApiResponse> Delete(int id)
	{
		var operation = new DeleteAccountCommand(id);
		var result = await mediator.Send(operation);
		return result;
	}

}