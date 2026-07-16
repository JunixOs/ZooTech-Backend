namespace ZooTech.InterfaceAdapters.Utils
{
    public static class ExceptionExtensions
    {
        public static string ToAuditMessage(this Exception rootException)
        {
            var className = rootException.TargetSite?.DeclaringType?.Name ?? "UnknownClass";
            var method = rootException.TargetSite?.Name ?? "UnknownMethod";

            return $"{rootException.GetType().Name} in {className}.{method}: {rootException.Message}";
        }
    }
}