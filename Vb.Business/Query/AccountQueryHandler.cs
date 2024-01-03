using AutoMapper;
using MediatR;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data.Entity;
using Vb.Data;
using Vb.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vb.Business.Query;

public class AccountQueryHandler :
	IRequestHandler<GetAllAccountQuery, ApiResponse<List<AccountResponse>>>,
	IRequestHandler<GetAccountByIdQuery, ApiResponse<AccountResponse>>,
	IRequestHandler<GetAccountByParameterQuery, ApiResponse<List<AccountResponse>>>
{
	private readonly VbDbContext dbContext;
	private readonly IMapper mapper;

	public AccountQueryHandler(VbDbContext dbContext, IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAllAccountQuery request,
		CancellationToken cancellationToken)
	{
		var list = await dbContext.Set<Account>()
			.Include(x => x.AccountTransactions)
			.Include(x => x.EftTransactions).Where(x=>x.IsActive==true).ToListAsync(cancellationToken);

		var mappedList = mapper.Map<List<Account>, List<AccountResponse>>(list);
		return new ApiResponse<List<AccountResponse>>(mappedList);
	}

	public async Task<ApiResponse<AccountResponse>> Handle(GetAccountByIdQuery request,
		CancellationToken cancellationToken)
	{
		var entity = await dbContext.Set<Account>()
			.Include(x => x.AccountTransactions)
			.Include(x => x.EftTransactions)
			.Where(x => x.IsActive == true)
			.FirstOrDefaultAsync(x => x.AccountNumber == request.Id, cancellationToken);

		if (entity == null)
		{
			return new ApiResponse<AccountResponse>("Account is not found");
		}

		var mapped = mapper.Map<Account, AccountResponse>(entity);
		return new ApiResponse<AccountResponse>(mapped);
	}

	public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAccountByParameterQuery request,
		CancellationToken cancellationToken)
	{
		var list = await dbContext.Set<Account>()
			.Include(x => x.AccountTransactions)
			.Include(x => x.EftTransactions)
			.Where(x => x.IsActive == true && (
			x.IBAN.ToUpper().Contains(request.IBAN.ToUpper()) ||
			x.Name.ToUpper().Contains(request.Name.ToUpper()) ||
			(x.Customer.FirstName+" "+x.Customer.LastName).ToUpper().Contains(request.CustomerName.ToUpper()))

		).ToListAsync(cancellationToken);

		var mappedList = mapper.Map<List<Account>, List<AccountResponse>>(list);
		return new ApiResponse<List<AccountResponse>>(mappedList);
	}
}