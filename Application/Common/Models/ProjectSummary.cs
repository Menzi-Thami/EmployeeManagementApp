namespace EmployeeManagementApp.Application.Common.Models
{
    /// <summary>
    /// Read model for the project list: one row per project, with the assigned
    /// employees' full names already aggregated by the query (", "-separated).
    /// </summary>
    public sealed class ProjectSummary
    {
        public const string EmployeeNameSeparator = ", ";

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Cost { get; set; }
        public string? EmployeeNames { get; set; }
    }
}
