export const TranslationStatus = {
    Missing: 0,
    Draft: 1,
    Translated: 2,
    Rejected: 3,
    Reviewed: 4,
    Published: 5
};

export const getStatusText = (status) => {
    switch (Number(status)) {
        case TranslationStatus.Missing:
            return "Missing";

        case TranslationStatus.Draft:
            return "Draft";

        case TranslationStatus.Translated:
            return "Translated";

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
    switch (Number(status)) {

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
            return "dark";
    }
};

export const getStatusBadgeClass = (status) => {
    return `bg-${getStatusColor(status)}`;
};