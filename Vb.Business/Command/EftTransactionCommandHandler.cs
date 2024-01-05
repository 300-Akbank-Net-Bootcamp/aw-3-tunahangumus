using AutoMapper;
using MediatR;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data.Entity;
using Vb.Data;
using Vb.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vb.Business.Command;

public class EftTransactionCommandHandler :
	IRequestHandler<CreateEftTransactionCommand, ApiResponse<EftTransactionResponse>>,
	IRequestHandler<UpdateEftTransactionCommand, ApiResponse>,
	IRequestHandler<DeleteEftTransactionCommand, ApiResponse>

{
	private readonly VbDbContext dbContext;
	private readonly IMapper mapper;

	public EftTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<ApiResponse<EftTransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
	{
		var checkIdentity = await dbContext.Set<EftTransaction>().Where(x => x.Id == request.Model.Id)
			.FirstOrDefaultAsync(cancellationToken);
		if (checkIdentity != null)
		{
			return new ApiResponse<EftTransactionResponse>($"{request.Model.ReferenceNumber} is used by another EftTransaction.");
		}

		var entity = mapper.Map<EftTransactionRequest, EftTransaction>(request.Model);
		entity.InsertDate = DateTime.UtcNow;
		var entityResult = await dbContext.AddAsync(entity, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entityResult.Entity);
		return new ApiResponse<EftTransactionResponse>(mapped);
	}

	public async Task<ApiResponse> Handle(UpdateEftTransactionCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id)
			.FirstOrDefaultAsync(cancellationToken);
		if (fromdb == null)
		{
			return new ApiResponse("Record not found");
		}
		// other parts should not be changed
		fromdb.Description = request.Model.Description;
		fromdb.UpdateDate = DateTime.UtcNow;
		await dbContext.SaveChangesAsync(cancellationToken);
		return new ApiResponse();
	}

	public async Task<ApiResponse> Handle(DeleteEftTransactionCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (fromdb == null)
		{
			return new ApiResponse("Record not found");
		}
		//dbContext.Set<EftTransaction>().Remove(fromdb);

		fromdb.IsActive = false;
		await dbContext.SaveChangesAsync(cancellationToken);
		return new ApiResponse();
	}
}