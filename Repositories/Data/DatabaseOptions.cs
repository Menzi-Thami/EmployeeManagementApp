namespace EmployeeManagementApp.Infrastructure.Data
{
    /// <summary>Bound from the "ConnectionStrings" section and validated at startup.</summary>
    public sealed class DatabaseOptions
    {
        public const string Section = "ConnectionStrings";

        public string DefaultConnection { get; set; } = string.Empty;
    }
}
