using Autofac.Extras.Moq;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Studi.Proctoring.BackOffice_Api.Tests
{
    public class UserExamSessionRepositoryTests
    {
        [Fact]
        public async Task GetUserExamSessionsByExamSessionIdAsync_Valid()
        {

            using (var mock = AutoMock.GetLoose())
            {


                var userExamSessions = new List<UserExamSession>
            {
                new UserExamSession
                {
                    Id = 1,
                    UserId = 3,
                    SessionExamId = 1
                },
                new UserExamSession
                {
                    Id = 2,
                    UserId = 4,
                    SessionExamId = 1
                },
                new UserExamSession
                {
                    Id = 3,
                    UserId = 2,
                    SessionExamId = 1
                },
                new UserExamSession
                {
                    Id = 4,
                    UserId = 1,
                    SessionExamId = 1
                }
            }.AsQueryable();

                var mockSet = new Mock<DbSet<UserExamSession>>();
                mockSet.As<IDbAsyncEnumerable<UserExamSession>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<UserExamSession>(userExamSessions.GetEnumerator()));
                mockSet.As<IQueryable<UserExamSession>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<UserExamSession>(userExamSessions.Provider));
                mockSet.As<IQueryable<UserExamSession>>().Setup(x => x.Expression).Returns(userExamSessions.Expression);
                mockSet.As<IQueryable<UserExamSession>>().Setup(x => x.ElementType).Returns(userExamSessions.ElementType);
                mockSet.As<IQueryable<UserExamSession>>().Setup(x => x.GetEnumerator()).Returns(userExamSessions.GetEnumerator());

                var context = new Mock<IProctoringContext>();
                context.Setup(x => x.UserExamSessions).Returns(mockSet.Object);
                   
                var mapperMock = new Mock<IMapper>();

                var userExamSessionRepository = new UserExamSessionRepository(context.Object, mapperMock.Object);

                var expected = await GetUserExamSessions();
                var actual = await userExamSessionRepository.GetUserExamSessionsByExamSessionIdAsync(1, false);

                Assert.True(actual != null);
                Assert.Equal(expected.Count(), actual.Count());
            }

            //throw new NotImplementedException();
        }

        [Fact]
        public async Task GetObsoleteUserExamSessionsToDeleteAsync_Valid()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IUserExamSessionRepository>()
                    .Setup(x => x.GetObsoleteUserExamSessionsToDeleteAsync())
                    .Returns(GetUserExamSessions());

                var rep = mock.Create<IUserExamSessionRepository>();

                var expected = await GetUserExamSessions();
                var actual = await rep.GetObsoleteUserExamSessionsToDeleteAsync();

                Assert.True(actual != null);
                Assert.Equal(expected.Count(), actual.Count());
            }

            //throw new NotImplementedException();
        }

        private async Task<IEnumerable<UserExamSessionDto>> GetUserExamSessions()
        {
            var userExamSessionDtos = new List<UserExamSessionDto>
            {
                new UserExamSessionDto
                {
                    Id = 1,
                    SessionExamId = 1
                },
                new UserExamSessionDto
                {
                    Id = 2,
                    SessionExamId = 1
                },
                new UserExamSessionDto
                {
                    Id = 3,
                    SessionExamId = 1
                },
                new UserExamSessionDto
                {
                    Id = 4,
                    SessionExamId = 1
                }
            };

            return userExamSessionDtos;
        }
    }
}
