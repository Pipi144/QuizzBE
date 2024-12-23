using AutoMapper;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Mappers;

public class QuestionMapper : Profile
{
    public QuestionMapper()
    {
        // Map from Question entity to QuestionBasicDto
        CreateMap<Question, QuestionBasicDto>();

        // Map from QuestionOption entity to QuestionOptionBasicDto
        CreateMap<QuestionOption, QuestionOptionBasicDto>();

        // Map from Question entity to QuestionDetailDto, including its options
        CreateMap<Question, QuestionDetailDto>()
            .ForMember(dest => dest.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions));

        // Map from AddQuestionDataDto to Question for creation
        CreateMap<AddQuestionDataDto, Question>()
            .ForMember(dest => dest.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions));

        CreateMap<AddQuestionOptionDataDto, QuestionOption>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        
        // Map UpdateQuestionOptionDataDto -> QuestionOption
        CreateMap<UpdateQuestionOptionDataDto, QuestionOption>();

        // Map from QuestionOption to QuestionOptionDto
        CreateMap<QuestionOption, QuestionOptionDto>();

        // Map from QuestionOptionDto to QuestionOption for updates
        CreateMap<QuestionOptionDto, QuestionOption>();
    }
}