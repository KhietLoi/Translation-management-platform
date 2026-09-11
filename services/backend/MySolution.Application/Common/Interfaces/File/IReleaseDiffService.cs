using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.File;

public interface IReleaseDiffService
{
    Task<ReleaseDiffResult> CompareAsync(
        TranslationRelease sourceRelease,
        TranslationRelease targetRelease,
        CancellationToken cancellationToken);
}