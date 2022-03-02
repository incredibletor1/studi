using Microsoft.EntityFrameworkCore;
using Moq;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace Studi.Proctoring.BackOffice_Api.Tests
{
    public static class DBSetInitializer
    {
        public static Mock<DbSet<T>> GetDbSet<T>(IQueryable<T> TestData) where T : class
        {
            var MockSet = new Mock<DbSet<T>>();
            //MockSet.As<IDbAsyncEnumerable<UserExamSession>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<UserExamSession>(TestData.GetEnumerator()));
            MockSet.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<T>(TestData.Provider));
            MockSet.As<IQueryable<T>>().Setup(x => x.Expression).Returns(TestData.Expression);
            MockSet.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(TestData.ElementType);
            MockSet.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(TestData.GetEnumerator());
            return MockSet;
        }
    }
}
