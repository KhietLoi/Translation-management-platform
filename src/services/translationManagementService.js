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