namespace ZooTech.Application.Common.Models
{
    public sealed class EmptyCommand
    {
        public static readonly EmptyCommand Value = new();

        private EmptyCommand()
        {
        }
    }
}