using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiClients;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.ApiClients
{
    [TestFixture]
    public class CourseManagementOuterApiClientTests
    {
        [Test]
        public async Task Get_Successful_ReturnsResponse()
        {
            var model = new Person { Name = "person name" };
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                });
            HttpClient httpClient = new HttpClient(mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://test");
            var sut = new CourseManagementOuterApiClient(httpClient, Mock.Of<ILogger<CourseManagementOuterApiClient>>());

            var (success, response) = await sut.Get<Person>("test");

            success.Should().BeTrue();
            response.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task Get_Unsuccessful_ReturnsFalseAndNull()
        {
            var model = new Person { Name = "person name" };
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"),
                    RequestMessage = new HttpRequestMessage()
                });
            HttpClient httpClient = new HttpClient(mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://test");
            var sut = new CourseManagementOuterApiClient(httpClient, Mock.Of<ILogger<CourseManagementOuterApiClient>>());

            var (success, response) = await sut.Get<Person>("test");

            success.Should().BeFalse();
            response.Should().BeNull();
        }

        class Person
        {
            public string Name { get; set; }
        }
    }
}
