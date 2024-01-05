using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Schema;

namespace VbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountTransactionsController : ControllerBase
{
	private readonly IMediator mediator;
	public AccountTransactionsController(IMediator mediator)
	{
		this.mediator = mediator;
	}

	[HttpGet]
	public async Task<ApiResponse<List<AccountTransactionResponse>>> Get()
	{
		var operation = new GetAllAccountTransactionQuery();
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpGet("{id}")]
	public async Task<ApiResponse<AccountTransactionResponse>> Get(int id)
	{
		var operation = new GetAccountTransactionByIdQuery(id);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpGet("{ReferenceNumber?}/{AccountId?}")]
	public async Task<ApiResponse<List<AccountTransactionResponse>>> GetByParameter(string ReferenceNumber, int AccountId)
	{
		var operation = new GetAccountTransactionByParameterQuery(ReferenceNumber, AccountId);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpPost]
	public async Task<ApiResponse<AccountTransactionResponse>> Post([FromBody] AccountTransactionRequest accountTransaction)
	{
		var operation = new CreateAccountTransactionCommand(accountTransaction);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpPut("{id}")]
	public async Task<ApiResponse> Put(int id, [FromBody] AccountTransactionRequest accountTransaction)
	{
		var operation = new UpdateAccountTransactionCommand(id, accountTransaction);
		var result = await mediator.Send(operation);
		return result;
	}

	[HttpDelete("{id}")]
	public async Task<ApiResponse> Delete(int id)
	{
		var operation = new DeleteAccountTransactionCommand(id);
		var result = await mediator.Send(operation);
		return result;
	}
}