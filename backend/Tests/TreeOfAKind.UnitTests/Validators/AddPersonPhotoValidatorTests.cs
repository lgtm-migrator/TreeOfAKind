using System.IO;
using TreeOfAKind.Application.Command;
using TreeOfAKind.Application.Command.Trees.People.AddPersonFile;
using TreeOfAKind.Domain.Trees;
using TreeOfAKind.Domain.Trees.People;
using Xunit;
using AutoFixture;
using TreeOfAKind.Application.Command.Trees.People.AddOrChangePersonsPhoto;

namespace TreeOfAKind.UnitTests.Validators
{
    public class AddOrChangePersonPhotoCommandValidatorTests : ValidatorTestsBase<AddOrChangePersonsPhotoCommandValidator>
    {
        [Theory]
        [InlineData("image/jpeg")]
        [InlineData("image/png")]
        private void Validate_ValidData_ValidationPasses(string contentType)
        {
            var command = new AddOrChangePersonsPhotoCommand("AuthId", Fixture.Create<TreeId>(),
                new Document(Stream.Null, contentType, "name"), Fixture.Create<PersonId>());

            Assert.True(Validator.Validate(command).IsValid);
        }

        [Theory]
        [InlineData("image/gif")]
        [InlineData("asdf")]
        [InlineData("image/tiff")]
        [InlineData("")]
        [InlineData("application/pdf")]
        [InlineData(null)]
        private void Validate_InvalidContentType_ValidationFails(string contentType)
        {
            var command = new AddOrChangePersonsPhotoCommand("AuthId", Fixture.Create<TreeId>(),
                new Document(Stream.Null, contentType, "name"), Fixture.Create<PersonId>());

            Assert.False(Validator.Validate(command).IsValid);
        }

        [Fact]
        private void Validate_NoContent_ValidationFails()
        {
            var command = new AddOrChangePersonsPhotoCommand("AuthId", Fixture.Create<TreeId>(),
                new Document(null, "image/jpeg", "name"), Fixture.Create<PersonId>());

            Assert.False(Validator.Validate(command).IsValid);
        }

        [Fact]
        private void Validate_NoName_ValidationFails()
        {
            var command = new AddOrChangePersonsPhotoCommand("AuthId", Fixture.Create<TreeId>(),
                new Document(Stream.Null, "image/jpeg", null), Fixture.Create<PersonId>());

            Assert.False(Validator.Validate(command).IsValid);
        }

        [Fact]
        private void Validate_NoDocument_ValidationFails()
        {
            var command = new AddOrChangePersonsPhotoCommand("AuthId", Fixture.Create<TreeId>(),
                null, Fixture.Create<PersonId>());

            Assert.False(Validator.Validate(command).IsValid);
        }
    }
}
