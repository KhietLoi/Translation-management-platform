import { normalizeStatus, TranslationStatus } from "../utils/translationStatus";

export const getOverallStatus = (rawValues = []) => {
    const values = Array.isArray(rawValues) ? rawValues : [];
    if (!values.length) {
        return TranslationStatus.Missing;
    }

    const statuses = values.map(x => normalizeStatus(x?.status ?? x?.Status));

    if (statuses.some(x => x === TranslationStatus.Missing)) {
        return TranslationStatus.Missing;
    }

    if (statuses.some(x => x === TranslationStatus.Draft)) {
        return TranslationStatus.Draft;
    }

    if (statuses.some(x => x === TranslationStatus.Rejected)) {
        return TranslationStatus.Rejected;
    }

    if (statuses.some(x => x === TranslationStatus.Translated)) {
        return TranslationStatus.Translated;
    }

    if (statuses.every(x => x === TranslationStatus.Published)) {
        return TranslationStatus.Published;
    }

    if (statuses.every(x => x === TranslationStatus.Reviewed || x === TranslationStatus.Published)) {
        return TranslationStatus.Reviewed;
    }

    return TranslationStatus.Missing;
};