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


public class AccountCommandHandler :
	IRequestHandler<CreateAccountCommand, ApiResponse<AccountResponse>>,
	IRequestHandler<UpdateAccountCommand, ApiResponse>,
	IRequestHandler<DeleteAccountCommand, ApiResponse>

{
	private readonly VbDbContext dbContext;
	private readonly IMapper mapper;

	public AccountCommandHandler(VbDbContext dbContext, IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
	{

		var entity = mapper.Map<AccountRequest, Account>(request.Model);
		while (true)
		{
			entity.AccountNumber = new Random().Next(1000000, 9999999);

			var checkAccount = await dbContext.Set<Account>().Where(x => x.AccountNumber == entity.AccountNumber)
				.FirstOrDefaultAsync(cancellationToken);
			if (checkAccount != null)
			{
				continue;
			}
			else
			{
				entity.InsertDate = DateTime.UtcNow;
				var entityResult = await dbContext.AddAsync(entity, cancellationToken);
				await dbContext.SaveChangesAsync(cancellationToken);
				var mapped = mapper.Map<Account, AccountResponse>(entityResult.Entity);
				return new ApiResponse<AccountResponse>(mapped);
			}
		}
		
	}

	public async Task<ApiResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id)
			.FirstOrDefaultAsync(cancellationToken);
		if (fromdb == null)
		{
			return new ApiResponse("Record not found");
		}

		fromdb.Balance = request.Model.Balance;
		fromdb.CurrencyType = request.Model.CurrencyType;
		fromdb.Name = request.Model.Name;
		fromdb.UpdateDate = DateTime.UtcNow;

		await dbContext.SaveChangesAsync(cancellationToken);
		return new ApiResponse();
	}

	public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
	{
		var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id)
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