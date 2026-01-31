namespace LoanFlow.Configuration;

public sealed class DatabaseSettings
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public bool AutoMigrate { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
    public int CommandTimeout { get; set; } = 30;
}
