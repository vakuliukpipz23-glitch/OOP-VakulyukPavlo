using Moq;
using Xunit;
using lab31vN;

namespace lab31vN.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> repoMock;
        private readonly Mock<IEmailService> emailMock;
        private readonly OrderService service;

        public OrderServiceTests()
        {
            repoMock = new Mock<IOrderRepository>();
            emailMock = new Mock<IEmailService>();
            service = new OrderService(repoMock.Object, emailMock.Object);
        }

        [Fact]
        public void CreateOrder_ShouldSaveOrder()
        {
            var order = new Order { Id = 1, CustomerEmail = "test@mail.com", Amount = 100 };

            service.CreateOrder(order);

            repoMock.Verify(r => r.Save(order), Times.Once);
        }

        [Fact]
        public void CreateOrder_ShouldSendEmail()
        {
            var order = new Order { Id = 1, CustomerEmail = "test@mail.com", Amount = 100 };

            service.CreateOrder(order);

            emailMock.Verify(e => e.SendConfirmation(order.CustomerEmail), Times.Once);
        }

        [Fact]
        public void CreateOrder_NullOrder_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => service.CreateOrder(null));
        }

        [Fact]
        public void CreateOrder_InvalidAmount_ShouldThrowException()
        {
            var order = new Order { Amount = 0 };

            Assert.Throws<ArgumentException>(() => service.CreateOrder(order));
        }

        [Fact]
        public void GetOrder_ShouldReturnOrder()
        {
            var order = new Order { Id = 1, Amount = 50 };

            repoMock.Setup(r => r.GetById(1)).Returns(order);

            var result = service.GetOrder(1);

            Assert.Equal(order, result);
        }

        [Fact]
        public void GetOrder_ShouldCallRepository()
        {
            repoMock.Setup(r => r.GetById(1)).Returns(new Order());

            service.GetOrder(1);

            repoMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Fact]
        public void CreateOrder_ShouldCallSaveOnce()
        {
            var order = new Order { CustomerEmail = "test@mail.com", Amount = 200 };

            service.CreateOrder(order);

            repoMock.Verify(r => r.Save(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void CreateOrder_ShouldCallEmailServiceOnce()
        {
            var order = new Order { CustomerEmail = "test@mail.com", Amount = 200 };

            service.CreateOrder(order);

            emailMock.Verify(e => e.SendConfirmation(It.IsAny<string>()), Times.Once);
        }
    }
}