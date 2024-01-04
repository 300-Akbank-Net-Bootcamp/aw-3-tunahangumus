using AutoMapper;
using MediatR;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data.Entity;
using Vb.Data;
using Vb.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vb.Business.Query;

public class EftTransactionQueryHandler :
	IRequestHandler<GetAllEftTransactionQuery, ApiResponse<List<EftTransactionResponse>>>,
	IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>,
	IRequestHandler<GetEftTransactionByParameterQuery, ApiResponse<List<EftTransactionResponse>>>
{
	private readonly VbDbContext dbContext;
	private readonly IMapper mapper;

	public EftTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionQuery request,
		CancellationToken cancellationToken)
	{
		var list = await dbContext.Set<EftTransaction>()
			.Include(x => x.Account)
			.Where(x => x.IsActive == true)
			.ToListAsync(cancellationToken);

		var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
		return new ApiResponse<List<EftTransactionResponse>>(mappedList);
	}

	public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
		CancellationToken cancellationToken)
	{
		var entity = await dbContext.Set<EftTransaction>()
			.Include(x => x.Account)
			.Where(x => x.IsActive == true)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		if (entity == null)
		{
			return new ApiResponse<EftTransactionResponse>("Record not found");
		}

		var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entity);
		return new ApiResponse<EftTransactionResponse>(mapped);
	}

	public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetEftTransactionByParameterQuery request,
		CancellationToken cancellationToken)
	{
		//CONDITION IS EITHER THE DATE AND THE IBAN SHOULD MATCH OR  AMOUNT AND SENDER IBAN SHOULD MATCH
		var list = await dbContext.Set<EftTransaction>()
			.Include(x => x.Account)
			.Where(x => x.IsActive == true && (
			(x.SenderIban.ToUpper().Contains(request.senderIban) && x.Amount == request.amount) ||
			(x.SenderIban.ToUpper().Contains(request.senderIban) && x.TransactionDate.ToShortDateString() == request.date.ToShortDateString())
			)
		).ToListAsync(cancellationToken);

		var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
		return new ApiResponse<List<EftTransactionResponse>>(mappedList);
	}
}