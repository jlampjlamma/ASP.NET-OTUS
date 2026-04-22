using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRepository<Partner>> _repositoryMock;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();

            //Создавай контроллер только через конструктор, свойства не трогай
            _fixture.Customize<PartnersController>(c => c.OmitAutoProperties());

            //исключительно для обучения, понимаю что два варианта получения SetPartnerPromoCodeLimitRequest не правильно
            _fixture.Customize<SetPartnerPromoCodeLimitRequest>(c =>
            c.With(p => p.Limit, 10)
            .With(p => p.EndDate, DateTime.Now.AddDays(10))
            );
        }

        private static SetPartnerPromoCodeLimitRequest CreateDefaultSetPartnerPromoCodeLimitRequest() =>
            new()
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

        public static IEnumerable<object[]> GetInvalidRequests()
        {
            yield return new object[]
            {
                new SetPartnerPromoCodeLimitRequest { Limit = 0, EndDate = DateTime.Now.AddDays(-1) }
            };
            yield return new object[]
            {
                new SetPartnerPromoCodeLimitRequest { Limit = -5, EndDate = DateTime.Now.AddDays(10) }
            };
            yield return new object[]
            {
                new SetPartnerPromoCodeLimitRequest { Limit = 10, EndDate = DateTime.Now.AddDays(-5) }
            };
        }

        //1. Если партнер не найден, то также нужно выдать ошибку 404;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenPartnerNotFound_Returns404()
        {
            //arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Partner)null);

            var request = CreateDefaultSetPartnerPromoCodeLimitRequest();

            var testId = Guid.NewGuid();

            var controller = _fixture.Create<PartnersController>();

            //act
            var result = await controller.SetPartnerPromoCodeLimitAsync(testId, request);

            //assert
            result.Should().BeOfType<NotFoundResult>()
                .Which.StatusCode.Should().Be(404);
        }

        //2. Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenPartnerNotActive_Returns400()
        {
            //arrange
            var testId = Guid.NewGuid();
            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, testId)
                .With(p => p.IsActive, false)
                .Without(p => p.PartnerLimits)
                .Create();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            //Решил попробывать requst через AutoFixture
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            var controller = _fixture.Create<PartnersController>();

            //act
            var result = await controller.SetPartnerPromoCodeLimitAsync(testId, request);

            //assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(400);

            //TODO: так же проверяем что не было никаких изменений в бд
            _repositoryMock.Verify(r => r.UpdateAsync(partner), Times.Never);
        }

        //5. Лимит должен быть больше 0;
        //TODO: под этот пункт добавлю комбинацию проверок с датой
        [Theory]
        [MemberData(nameof(GetInvalidRequests))]
        public async Task SetPartnerPromoCodeLimitAsync_WhenRequestIsInvalid_Returns400(SetPartnerPromoCodeLimitRequest invalidRequest)
        {
            //arrange
            var testId = Guid.NewGuid();
            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, testId)
                .With(p => p.IsActive, true)
                .Without(p => p.PartnerLimits)
                .Create();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var controller = _fixture.Create<PartnersController>();

            //act
            var result = await controller.SetPartnerPromoCodeLimitAsync(testId, invalidRequest);

            //assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(400);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Partner>()), Times.Never);
        }
        

        //3. Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenNoActiveLimit_CreatesNewLimitAndPreservesCounter()
        {
            //arrange
            var testId = Guid.NewGuid();
            var initialCounter = 15;
            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, testId)
                .With(p => p.IsActive, true)
                .With(p => p.NumberIssuedPromoCodes, initialCounter)
                .With(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>())
                .Create();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            var controller = _fixture.Create<PartnersController>();

            //act
            var result = await controller.SetPartnerPromoCodeLimitAsync(testId, request);

            //assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.StatusCode.Should().Be(201);

            partner.NumberIssuedPromoCodes.Should().Be(initialCounter);

            partner.PartnerLimits.Should().HaveCount(1)
                .And.ContainSingle(l => l.Limit == request.Limit);

            //6. Нужно убедиться, что сохранили новый лимит в базу данных(это нужно проверить Unit-тестом);
            _repositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenActiveLimit_CreatesNewLimitAndDeactivatesOldLimitAndResetCounter()
        {
            //arrange
            var partnerId = Guid.NewGuid();
            var initialCounter = 15;

            var oldLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.EndDate, DateTime.Now.AddDays(10))
                .With(p => p.CancelDate, (DateTime?)null)
                .Without(p => p.Partner)
                .Create();

            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .With(p => p.NumberIssuedPromoCodes, initialCounter)
                .With(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>() { oldLimit })
                .Create();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            var controller = _fixture.Create<PartnersController>();

            //act
            var result = await controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            //assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.StatusCode.Should().Be(201);

            //4. При установке лимита нужно отключить предыдущий лимит;
            oldLimit.CancelDate.Should().HaveValue()
                .And.BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
            //TODO: можно было бы использовать BeCloseTo с разницей в одну секунду
            //чтобы больше быть увереным, что использовалось DateTime.Now, но мне кажется это будет костыльный тест.
            //Я решил проверить адекватность даты. что она не вчера, не через час, а +- сейчас. 

            //Еще как вариант, можно было бы ввести интерфейс для времени IDateTimeProvider. Реализация для работы была бы вызовом DateTime.Now
            //а для тестов мы бы мокали этот интерфейс и таким образом проверяли бы его вызов и что дата не меняется
            //я думаю именно на это и намекалось в 7 пункте
            //НО я предполагаю что абстракция избыточна в столь маленьком приложении. Внутренний маленьких архитектор кричит "НИНАДА"

            partner.NumberIssuedPromoCodes.Should().Be(0);

            partner.PartnerLimits.Should().HaveCount(2)
                .And.ContainSingle(l => l.Limit == request.Limit && l.CancelDate == null);

            _repositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);
        }
    }
}