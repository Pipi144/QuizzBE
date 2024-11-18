using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Role, opt => 
                opt.MapFrom(src => src.UserRole.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.UserRole, opt 
                => opt.MapFrom(src => Enum.Parse<Role>(src.Role)));
    }
}