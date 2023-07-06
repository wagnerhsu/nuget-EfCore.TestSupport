﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.BookApp.EfCode;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Test.Helpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class TestSqlServerHelpers 
    {
        private readonly ITestOutputHelper _output;

        public TestSqlServerHelpers(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSqlDatabaseEnsureCleanOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<BookContext>();
            using var context = new BookContext(options);
            
            context.Database.EnsureClean();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
        }

        [Fact]
        public void TestEnsureDeletedEnsureCreatedOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<BookContext>();
            using var context = new BookContext(options);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
        }

        [Fact]
        public void TestSqlServerUniqueClassOk()
        {
            //SETUP
            //ATTEMPT
            var options = this.CreateUniqueClassOptions<BookContext>();
            using (var context = new BookContext(options))
            {
                //VERIFY
                var builder = new SqlConnectionStringBuilder(context.Database.GetDbConnection().ConnectionString);
                builder.InitialCatalog.ShouldEndWith(GetType().Name);
            }
        }

        [Fact]
        public void TestSqlServerUniqueMethodOk()
        {
            //SETUP
            //ATTEMPT
            var options = this.CreateUniqueMethodOptions<BookContext>();
            using (var context = new BookContext(options))
            {

                //VERIFY
                var builder = new SqlConnectionStringBuilder(context.Database.GetDbConnection().ConnectionString);
                builder.InitialCatalog
                    .ShouldEndWith($"{GetType().Name}_{nameof(TestSqlServerUniqueMethodOk)}" );
            }
        }

        [Fact]
        public void TestCreateEmptyViaDeleteOk()
        {
            //SETUP
            var options = this.CreateUniqueMethodOptions<BookContext>();
            using (var context = new BookContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new BookContext(options))
            {
                //ATTEMPT
                using (new TimeThings(_output, "Time to delete and create the database"))
                {
                    context.CreateEmptyViaDelete();
                }

                //VERIFY
                context.Books.Count().ShouldEqual(0);

            }
        }

        [RunnableInDebugOnly]
        public void TestCreateDbToGetLogsOk()
        {
            //SETUP
            var logs = new List<string>();
            var options = this.CreateUniqueClassOptionsWithLogTo<BookContext>(log => logs.Add(log));
            using (var context = new BookContext(options))
            {
                //ATTEMPT
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //VERIFY
                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestAddExtraBuilderOptions()
        {
            //SETUP
            var options1 = this.CreateUniqueMethodOptions<BookContext>();
            using (var context = new BookContext(options1))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                var book = context.Books.First();
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
            //ATTEMPT
            var options2 = this.CreateUniqueMethodOptions<BookContext>(
                builder => builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            using (var context = new BookContext(options2))
            {
                //VERIFY
                var book = context.Books.First();
                context.Entry(book).State.ShouldEqual(EntityState.Detached);

            }
        }

        [Fact]
        public void TestQueryData()
        {
            //SETUP
            var options = this.CreateUniqueMethodOptions<BookContext>();
            using (var context = new BookContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new BookContext(options))
            {
                var book = context.Books.FirstOrDefault(x => x.Title == "Refactoring");
                book.ShouldNotBeNull();
                book = context.Books.FirstOrDefault(x => x.Title == "FakeTitle");
                book.ShouldBeNull();
            }

        }
    }
}