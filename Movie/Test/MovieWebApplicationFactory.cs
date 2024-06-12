﻿using Moq;
using Movie.Dto;
using Movie.Service;

namespace Movie.Test
{
    public class PersonWebApplicationFactory : CustomWebApplicationFactory<CustomStartup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IMovieService>(serviceProvider =>
                {
                    var mock = new Mock<IMovieService>();
                    mock.Setup(x => x.Get(It.IsAny<CancellationToken>(), 1)).
                    ReturnsAsync(new MovieResponseModel
                    {
                        Id = 1,
                        Title = "Lord of the rings",
                        Description = "",
                    });

                    mock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>(), It.Is<MovieRequestModel>(x => x.Id == 1))).Verifiable();

                    return mock.Object;
                });
            });
        }
    }
}