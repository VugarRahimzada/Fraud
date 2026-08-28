using AutoMapper;
using Fraud.Core.Entities;
using Fraud.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Mapping
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
            CreateMap<Transaction, TransactionResponseDto>().ReverseMap();
        }
    }
}
