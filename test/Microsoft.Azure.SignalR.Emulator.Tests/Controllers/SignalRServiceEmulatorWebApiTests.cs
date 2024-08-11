// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Emulator.Controllers;
using Microsoft.Azure.SignalR.Emulator.HubEmulator;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Azure.SignalR.Emulator.Tests.Controllers
{
    public class SignalRServiceEmulatorWebApiTests
    {
        private readonly Mock<IDynamicHubContextStore> storeMock;
        private readonly Mock<ILogger<SignalRServiceEmulatorWebApi>> loggerMock;
        private readonly SignalRServiceEmulatorWebApi controller;

        // Constants
        private const string testHub = "testHub";
        private const string testConnectionId = "testConnectionId";
        private const string testGroup = "testGroup";
        private const string testUser = "testUser";
        private const string testApplication = "testApplication";

        private const string MicrosoftErrorCode = "x-ms-error-code";

        private const string Warning_Connection_NotExisted = "Warning.Connection.NotExisted";
        private const string Warning_Group_NotExisted = "Warning.Group.NotExisted";
        private const string Warning_User_NotExisted = "Warning.User.NotExisted";
        private const string Error_Connection_NotExisted = "Error.Connection.NotExisted";
        private const string Info_User_NotInGroup = "Info.User.NotInGroup";

        public SignalRServiceEmulatorWebApiTests()
        {
            storeMock = new Mock<IDynamicHubContextStore>();
            loggerMock = new Mock<ILogger<SignalRServiceEmulatorWebApi>>();
            controller = new SignalRServiceEmulatorWebApi(storeMock.Object, loggerMock.Object);

            HttpContext httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            controller.ControllerContext = controllerContext;
        }

        // CheckConnectionExistence Tests
        [Fact]
        public void CheckConnectionExistenceValidConnectionReturnsOk()
        {
            // Arrange
            var connectionContext = new DefaultConnectionContext();
            connectionContext.ConnectionId = testConnectionId;

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var hubConnectionContext = new HubConnectionContext(connectionContext, new HubConnectionContextOptions(), mockLoggerFactory.Object);

            var connectionStore = new HubConnectionStore();
            connectionStore.Add(hubConnectionContext);

            var lifetimeManagerMock = new Mock<IHubLifetimeManager>();
            lifetimeManagerMock.Setup(l => l.Connections).Returns(connectionStore);

            var dynamicHubContext = new DynamicHubContext(typeof(DynamicHubContext), null, lifetimeManagerMock.Object, null);
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.CheckConnectionExistence(testHub, testConnectionId, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CheckConnectionExistenceInvalidConnectionReturnsNotFound()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.CheckConnectionExistence(testHub, testConnectionId, testApplication);

            // assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(Warning_Connection_NotExisted, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void CheckConnectionExistenceInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.CheckConnectionExistence(testHub, testConnectionId, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        // CheckGroupExistence Tests
        [Fact]
        public void CheckGroupExistenceValidConnectionReturnsOk()
        {
            // Arrange
            var groupManager = new GroupManager();
            groupManager.AddConnectionIntoGroup(testConnectionId, testGroup);

            var dynamicHubContextMock = new Mock<DynamicHubContext>();
            dynamicHubContextMock.Setup(d => d.UserGroupManager).Returns(groupManager);

            var dynamicHubContext = dynamicHubContextMock.Object;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.CheckGroupExistence(testHub, testGroup, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CheckGroupExistenceInvalidConnectionReturnsNotFound()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.CheckGroupExistence(testHub, testGroup, testApplication);

            // assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(Warning_Group_NotExisted, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void CheckGroupExistenceInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.CheckGroupExistence(testHub, testGroup, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        // CheckUserExistence Tests
        [Fact]
        public void CheckUserExistenceValidConnectionReturnsOk()
        {
            // Arrange
            var connectionContext = new DefaultConnectionContext();

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var hubConnectionContext = new HubConnectionContext(connectionContext, new HubConnectionContextOptions(), mockLoggerFactory.Object);
            hubConnectionContext.UserIdentifier = testUser;

            var connectionStore = new HubConnectionStore();
            connectionStore.Add(hubConnectionContext);

            var lifetimeManagerMock = new Mock<IHubLifetimeManager>();
            lifetimeManagerMock.Setup(l => l.Connections).Returns(connectionStore);

            var dynamicHubContext = new DynamicHubContext(typeof(DynamicHubContext), null, lifetimeManagerMock.Object, null);
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.CheckUserExistence(testHub, testUser, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CheckUserExistenceInvalidConnectionReturnsNotFound()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.CheckUserExistence(testHub, testUser, testApplication);

            // assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(Warning_User_NotExisted, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void CheckUserExistenceInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.CheckUserExistence(testHub, testUser, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        // RemoveConnectionFromAllGroups Tests
        [Fact]
        public void RemoveConnectionFromAllGroupsValidConnectionReturnsOk()
        {
            // Arrange
            var groupManager = new GroupManager();
            groupManager.AddConnectionIntoGroup(testConnectionId, testGroup);

            var dynamicHubContextMock = new Mock<DynamicHubContext>();
            dynamicHubContextMock.Setup(d => d.UserGroupManager).Returns(groupManager);

            var dynamicHubContext = dynamicHubContextMock.Object;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.RemoveConnectionFromAllGroups(testHub, testConnectionId, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void RemoveConnectionFromAllGroupsInvalidConnectionReturnsOk()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.RemoveConnectionFromAllGroups(testHub, testConnectionId, testApplication);

            // assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(Error_Connection_NotExisted, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void RemoveConnectionFromAllGroupsInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.RemoveConnectionFromAllGroups(testHub, testConnectionId, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        // AddConnectionToGroup Tests
        [Fact]
        public void AddConnectionToGroupValidConnectionReturnsOk()
        {
            // Arrange
            var connectionContext = new DefaultConnectionContext();
            connectionContext.ConnectionId = testConnectionId;

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var hubConnectionContext = new HubConnectionContext(connectionContext, new HubConnectionContextOptions(), mockLoggerFactory.Object);

            var connectionStore = new HubConnectionStore();
            connectionStore.Add(hubConnectionContext);

            var lifetimeManagerMock = new Mock<IHubLifetimeManager>();
            lifetimeManagerMock.Setup(l => l.Connections).Returns(connectionStore);

            var dynamicHubContext = new DynamicHubContext(typeof(DynamicHubContext), null, lifetimeManagerMock.Object, null);
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.AddConnectionToGroup(testHub, testGroup, testConnectionId, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void AddConnectionToGroupInvalidConnectionReturnsNotFound()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.AddConnectionToGroup(testHub, testGroup, testConnectionId, testApplication);

            // assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(Error_Connection_NotExisted, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void AddConnectionToGroupInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.AddConnectionToGroup(testHub, testGroup, testConnectionId, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        // CheckUserExistenceInGroup Tests
        [Fact]
        public void CheckUserExistenceInGroupValidConnectionReturnsOk()
        {
            // Arrange
            var expireAt = System.DateTimeOffset.Now.AddMinutes(10);

            var groupManager = new GroupManager();
            groupManager.AddUserToGroup(testUser, testGroup, expireAt);

            var dynamicHubContextMock = new Mock<DynamicHubContext>();
            dynamicHubContextMock.Setup(d => d.UserGroupManager).Returns(groupManager);

            var dynamicHubContext = dynamicHubContextMock.Object;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(true);

            // Act
            var result = controller.CheckUserExistenceInGroup(testHub, testGroup, testUser, testApplication);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CheckUserExistenceInGroupInvalidConnectionReturnsNotFound()
        {
            // arrange
            DynamicHubContext dynamicHubContext = null;
            storeMock.Setup(s => s.TryGetLifetimeContext(It.IsAny<string>(), out dynamicHubContext)).Returns(false);

            // act
            var result = controller.CheckUserExistenceInGroup(testHub, testGroup, testUser, testApplication);

            // assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(Info_User_NotInGroup, controller.Response.Headers[MicrosoftErrorCode]);
        }

        [Fact]
        public void CheckUserExistenceInGroupInvalidModelStateReturnsBadRequest()
        {
            // arrange
            controller.ModelState.AddModelError("key", "error");

            // act
            var result = controller.CheckUserExistenceInGroup(testHub, testGroup, testUser, testApplication);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

    }
}

