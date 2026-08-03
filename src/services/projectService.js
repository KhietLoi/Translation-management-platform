import api from "./api";

export const getProjects = async () => {
    const response = await api.get("/Project");

    return response.data;
};

export const getProjectById = async (id) => {
    const response = await api.get(
        `/Project/${id}`
    );

    return response.data;
};

//members:
export const getProjectMembers = async (id) => {
    const response = await api.get(
        `/Project/${id}/members`
    );

    return response.data;
};
export const updateProjectMembers = async (
    projectId,
    memberIds
) => {

    console.log("projectId", projectId);
    console.log("memberIds", memberIds);
    const response = await api.put(
        `/Project/${projectId}/members`,
        {
            members: memberIds.map(id => ({
                userId: id
            }))
        }
    );

    return response.data;
};

export const getProjectLanguages = async (id) => {
    const response = await api.get(
        `/Project/${id}/languages`
    );

    return response.data;
};

export const updateProjectLanguages = async (
    projectId,
    languageIds
) => {

    const response = await api.put(
        `/Project/${projectId}/languages`,
        {
            languagesIds: languageIds
        }
    );

    return response.data;
};

//namespace:

export const getProjectNamespaces = async (projectId) => {

    const response = await api.get(
        `/Project/${projectId}/namespaces`
    );

    return response.data;
};

export const createProjectNamespace = async (
    projectId,
    name
) => {

    const response = await api.post(
        `/Project/namespaces?projectId=${projectId}`,
        {
            name
        }
    );

    return response.data;
};

export const updateProjectNamespace = async (
    namespaceId,
    name
) => {

    const response = await api.put(
        `/Project/namespaces/${namespaceId}`,
        {
            name
        }
    );

    return response.data;
};

export const deleteProjectNamespace = async (
    namespaceId
) => {

    const response = await api.delete(
        `/Project/namespaces/${namespaceId}`
    );

    return response.data;
};
