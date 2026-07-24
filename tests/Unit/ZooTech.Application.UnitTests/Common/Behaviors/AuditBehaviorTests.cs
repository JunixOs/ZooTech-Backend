using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Common.Behaviors;

public sealed class AuditBehaviorTests
{
    [Fact]
    public async Task Handle_WhenResponseProvidesMetadata_AuditsWithoutBinaryContent()
    {
        var auditService = Substitute.For<IAppAuditService>();
        AuditEventInfo? captured = null;
        auditService
            .When(service => service.AuditEventAsync(Arg.Any<AuditEventInfo>()))
            .Do(call => captured = call.Arg<AuditEventInfo>());
        var sut = new AuditBehavior<AuditableRequest, GeneratedReportDocument>(auditService);
        var response = new GeneratedReportDocument(
            [1, 2, 3, 4],
            "application/pdf",
            ".pdf",
            "actividad.pdf");

        var result = await sut.Handle(
            new AuditableRequest(),
            () => Task.FromResult(response));

        result.Should().BeSameAs(response);
        await auditService.Received(1).AuditEventAsync(Arg.Any<AuditEventInfo>());
        var metadata = captured!.ResponseValues.Should()
            .BeOfType<ExportedFileAuditMetadata>()
            .Subject;
        metadata.FileName.Should().Be("actividad.pdf");
        metadata.Format.Should().Be("pdf");
        metadata.SizeBytes.Should().Be(4);
        metadata.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenResponseDoesNotProvideMetadata_PreservesResponse()
    {
        var auditService = Substitute.For<IAppAuditService>();
        AuditEventInfo? captured = null;
        auditService
            .When(service => service.AuditEventAsync(Arg.Any<AuditEventInfo>()))
            .Do(call => captured = call.Arg<AuditEventInfo>());
        var sut = new AuditBehavior<AuditableRequest, PlainResponse>(auditService);
        var response = new PlainResponse("ok");

        await sut.Handle(
            new AuditableRequest(),
            () => Task.FromResult(response));

        await auditService.Received(1).AuditEventAsync(Arg.Any<AuditEventInfo>());
        captured!.ResponseValues.Should().BeSameAs(response);
    }

    [Fact]
    public async Task Handle_WhenGenealogyResponseContainsBytes_AuditsOnlyMetadata()
    {
        var auditService = Substitute.For<IAppAuditService>();
        AuditEventInfo? captured = null;
        auditService
            .When(service => service.AuditEventAsync(Arg.Any<AuditEventInfo>()))
            .Do(call => captured = call.Arg<AuditEventInfo>());
        var sut = new AuditBehavior<AuditableRequest, ExportarArbolGenealogicoOutput>(
            auditService);
        var response = new ExportarArbolGenealogicoOutput(
            [1, 2, 3],
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Genealogia_1.xlsx");

        await sut.Handle(
            new AuditableRequest(),
            () => Task.FromResult(response));

        var metadata = captured!.ResponseValues.Should()
            .BeOfType<ExportedFileAuditMetadata>()
            .Subject;
        metadata.FileName.Should().Be("Genealogia_1.xlsx");
        metadata.Format.Should().Be("xlsx");
        metadata.SizeBytes.Should().Be(3);
        metadata.GetType().GetProperties()
            .Should()
            .NotContain(property => property.PropertyType == typeof(byte[]));
    }

    private sealed record AuditableRequest : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.DataExport;
        public string Action => "Exportar";
    }

    private sealed record PlainResponse(string Value);
}
