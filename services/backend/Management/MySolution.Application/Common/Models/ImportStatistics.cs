namespace MySolution.Application.Common.Models;

public class ImportStatistics
{
    public int TotalRecords { get; set; }
    public int CreatedKeys { get; set; }
    public int CreatedValues { get; set; }
    public int UpdatedValues { get; set; }
    public int SkippedRecords { get; set; }
    public int FailedRecords { get; set; }
}