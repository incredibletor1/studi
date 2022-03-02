using AutoMapper;
using IO.Swagger.Model;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Models.VM;
using Studi.Proctoring.Database.Context;
using System;

namespace Studi.Api.Proctoring.Helpers
{
    public class DtoAndEntityConversionProfile : Profile
    {
        private readonly IMapper _mapper = null;

        public DtoAndEntityConversionProfile()
        {
            // Create mapping for all entities, DTOs and VMs
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<ProctoringGetUserInfosViewModel, UserDto>();
            CreateMap<UserDto, UserVM>();

            CreateMap<ExamSession, ExamSessionView>();
            CreateMap<ExamSession, ExamSessionDto>();
            CreateMap<ExamSessionDto, ExamSession>();
            CreateMap<ProctoringGetBlockExamInfosViewModel, ExamSessionDto>();            

            CreateMap<UserExamSession, UserExamSessionDto>();
            CreateMap<UserExamSessionDto, UserExamSession>();
            CreateMap<ProctoringGetUserBlockExamInfosViewModel, UserExamSessionDto>();
            CreateMap<UserExamMaterialCheckVM, UserExamMaterialCheckDto>();

            CreateMap<ProctoringImage, ProctoringImageDto>();
            CreateMap<ProctoringImageDto, ProctoringImage>();

            CreateMap<UserImageCheck, UserImageCheckDto>();
            CreateMap<UserImageCheckDto, UserImageCheck>();

            CreateMap<GlobalSetting, GlobalSettingDto>();
            CreateMap<GlobalSettingDto, GlobalSetting>();
        }
    }

    public static class ConversionExtension
    {
        private static IMapper _mapper = null;

        public static void InitMapper(IMapper mapper)
        {
            if (_mapper is null)
                _mapper = mapper;
        }

        public static User ToEntity(this UserDto userDto)
        {
            if (userDto is null)
                return null;
            else
                return _mapper.Map<User>(userDto);
        }

        public static UserDto ToDto(this User userEntity)
        {
            if (userEntity is null)
                return null;
            else
                return _mapper.Map<UserDto>(userEntity);
        }

        public static UserDto ToDto(this ProctoringGetUserInfosViewModel userInfosVM)
        {
            if (userInfosVM is null)
                return null;
            else
                return _mapper.Map<UserDto>(userInfosVM);
        }

        public static ExamSession ToEntity(this ExamSessionDto examSessionDto)
        {
            if (examSessionDto is null)
                return null;
            else
                return _mapper.Map<ExamSession>(examSessionDto);
        }

        public static ExamSessionDto ToDto(this ExamSession examSessionEntity)
        {
            if (examSessionEntity is null)
                return null;
            else
                return _mapper.Map<ExamSessionDto>(examSessionEntity);
        }

        public static ExamSessionDto ToDto(this ProctoringGetBlockExamInfosViewModel examSessionVM)
        {
            if (examSessionVM is null)
                return null;
            else
                return _mapper.Map<ExamSessionDto>(examSessionVM);
        }

        public static UserExamSession ToEntity(this UserExamSessionDto userExamSessionDto)
        {
            if (userExamSessionDto is null)
                return null;
            else
                return _mapper.Map<UserExamSession>(userExamSessionDto);
        }

        public static UserExamSessionDto ToDto(this UserExamSession userExamSessionEntity)
        {
            if (userExamSessionEntity is null)
                return null;
            else
                return _mapper.Map<UserExamSessionDto>(userExamSessionEntity);
        }

        public static UserExamSessionDto ToDto(this ProctoringGetUserBlockExamInfosViewModel userExamSessionVM)
        {
            if (userExamSessionVM is null)
                return null;
            else
            {
                var userExamSessionDto = _mapper.Map<UserExamSessionDto>(userExamSessionVM);

                // Calculate exam's actual duration
                if (userExamSessionDto.ActualStartTime != null && userExamSessionDto.ActualEndTime != null)
                    userExamSessionDto.ExamActualDuration = Convert.ToInt32(
                        Math.Truncate((userExamSessionDto.ActualEndTime.Value - userExamSessionDto.ActualStartTime.Value).TotalMinutes));

                return userExamSessionDto;
            }
        }

        public static UserExamMaterialCheckDto ToDto(this UserExamMaterialCheckVM userExamMaterialCheckVM)
        {
            if (userExamMaterialCheckVM is null)
                return null;
            else
            {
                var userExamMaterialCheckDto = _mapper.Map<UserExamMaterialCheckDto>(userExamMaterialCheckVM);
                userExamMaterialCheckDto.IdentityDocumentType = (int?)userExamMaterialCheckVM.IdentityDocumentType;
                return userExamMaterialCheckDto;
            }
        }

        public static ProctoringImage ToEntity(this ProctoringImageDto proctoringImageDto)
        {
            if (proctoringImageDto is null)
                return null;
            else
                return _mapper.Map<ProctoringImage>(proctoringImageDto);
        }

        public static ProctoringImageDto ToDto(this ProctoringImage proctoringImageEntity)
        {
            if (proctoringImageEntity is null)
                return null;
            else
                return _mapper.Map<ProctoringImageDto>(proctoringImageEntity);
        }

        public static UserImageCheck ToEntity(this UserImageCheckDto userImageCheckDto)
        {
            if (userImageCheckDto is null)
                return null;
            else
                return _mapper.Map<UserImageCheck>(userImageCheckDto);
        }

        public static GlobalSettingDto ToDto(this GlobalSetting globalSettingEntity)
        {
            if (globalSettingEntity is null)
                return null;
            else
                return _mapper.Map<GlobalSettingDto>(globalSettingEntity);
        }
    }
}
