﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.NormalSql.Books;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceNormalSqlBooks
{
    public class TestBookDbContext
    {
        [Fact]
        public void TestBookDbContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
            context.Authors.Count().ShouldEqual(3);
            context.Set<Review>().Count().ShouldEqual(2);
        }

        [Fact]
        public void TestBookDbContextSeedDatabaseFourBooksFillsInCacheValuesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var books = context.SeedDatabaseFourBooks();

            //VERIFY
            books.Select(x => new{ x.AuthorsOrdered, x.AuthorsLink.First().Author.Name})
                .All(x => x.AuthorsOrdered == x.Name).ShouldBeTrue();
            books.Select(x => new { x.ReviewsCount, x.Reviews})
                .All(x => x.ReviewsCount == x.Reviews.Count).ShouldBeTrue();
            books.Select(x => new { x.ReviewsAverageVotes, x.Reviews })
                .All(x => x.ReviewsAverageVotes ==
                          (x.Reviews.Any() ? x.Reviews.Average(y => y.NumStars) : 0.0)).ShouldBeTrue();
        }
    }
}