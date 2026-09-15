using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationJobRepository :  IRepository<TranslationJob>
{
    Task<TranslationJob?> GetByIdAsync(Guid id);
    // Task<(List<TranslationJob> Items, int TotalCount)> GetHistoryAsync
    // (
    //         Guid projectId,
    //         int pageNumber,
    //         int pageSize,
    //         CancellationToken cancellationToken
    // );
}