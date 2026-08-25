using AutoMapper;
using Fraud.Core.Entities;
using Fraud.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, MeResponseDto>().ReverseMap();
            CreateMap<User, RegisterResponseDto>().ReverseMap();
            CreateMap<User, RegisterRequestDto>().ReverseMap();
            CreateMap<User, LoginResponseDto>().ReverseMap();
            CreateMap<User, LoginRequestDto>().ReverseMap();
        }
    }
}
