// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.SignalR.Emulator.Controllers;
using Microsoft.Azure.SignalR.Emulator.HubEmulator;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Azure.SignalR.Emulator.Tests.Controllers
{
    public class SignalRServiceEmulatorWebApiTests
    {
        private readonly Mock<IDynamicHubContextStore> _storeMock;
        private readonly Mock<ILogger<SignalRServiceEmulatorWebApi>> _loggerMock;
        private readonly SignalRServiceEmulatorWebApi _controller;

        public SignalRServiceEmulatorWebApiTests()
        {
            _storeMock = new Mock<IDynamicHubContextStore>();
            _loggerMock = new Mock<ILogger<SignalRServiceEmulatorWebApi>>();
            _controller = new SignalRServiceEmulatorWebApi(_storeMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void CheckConnectionExistenceValidConnectionReturnsOk()
        {
            // Arrange
            var hub = "testHub";
            var connectionId = "testConnectionId";
            var application = "testApplication";

            DynamicHubContext dynamicHubContext = null;
            _storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);


            // Act
            var result = _controller.CheckConnectionExistence(hub, connectionId, application);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CheckConnectionExistenceInvalidConnectionReturnsNotFound()
        {
            // arrange
            var hub = "testhub";
            var connectionid = "testconnectionid";
            var application = "testapplication";

            DynamicHubContext dynamicHubContext = null;
            _storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            //_controller.Response = new DefaultHttpContext().Response;

            // act
            var result = _controller.CheckConnectionExistence(hub, connectionid, application);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

        //[Fact]
        //public async Task CheckConnectionExistence_InvalidModelState_ReturnsBadRequest()
        //{
        //    // Arrange
        //    _controller.ModelState.AddModelError("key", "error");

        //    // Act
        //    var result = await _controller.CheckConnectionExistence("testHub", "testConnectionId", "testApplication");

        //    // Assert
        //    Assert.IsType<BadRequestResult>(result);
        //}
    }
}


