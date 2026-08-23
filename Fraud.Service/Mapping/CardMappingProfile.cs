using AutoMapper;
using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.DTO.Card;

namespace Fraud.Service.Mapping
{
    public class CardMappingProfile : Profile
    {
        public CardMappingProfile()
        {
            CreateMap<Card, CardDto>();
            CreateMap<CreateCardDto, Card>();
            CreateMap<UpdateCardDto, Card>();
        }
    }
}