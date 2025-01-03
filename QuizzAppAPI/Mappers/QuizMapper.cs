using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Mappers;

public class QuizMapper : Profile
{
    public QuizMapper()
    {
        // Map Quiz -> QuizBasicDto
        CreateMap<Quiz, QuizBasicDto>()
            .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Id)) // Map Id to QuizId
            .ForMember(dest => dest.NumberOfQuestions, opt => opt.MapFrom(src => src.QuizQuestions.Count));

        // Map Quiz -> QuizDetailDto
        CreateMap<Quiz, QuizDetailDto>()
            .IncludeBase<Quiz, QuizBasicDto>() // Inherit basic mapping
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.QuizQuestions.Select(qq => qq.Question)));
        
        // Map Quiz -> QuizWithFullQuestionsDto
        CreateMap<Quiz, QuizWithFullQuestionsDto>()
            .IncludeBase<Quiz, QuizBasicDto>() // Inherit basic mapping
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.QuizQuestions.Select(qq => qq.Question)))
            .AfterMap((src, dest, context) =>
            {
                // Map each Question entity to QuestionDetailDto
                dest.Questions = src.QuizQuestions
                    .Select(qq => context.Mapper.Map<QuestionDetailDto>(qq.Question))
                    .ToList();
            });
        
        // Map AddQuizDataDto -> Quiz
        CreateMap<AddQuizDataDto, Quiz>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id as it will be generated
            .ForMember(dest => dest.QuizQuestions, opt => opt.Ignore()) // Handled explicitly in service/repository
            .ForMember(dest => dest.QuizAttempts, opt => opt.Ignore()); // Initialize empty collection

        // Map UpdateQuizDataDto -> Quiz
        CreateMap<UpdateQuizDataDto, Quiz>()
            .ForMember(dest => dest.QuizQuestions, opt => opt.Ignore()) // Handled explicitly in service/repository
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for updates
            .ForMember(dest => dest.QuizAttempts, opt => opt.Ignore()); // Preserve existing attempts
    }
}