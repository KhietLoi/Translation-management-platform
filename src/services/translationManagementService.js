import api from "./api";

export const getTranslationGrid = async ({
    projectId,
    namespaceId,
    keyword,
    status,
    numberOfLanguages = 3,
    pageNumber = 1,
    pageSize = 20
}) => {

    const response = await api.get(
        "/TranslationManagement/grid",
        {
            params: {
                projectId,
                namespaceId,
                keyword,
                status,
                numberOfLanguages,
                pageNumber,
                pageSize
            }
        }
    );

    return response.data;
};

export const createTranslationKey = async (payload) => {
    const response = await api.post(
        "/TranslationKey",
        payload
    );

    return response.data;
};

export const updateTranslationKey = async (
    id,
    payload
) => {
    const response = await api.put(
        `/TranslationKey/${id}`,
        payload
    );

    return response.data;
};

export const deleteTranslationKey = async (
    id
) => {
    const response =
        await api.delete(
            `/TranslationKey/${id}`
        );

    return response.data;
};

export const getTranslationValueById = async (id) => {
    const response = await api.get(
        `/TranslationValue/${id}`
    );

    return response.data;
};

export const updateTranslationValue = async (
    id,
    payload
) => {
    const response = await api.put(
        `/TranslationValue/${id}`,
        payload
    );

    return response.data;
};


export const submitTranslation = async (
    id
) => {
    const response = await api.post(
        `/TranslationManagement/${id}/submit`
    );

    return response.data;
};


export const reviewTranslation = async (
    id
) => {
    const response = await api.post(
        `/TranslationManagement/${id}/review`
    );

    return response.data;
};


export const rejectTranslation = async (
    id,
    payload
) => {
    const response = await api.post(
        `/TranslationManagement/${id}/reject`,
        payload
    );

    return response.data;
};

export const getReviewTranslations = async ({
    projectId,
    languageId,
    namespaceId
}) => {
    const response = await api.get(
        "/TranslationManagement/review",
        {
            params: {
                ProjectId: projectId,
                LanguageId: languageId,
                NamespaceId: namespaceId
            }
        }
    );

    return response.data;
};

export const batchReviewTranslations = async (payload) => {
    const response = await api.post(
        "/TranslationManagement/batch-review",
        payload
    );

    return response.data;
};

