using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.BackOffice_Api.Services;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Studi.Proctoring.BackOffice_Api.Tests
{
    public class UserExamSessionServiceTest
    {
        private readonly UserExamSessionService _ser;
        private readonly Mock<IServiceProvider> _serviceProvider = new Mock<IServiceProvider>();

        public UserExamSessionServiceTest()
        {
            _ser = new UserExamSessionService(_serviceProvider.Object);
        }

        [Fact]
        public async Task GetUserExamDetailsByUserExamSessionIdAsync_Valid()
        {
            var userExamSessionId = 1;

            var expected = new UserExamDetailInfosDto
            {
                Id = 1,
                UserEmail = "alex"
            };

            _serviceProvider.Setup(x => x.UserExamSessionService().GetUserExamDetailsByUserExamSessionIdAsync(userExamSessionId))
                .ReturnsAsync(expected);

            var actual = await _ser.GetUserExamDetailsByUserExamSessionIdAsync(userExamSessionId);
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 5, 1, "Id")]
        public void GetUserExamSessionsFilteredPaginatedAsync_Valid(int examSessionId, int PageSize, int PageIndex, string SearchForString)
        {
            // Mocks
            //IServiceProvider serviceProvider;
            var proctoringContextMock = new Mock<IProctoringContext>();
            var mapperMock = new Mock<IMapper>();
            var userExamSessionRepository = new UserExamSessionRepository(proctoringContextMock.Object, mapperMock.Object);

            var userExamSession = new UserExamSession
            {
                Id = 1,
                SessionExamId = 1,
                UserId = 1,
                User = new User
                {
                    FirstName = "1",
                    LastName = "2",
                    Email = "3"
                },
                data_ExamStatu = new data_ExamStatu
                {
                    Code = "1"
                },
                HasUserConnectionBeenTested = true,
                HasUserIdentityDocBeenProvided = true,
                ConnectionQuality = 1,
                HasMicrophone = true,
                HasUserPictureBeenProvided = true,
                HasWebcam = true
            };

            var examSession = new ExamSessionDto
            {
                Id = 1,
                RessourceName = "test"
            };

            var user = new User
            {
                Id = 1,
            };

            List<UserExamSession> userExamSessions = new List<UserExamSession>();
            userExamSessions.Add(userExamSession);

            // Setup
            proctoringContextMock.Setup(context => context.UserExamSessions).Returns(GetQueryableMockDbSet(userExamSessions));

            //userExamSessionRepository.Setup(ues => ues.GetCountUserExamSessionsFilteredAsync(It.IsAny<int>(), It.IsAny<string>()))
            //    .ReturnsAsync(7);
            //userExamSessionRepository.Setup(ues => ues.GetUserExamSessionsFilteredPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SortDirection>()))
            //    ;
            //examSessionService.Setup(es => es.GetExamSessionByIdAsync(It.IsAny<int>()))
            //    .ReturnsAsync(examSession);

            // init 
            //UserExamSessionRepository userExamSessionRepository = new UserExamSessionRepository(proctoringContextMocking.Object, mapperMocking.Object);


            // ACT
            var result =Task.Run(async()=>await userExamSessionRepository.GetUserExamSessionsFilteredPaginatedAsync(examSessionId, PageSize, PageIndex));
            Assert.NotNull(result.Result);
            
            // ASSER

            //Assert.True(result == awaitedOutput);
            //result.Should().Be(awaitedOutput);
        }

        private static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }

        private readonly Expression<Func<UserExamSession, bool>> IsntDeleted = (userExamSession) => (userExamSession.DateDelete == null || userExamSession.DateDelete > DateTime.Now);
    }
}
