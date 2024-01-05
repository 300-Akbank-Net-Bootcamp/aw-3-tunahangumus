using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data.Entity;
using Vb.Data;
using Vb.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vb.Business.Command;


public class AccountTransactionCommandHandler :
	IRequestHandler<CreateAccountTransactionCommand, ApiResponse<AccountTransactionResponse>>,
	IRequestHandler<UpdateAccountTransactionCommand, ApiResponse>,
	IRequestHandler<DeleteAccountTransactionCommand, ApiResponse>

{
	private readonly VbDbContext dbContext;
	private readonly IMapper mapper;

	public AccountTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<ApiResponse<AccountTransactionResponse>> Handle(CreateAccountTransactionCommand request, CancellationToken cancellationToken)
	{
		var checkIdentity = await dbContext.Set<AccountTransaction>().Where(x => x.AccountId == request.Model.AccountId)
			.FirstOrDefaultAsync(cancellationToken);
		if (checkIdentity != null)
		{
			return new ApiResponse<AccountTransactionResponse>($"{request.Model.AccountId} is used by another Account Transaction proccess.");
		}

		var entity = mapper.Map<AccountTransactionRequest, AccountTransaction>(request.Model);
		entity.InsertDate = DateTime.UtcNow;
		var entityResult = await dbContext.AddAsync(entity, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		var mapped = mapper.Map<AccountTransaction, AccountTransactionResponse>(entityResult.Entity);
		return new ApiResponse<AccountTransactionResponse>(mapped);
	}

	public async Task<ApiResponse> Handle(UpdateAccountTransactionCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.AccountId == request.Id)
			.FirstOrDefaultAsync(cancellationToken);
		if (fromdb == null)
		{
			return new ApiResponse("Record not found");
		}
		fromdb.UpdateDate = DateTime.UtcNow;
		fromdb.Amount = request.Model.Amount;
		fromdb.TransferType = request.Model.TransferType;

		await dbContext.SaveChangesAsync(cancellationToken);
		return new ApiResponse();
	}

	public async Task<ApiResponse> Handle(DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.AccountId == request.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (fromdb == null)
		{
			return new ApiResponse("Record not found");
		}

		fromdb.IsActive = false;
		await dbContext.SaveChangesAsync(cancellationToken);
		return new ApiResponse();
	}
}