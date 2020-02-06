using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QuestionsApp.Web.DB;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace QuestionsApp.Tests
{
    public class QuestionsTests
    {
        private readonly QuestionsContext Context;

        public QuestionsTests()
        {
            var options = new DbContextOptionsBuilder<QuestionsContext>().
                                UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            Context = new QuestionsContext(options);
        }

        private Web.Api.Controllers.Queries.QuestionsController NewQuery()
        {
            return new Web.Api.Controllers.Queries.QuestionsController(Context);
        }

        private Web.Api.Controllers.Commands.QuestionsController NewCommand()
        {
            return new Web.Api.Controllers.Commands.QuestionsController(Context);
        }

        [Fact]
        public void Empty()
        {
            var query = NewQuery();
            query.Get().Should().BeEmpty();
        }

        [Fact]
        public void OneQuestion()
        {
            var query = NewQuery();
            var command = NewCommand();

            command.Ask("Dummy Question").Should().NotBeNull();

            query.Get().Should().HaveCount(1);
        }

        [Fact]
        public void OneQuestionAndVote()
        {
            var query = NewQuery();
            var command = NewCommand();

            command.Ask("Dummy Question").Should().NotBeNull();

            var result = query.Get();
            result.Should().HaveCount(1);
            result[0].Votes.Should().Be(0);

            command.Vote(result[0].ID).Should().NotBeNull();
            result = query.Get();
            result.Should().HaveCount(1);
            result[0].Votes.Should().Be(1);
        }
    }
}
