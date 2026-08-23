using AutoMapper;
using Fraud.Core.Common;

namespace Fraud.Service.Mapping
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}