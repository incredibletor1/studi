using AutoMapper;
using IO.Swagger.Model;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Models.VM;
using Studi.Proctoring.Database.Context;
using System;

namespace Studi.Proctoring.BackOffice_Api.Helpers
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
            CreateMap<ExamSessionDto, ExamSessionVM>();
            CreateMap<ExamSessionView, ExamSessionVM>();


            CreateMap<UserExamSession, UserExamSessionDto>();
            CreateMap<UserExamSessionDto, UserExamSession>();
            CreateMap<ProctoringGetUserBlockExamInfosViewModel, UserExamSessionDto>();
            CreateMap<UserExamGeneralInfosDto, UserExamGeneralInfosVM>();

            CreateMap<ProctoringImage, ProctoringImageDto>();
            CreateMap<ProctoringImageDto, ProctoringImage>();

            CreateMap<UserImageCheck, UserImageCheckDto>();
            CreateMap<UserImageCheckDto, UserImageCheck>();

            CreateMap<ProctoringAdminUser, AdminUserDTO>();
            CreateMap<AdminUserDTO, ProctoringAdminUser>();
            CreateMap<AdminUserDTO, AdminUserVM>();

            CreateMap<GlobalSetting, GlobalSettingDto>();
            CreateMap<GlobalSettingDto, GlobalSetting>();
            CreateMap<GlobalSettingDto, GlobalSettingVM>();
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

        public static UserVM ToVM(this UserDto userDto)
        {
            if (userDto is null)
                return null;
            else
                return _mapper.Map<UserVM>(userDto);
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

        public static ExamSessionView ToView(this ExamSession examSessionEntity, int usersCount, ExamStatusEnum examStatus)
        {
            if (examSessionEntity is null)
                return null;
            else
            {
                var examSessionView = _mapper.Map<ExamSessionView>(examSessionEntity);
                examSessionView.UsersCount = usersCount;
                examSessionView.ExamStatus = examStatus;
                return examSessionView;
            }
        }

        public static ExamSessionDto ToDto(this ProctoringGetBlockExamInfosViewModel examSessionVM)
        {
            if (examSessionVM is null)
                return null;
            else
                return _mapper.Map<ExamSessionDto>(examSessionVM);
        }

        public static ExamSessionVM ToVM(this ExamSessionDto examSessionDto)
        {
            if (examSessionDto is null)
                return null;
            else
                return _mapper.Map<ExamSessionVM>(examSessionDto);
        }

        public static ExamSessionVM ToVM(this ExamSessionView examSessionView)
        {
            if (examSessionView is null)
                return null;
            else
                return _mapper.Map<ExamSessionVM>(examSessionView);
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

        public static UserExamGeneralInfosVM ToVM(this UserExamGeneralInfosDto userExamGeneralInfosDto)
        {
            if (userExamGeneralInfosDto is null)
                return null;
            else
                return _mapper.Map<UserExamGeneralInfosVM>(userExamGeneralInfosDto);           
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

        public static UserImageCheckDto ToDto(this UserImageCheck userImageCheckEntity)
        {
            if (userImageCheckEntity is null)
                return null;
            else
                return _mapper.Map<UserImageCheckDto>(userImageCheckEntity);
        }

        public static ProctoringAdminUser ToEntity(this AdminUserDTO adminUserDto)
        {
            if (adminUserDto is null)
                return null;
            else
                return _mapper.Map<ProctoringAdminUser>(adminUserDto);
        }

        public static AdminUserDTO ToDto(this ProctoringAdminUser adminUserEntity)
        {
            if (adminUserEntity is null)
                return null;
            else
            {
                var adminUserDto = _mapper.Map<AdminUserDTO>(adminUserEntity);

                return adminUserDto;
            }
        }

        public static AdminUserVM ToVM(this AdminUserDTO adminUserDto)
        {
            if (adminUserDto is null)
                return null;
            else
                return _mapper.Map<AdminUserVM>(adminUserDto);
        }


        public static GlobalSettingDto ToDto(this GlobalSetting globalSettingEntity)
        {
            if (globalSettingEntity is null)
                return null;
            else
                return _mapper.Map<GlobalSettingDto>(globalSettingEntity);
        }

        public static GlobalSettingVM ToVM(this GlobalSettingDto globalSettingDto)
        {
            if (globalSettingDto is null)
                return null;
            else
                return _mapper.Map<GlobalSettingVM>(globalSettingDto);
        }
    }
}
