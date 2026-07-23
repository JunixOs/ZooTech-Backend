using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

internal static class VacunoReportTheme
{
    public const string Primary = "#802b4d";
    public const string PrimaryLight = "#ecdfe4";
    public const string TextDark = "#2d2d2d";
    public const string BorderSoft = "#d9bfca";

    public static StyledReportTheme Styled { get; } = new(
        Primary,
        PrimaryLight,
        TextDark,
        BorderSoft);
}
