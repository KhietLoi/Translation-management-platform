export const TranslationStatus = {
    Missing: 0,
    Draft: 1,
    Translated: 2,
    Rejected: 3,
    Reviewed: 4,
    Published: 5
};

export const normalizeStatus = (status) => {
    if (typeof status === "number") return status;
    if (typeof status === "string") {
        const parsed = parseInt(status, 10);
        if (!isNaN(parsed)) return parsed;
        const lower = status.trim().toLowerCase();
        if (lower === "missing" || lower === "0") return TranslationStatus.Missing;
        if (lower === "draft" || lower === "1") return TranslationStatus.Draft;
        if (lower === "translated" || lower === "submitted" || lower === "2") return TranslationStatus.Translated;
        if (lower === "rejected" || lower === "3") return TranslationStatus.Rejected;
        if (lower === "reviewed" || lower === "approved" || lower === "4") return TranslationStatus.Reviewed;
        if (lower === "published" || lower === "5") return TranslationStatus.Published;
    }
    return Number(status);
};

export const getStatusText = (status) => {
    const numStatus = normalizeStatus(status);
    switch (numStatus) {
        case TranslationStatus.Missing:
            return "Missing";

        case TranslationStatus.Draft:
            return "Draft";

        case TranslationStatus.Translated:
            return "Submitted";

        case TranslationStatus.Rejected:
            return "Rejected";

        case TranslationStatus.Reviewed:
            return "Reviewed";

        case TranslationStatus.Published:
            return "Published";

        default:
            return "Unknown";
    }
};

export const getStatusColor = (status) => {
    const numStatus = normalizeStatus(status);
    switch (numStatus) {
        case TranslationStatus.Missing:
            return "secondary";

        case TranslationStatus.Draft:
            return "warning";

        case TranslationStatus.Translated:
            return "info";

        case TranslationStatus.Rejected:
            return "danger";

        case TranslationStatus.Reviewed:
            return "primary";

        case TranslationStatus.Published:
            return "success";

        default:
            return "secondary";
    }
};

export const getStatusBadgeClass = (status) => {
    return `bg-${getStatusColor(status)}`;
};