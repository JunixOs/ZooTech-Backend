using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.UnitTests.Modules.Module_Vacuno;

public sealed class VacunoPhotoStorageTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(),
        "zootech-photo-tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SaveAndReadAsync_ValidPng_UsesTenantDirectoryAndPreservesContent()
    {
        var storage = CreateStorage(17);
        byte[] content = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

        var saved = await storage.SaveAsync(
            new VacunoPhotoUpload("referencia.png", "image/png", content));
        var read = await storage.ReadAsync(saved.RelativePath);

        saved.RelativePath.Should().StartWith("17/vacunos/");
        saved.ContentType.Should().Be("image/png");
        saved.Sha256.Should().HaveLength(64);
        read.Should().NotBeNull();
        read!.Content.Should().Equal(content);
    }

    [Fact]
    public async Task SaveAsync_ContentDoesNotMatchMimeType_RejectsFile()
    {
        var storage = CreateStorage(17);
        var upload = new VacunoPhotoUpload(
            "archivo.png",
            "image/png",
            "not an image"u8.ToArray());

        var action = () => storage.SaveAsync(upload);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*JPG, JPEG o PNG*");
    }

    [Fact]
    public async Task ReadAsync_PathFromAnotherTenant_DoesNotExposeFile()
    {
        var tenant17Storage = CreateStorage(17);
        var tenant18Storage = CreateStorage(18);
        byte[] content = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        var saved = await tenant17Storage.SaveAsync(
            new VacunoPhotoUpload("referencia.png", "image/png", content));

        var result = await tenant18Storage.ReadAsync(saved.RelativePath);

        result.Should().BeNull();
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private VacunoPhotoStorage CreateStorage(int tenantId)
    {
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(context => context.TenantId).Returns(tenantId);
        var options = Options.Create(new ReportStorageOptions { VacunoMediaRoot = _root });
        return new VacunoPhotoStorage(options, tenant.Object);
    }
}
