    using MySolution.Application.Common.Models;
    using MySolution.Domain.Enums;

    namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValuesForBatch;

    public class GetTranslationValuesForBatchResponse : BaseResponse <GetTranslationValuesForBatchData>
    {

    }

    public class GetTranslationValuesForBatchData
    {
        public List<TranslationValueBatchItem> Items { get; set; } = [];
    }

    public class TranslationValueBatchItem
    {
        public Guid TranslationValueId { get; set; }
        public Guid TranslationKeyId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public TranslationStatus Status { get; set; }
    }