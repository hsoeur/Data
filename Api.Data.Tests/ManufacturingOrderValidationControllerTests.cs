using System.ComponentModel.DataAnnotations;
using Api.Data.Controllers;
using Api.Data.Dtos;
using Api.Data.Repositories.ManufacturingOrderRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Api.Data.Tests;

public sealed class ManufacturingOrderValidationControllerTests
{
    [Theory]
    [InlineData("100001", true)]
    [InlineData("100100", true)]
    [InlineData("100101", false)]
    [InlineData("100000", false)]
    public async Task ValidateManufacturingOrder_UsesInclusiveSerialRange(string serialNumber, bool expected)
    {
        var controller = CreateController(CreateOrder());

        var action = await controller.ValidateManufacturingOrder(CreateRequest("0640A ", serialNumber));

        var response = Assert.IsType<ManufacturingOrderValidationResponse>(Assert.IsType<OkObjectResult>(action.Result).Value);
        Assert.Equal(expected, response.SerialNumberInRange);
        Assert.Equal(expected, response.IsValid);
        Assert.Equal(100100, response.LastSerialNumber);
    }

    [Fact]
    public async Task ValidateManufacturingOrder_PreservesTrailingSpaceInProductSuffix()
    {
        var controller = CreateController(CreateOrder());

        var action = await controller.ValidateManufacturingOrder(CreateRequest("0640A ", "100001"));

        var response = Assert.IsType<ManufacturingOrderValidationResponse>(Assert.IsType<OkObjectResult>(action.Result).Value);
        Assert.True(response.ProductMatches);
        Assert.True(response.IsValid);
    }

    [Fact]
    public async Task ValidateManufacturingOrder_ReturnsInvalidWhenProductDoesNotMatch()
    {
        var controller = CreateController(CreateOrder());

        var action = await controller.ValidateManufacturingOrder(CreateRequest("9999Z ", "100001"));

        var response = Assert.IsType<ManufacturingOrderValidationResponse>(Assert.IsType<OkObjectResult>(action.Result).Value);
        Assert.False(response.ProductMatches);
        Assert.True(response.SerialNumberInRange);
        Assert.False(response.IsValid);
    }

    [Fact]
    public async Task ValidateManufacturingOrder_ReturnsNotFoundWhenOrderDoesNotExist()
    {
        var controller = CreateController(null);

        var action = await controller.ValidateManufacturingOrder(CreateRequest("0640A ", "100001"));

        Assert.IsType<NotFoundObjectResult>(action.Result);
    }

    [Theory]
    [InlineData("0640A ", "100001", "26789", true)]
    [InlineData("0640A", "100001", "26789", false)]
    [InlineData("0640A ", "10001", "26789", false)]
    [InlineData("0640A ", "100001", "2678", false)]
    public void RequestValidation_EnforcesExactInputFormats(
        string suffix,
        string serialNumber,
        string manufacturingOrderId,
        bool expectedValid)
    {
        var request = new ManufacturingOrderValidationRequest
        {
            ProductSuffix = suffix,
            SerialNumber = serialNumber,
            ManufacturingOrderId = manufacturingOrderId
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(reques
            t), results, true);

        Assert.Equal(expectedValid, isValid);
    }

    private static ManufacturingOrderValidationController CreateController(ManufacturingOrder? order) =>
        new(new StubRepository(order));

    private static ManufacturingOrder CreateOrder() => new()
    {
        ManufacturingOrderUid = 26789,
        ProductUid = "PRODUCT-0640A ",
        Quantity = 100,
        FirstSerialNumber = 100001,
        InternalOrderNumber = "INTERNAL",
        ExternalOrderNumber = "EXTERNAL",
        Status = "ACTIVE"
    };

    private static ManufacturingOrderValidationRequest CreateRequest(string suffix, string serialNumber) => new()
    {
        ProductSuffix = suffix,
        SerialNumber = serialNumber,
        ManufacturingOrderId = "26789"
    };

    private sealed class StubRepository(ManufacturingOrder? order) : IManufacturingOrderRepository
    {
        public Task<ManufacturingOrder?> GetManufacturingOrderForValidation(int manufacturingOrderUid) => Task.FromResult(order);
        public Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrders() => throw new NotSupportedException();
        public Task<ManufacturingOrder?> GetManufacturingOrderByManufacturingOrderUid(string manufacturingOrderUid) => throw new NotSupportedException();
        public Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByArticleUid(string articleUid) => throw new NotSupportedException();
        public Task<ManufacturingOrder> GetManufacturingOrderByManufacturingOrderUidKeepAlive(string value) => throw new NotSupportedException();
        public Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByManufacturingOrderAmphitechOrder(string amphitechOrder) => throw new NotSupportedException();
        public Task<List<ManufacturingOrder>> GetCollectionManufacturingOrdersByHorizonOrder(string horizonOrder) => throw new NotSupportedException();
        public Task<bool> UpdateManufacturingOrderQuantityAndFirstSerial(string manufacturingOrderUid, string quantity, string firstSerialNumber, string operatorUid) => throw new NotSupportedException();
    }
}
