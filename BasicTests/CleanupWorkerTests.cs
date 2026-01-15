using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MortgageComparer.Services.BackgroundLogic;
using MortgageComparer.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTests {
    public class CleanupWorkerTests {
        [Fact]
        public async Task Execute_ShouldRunCleanupOnce_Async() {
            // Arrange
            var cleanupServiceMock = new Mock<ICleanupService>();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICleanupService)))
                .Returns(cleanupServiceMock.Object);

            var scopeMock = new Mock<IServiceScope>();
            scopeMock.Setup(s => s.ServiceProvider)
                     .Returns(serviceProviderMock.Object);

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            scopeFactoryMock
                .Setup(f => f.CreateScope())
                .Returns(scopeMock.Object);

            var logger = Mock.Of<ILogger<CleanupWorker>>();

            var worker = new CleanupWorker(scopeFactoryMock.Object, logger);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100); // jedna iteracja

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            cleanupServiceMock.Verify(
                s => s.ProcessExpiredOffersAsync(It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);

            cleanupServiceMock.Verify(
                s => s.ProcessOldQuotesAsync(It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Execute_ShouldCreateServiceScope_Async() {
            // Arrange
            var cleanupService = new Mock<ICleanupService>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(p => p.GetService(typeof(ICleanupService)))
                           .Returns(cleanupService.Object);

            var scope = new Mock<IServiceScope>();
            scope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

            var scopeFactory = new Mock<IServiceScopeFactory>();
            scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

            var logger = Mock.Of<ILogger<CleanupWorker>>();
            var worker = new CleanupWorker(scopeFactory.Object, logger);

            using var cts = new CancellationTokenSource(100);

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            scopeFactory.Verify(f => f.CreateScope(), Times.AtLeastOnce);
            scope.Verify(s => s.Dispose(), Times.AtLeastOnce);
        }


        [Fact]
        public async Task Execute_WhenExceptionOccurs_ShouldLogError_Async() {
            // Arrange
            var cleanupService = new Mock<ICleanupService>();
            cleanupService
                .Setup(s => s.ProcessExpiredOffersAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("BOOM"));

            var provider = new Mock<IServiceProvider>();
            provider.Setup(p => p.GetService(typeof(ICleanupService)))
                    .Returns(cleanupService.Object);

            var scope = new Mock<IServiceScope>();
            scope.Setup(s => s.ServiceProvider).Returns(provider.Object);

            var scopeFactory = new Mock<IServiceScopeFactory>();
            scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

            var loggerMock = new Mock<ILogger<CleanupWorker>>();

            var worker = new CleanupWorker(scopeFactory.Object, loggerMock.Object);

            using var cts = new CancellationTokenSource(100);

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }


        [Fact]
        public async Task Execute_ShouldStop_WhenTokenCancelled_Async() {
            // Arrange
            var cleanupService = new Mock<ICleanupService>();

            var provider = new Mock<IServiceProvider>();
            provider.Setup(p => p.GetService(typeof(ICleanupService)))
                    .Returns(cleanupService.Object);

            var scope = new Mock<IServiceScope>();
            scope.Setup(s => s.ServiceProvider).Returns(provider.Object);

            var scopeFactory = new Mock<IServiceScopeFactory>();
            scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

            var logger = Mock.Of<ILogger<CleanupWorker>>();

            var worker = new CleanupWorker(scopeFactory.Object, logger);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(50);

            // Act & Assert (nie rzuca)
            await worker.StartAsync(cts.Token);
        }

    }
}
