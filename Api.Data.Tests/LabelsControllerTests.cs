using Api.Data.Controllers;
using Api.Data.Dtos;
using Api.Data.Repositories.LabelRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Api.Data.Tests;

public sealed class LabelsControllerTests
{
    [Fact]
    public async Task CreateLabels_RejectsAnEmptyBatch()
    {
        var repository = new StubLabelRepository();
        var controller = new LabelsController(repository);

        var result = await controller.CreateLabels([]);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Null(repository.CreatedLabels);
    }

    [Fact]
    public async Task CreateLabels_RejectsDuplicateAuthentiUids()
    {
        var repository = new StubLabelRepository();
        var controller = new LabelsController(repository);

        var result = await controller.CreateLabels([CreateRequest("AUTH-1", 100001), CreateRequest("AUTH-1", 100002)]);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Null(repository.CreatedLabels);
    }

    [Fact]
    public async Task CreateLabels_MapsAndPersistsTheBatchOnce()
    {
        var repository = new StubLabelRepository();
        var controller = new LabelsController(repository);

        var result = await controller.CreateLabels([CreateRequest("AUTH-1", 100001), CreateRequest("AUTH-2", 100002)]);

        Assert.IsType<OkObjectResult>(result);
        var labels = Assert.IsAssignableFrom<IReadOnlyCollection<Label>>(repository.CreatedLabels);
        Assert.Equal(2, labels.Count);
        Assert.Equal(["AUTH-1", "AUTH-2"], labels.Select(label => label.AuthentiUid));
        Assert.All(labels, label => Assert.False(string.IsNullOrWhiteSpace(label.LabelUid)));
        Assert.Equal([100001, 100002], labels.Select(label => label.CurrentSerialNumber));
    }

    private static LabelDto CreateRequest(string authentiUid, int serialNumber) => new()
    {
        AuthentiUid = authentiUid,
        UserUid = "USER-1",
        ProductUid = "PRODUCT-1",
        ManufacturingOrderUid = 12345,
        CreationDate = DateTimeOffset.Parse("2026-09-22T08:00:00+02:00"),
        CurrentSerialNumber = serialNumber,
        ProductVersion = "1.0",
        LabelTypeUid = "LOT",
        Template = "lot.prn"
    };

    private sealed class StubLabelRepository : ILabelRepository
    {
        public IReadOnlyCollection<Label>? CreatedLabels { get; private set; }

        public Task CreateLabels(IReadOnlyCollection<Label> labels)
        {
            CreatedLabels = labels;
            return Task.CompletedTask;
        }

        public Task CreateLabel(Label label) => throw new NotSupportedException();
        public Task<ICollection<Label>> GetCollectionLabelsByManufacturingOrderUid(int manufacturingOrderUid) => throw new NotSupportedException();
        public Task<ICollection<Label>> GetCollectionLabelsMostRecentByManufacturingOrderUid(int manufacturingOrderUid) => throw new NotSupportedException();
        public Task<Label> GetLabelByAuthentiUid(string authentiUid) => throw new NotSupportedException();
        public Task<bool> LabelExists(string authentiUid) => throw new NotSupportedException();
        public Task<bool> LabelPrinted(int manufacturingOrderUid, string usageUid) => throw new NotSupportedException();
        public Task<ICollection<Label>> GetCollectionLabelsByKeywordSearch(string keyword, int manufacturingOrderUid, string usageUid) => throw new NotSupportedException();
        public Task<List<Label>> GetCollectionLabelsByManufacturingOrderUidAndUsageUid(int manufacturingOrderUid, string usageUid) => throw new NotSupportedException();
        public Task<Label> GetLabelLastSerialNumberPrintedByManufacturingOrderUid(int manufacturingOrderUid) => throw new NotSupportedException();
    }
}
