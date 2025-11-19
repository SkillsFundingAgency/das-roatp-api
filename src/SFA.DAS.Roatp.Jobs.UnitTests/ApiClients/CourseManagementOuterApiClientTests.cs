using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiClients;

namespace SFA.DAS.Roatp.Jobs.UnitTests.ApiClients
{
    [TestFixture]
    public class CourseManagementOuterApiClientTests
    {
        private readonly Person _model = new Person { Name = "person name" };
        [Test]
        public async Task Get_Successful_ReturnsResponse()
        {
            var sut = new CourseManagementOuterApiClient(GetHttpClient(HttpStatusCode.OK), Mock.Of<ILogger<CourseManagementOuterApiClient>>());

            var (success, response) = await sut.Get<Person>("test");

            success.Should().BeTrue();
            response.Should().BeEquivalentTo(_model);
        }

        [Test]
        public async Task Get_Unsuccessful_ReturnsFalseAndNull()
        {
            var sut = new CourseManagementOuterApiClient(GetHttpClient(HttpStatusCode.BadRequest), Mock.Of<ILogger<CourseManagementOuterApiClient>>());

            var (success, response) = await sut.Get<Person>("test");

            success.Should().BeFalse();
            response.Should().BeNull();
        }

        [Test]
        public async Task Post_Successful_ReturnsResponse()
        {
            var sut = new CourseManagementOuterApiClient(PostHttpClient(HttpStatusCode.OK), Mock.Of<ILogger<CourseManagementOuterApiClient>>());
            var request = new TestRequest();
            var (success, response) = await sut.Post<TestRequest, Person>("test", request);

            success.Should().BeTrue();
            response.Should().BeEquivalentTo(_model);
        }

        [Test]
        public async Task Post_Unsuccessful_ReturnsFalseAndNull()
        {
            var sut = new CourseManagementOuterApiClient(PostHttpClient(HttpStatusCode.BadRequest), Mock.Of<ILogger<CourseManagementOuterApiClient>>());

            var request = new TestRequest();
            var (success, response) = await sut.Post<TestRequest, Person>("test", request);

            success.Should().BeFalse();
            response.Should().BeNull();
        }

        private HttpClient GetHttpClient(HttpStatusCode httpStatus)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatus,
                    Content = new StringContent(JsonSerializer.Serialize(_model, CourseManagementOuterApiClient.JsonSerializerOptions), Encoding.UTF8, "application/json"),
                    RequestMessage = new HttpRequestMessage()
                });
            HttpClient httpClient = new HttpClient(mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://test");
            return httpClient;
        }
        private HttpClient PostHttpClient(HttpStatusCode httpStatus)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatus,
                    Content = new StringContent(JsonSerializer.Serialize(_model), Encoding.UTF8, "application/json"),
                    RequestMessage = new HttpRequestMessage()
                });
            HttpClient httpClient = new HttpClient(mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://test");
            return httpClient;
        }

        class Person
        {
            public string Name { get; set; }
        }

        class TestRequest
        {
        }
    }
}
