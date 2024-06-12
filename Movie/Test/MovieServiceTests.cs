﻿using Moq;
using Movie.Dto;
using Movie.Entity;
using Movie.Exceptions;
using Movie.Repository;
using Movie.Service;
using Xunit;

namespace Movie.Test
{
    public class MovieServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IMovieRepository> _repository;
        private readonly MovieService _service;


        public MovieServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new Mock<IMovieRepository>();
            _service = new MovieService(_repository.Object);
        }

        [Fact]
        public async Task GetMovie_WhenIdNotExists_ShouldThrowMovieNotFoundException()
        {
            //arrange
            var service = new MovieService(new MockMovieRepository());

            //act
            var task = async () => await service.Get(CancellationToken.None, 2).ConfigureAwait(false);

            //assert
            var exception = await Assert.ThrowsAsync<MovieNotFoundException>(task);
            Assert.Equal($"Movie {2} not found", exception.Message);
        }

        [Fact]
        public async Task GetMovie_WhenIdNotExists_ShouldThrowMovieNotFoundException1()
        {
            //arrange
            _repository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>(), 2))
                .ReturnsAsync((MovieEntity)null);

            //act
            var task = async () => await _service.Get(CancellationToken.None, 2).ConfigureAwait(false);

            //assert
            var exception = await Assert.ThrowsAsync<MovieNotFoundException>(task);
            Assert.Equal($"Movie {2} not found", exception.Message);
        }

        [Fact]
        public async Task GetMovie_WhenIdExists_ShouldReturnOneMovie()
        {
            // Arrange
            var requestModel = new MovieRequestModel
            {
                Title = "Harry Potter",
                Description = "A story about a boy who lived",
            };

            var movieEntity = new MovieEntity
            {
                Id = 1,
                Title = requestModel.Title,
                Description = requestModel.Description,
            };

            // Simulate CreateAsync to add a movie and assign an ID
            _repository.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>(), It.IsAny<MovieEntity>()))
                .Returns(Task.CompletedTask)
                .Callback<CancellationToken, MovieEntity>((ct, movie) => movie.Id = 1);

            // Simulate GetAsync to retrieve the movie by ID
            _repository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>(), 1))
                .ReturnsAsync(movieEntity);

            // Act
            await _service.CreateAsync(CancellationToken.None, requestModel).ConfigureAwait(false);
            var movie = await _service.Get(CancellationToken.None, 1).ConfigureAwait(false);

            // Assert
            Assert.NotNull(movie);
            Assert.Equal(1, movie.Id);
            Assert.Equal("Harry Potter", movie.Title);
            Assert.Equal("A story about a boy who lived", movie.Description);
            Assert.IsType<MovieResponseModel>(movie);
        }

        [Fact]
        public async Task CreateMovie_WhenDataIsValid_ShoudAddedOneMovie()
        {
            //arrange
            var requestModel = new MovieRequestModel()
            {
                Title = "Harry Potter",
                Description = "A story about a boy who lived",
            };

            var repository = new MovieRepository(_fixture.Context);
            var service = new MovieService(repository);
            var last = await service.GetAll(CancellationToken.None);

            //act
            await service.CreateAsync(CancellationToken.None, requestModel).ConfigureAwait(false);
            var @new = await service.GetAll(CancellationToken.None);

            //assety
            Assert.NotNull(@new);
            Assert.Equal(last.Count + 1, @new.Count);
        }
    }
}
