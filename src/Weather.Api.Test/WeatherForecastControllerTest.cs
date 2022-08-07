using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Weather.Api.Controllers;
using Xunit;

namespace Weather.Api.Test;

public class WeatherForecastControllerTest
{
    [Fact]
    public void Get_ShouldReturnWeatherForecastResult()
    {
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var controller = new WeatherForecastController(loggerMock.Object);

        var result = controller.Get() as IEnumerable<WeatherForecast>;
        result.Should().HaveCount(5);
    }
}