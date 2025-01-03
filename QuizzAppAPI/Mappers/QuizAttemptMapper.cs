using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Mappers;

public class QuizAttemptMapper: Profile
{
    public QuizAttemptMapper()
    {
        CreateMap<QuizAttempt, QuizAttemptDto.QuizAttemptDetailDto>()
            .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz.QuizName));
    }
}