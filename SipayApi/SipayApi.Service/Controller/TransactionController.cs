using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SipayApi.Base;
using SipayApi.Data.Domain;
using SipayApi.Data.Repository;
using SipayApi.Schema;
using System.Linq.Expressions;

namespace SipayApi.Service;



[ApiController]
[Route("sipy/api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository repository;
    private readonly IMapper mapper;
    public TransactionController(ITransactionRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }



    [HttpGet("GetByParameter")]
    public ApiResponse<List<TransactionResponse>> GetByParameter(
     int AccountNumber,
     string ReferenceNumber,
     decimal? MinAmountCredit,
     decimal? MaxAmountCredit,
     decimal? MinAmountDebit,
     decimal? MaxAmountDebit,
     string Description,
     DateTime? BeginDate,
     DateTime? EndDate)
    {

        // İstenilen kriterlerin filtreleneceği bir expression oluşturuyoruz.
        Expression<Func<Transaction, bool>> expression = transaction =>
            (string.IsNullOrEmpty(AccountNumber.ToString()) || transaction.AccountNumber == AccountNumber) &&
            (string.IsNullOrEmpty(ReferenceNumber) || transaction.ReferenceNumber == ReferenceNumber) &&
            (!MinAmountCredit.HasValue || transaction.CreditAmount >= MinAmountCredit) &&
            (!MaxAmountCredit.HasValue || transaction.CreditAmount <= MaxAmountCredit) &&
            (!MinAmountDebit.HasValue || transaction.DebitAmount >= MinAmountDebit) &&
            (!MaxAmountDebit.HasValue || transaction.DebitAmount <= MaxAmountDebit) &&
            (string.IsNullOrEmpty(Description) || transaction.Description == Description) &&
            (!BeginDate.HasValue || transaction.TransactionDate >= BeginDate) &&
            (!EndDate.HasValue || transaction.TransactionDate <= EndDate);

        var entityList = repository.GetbyFilter(expression);
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }
}
