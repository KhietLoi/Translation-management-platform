import api from "./api";

//Project:
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

export const createProject = async (project) => {
    const response = await api.post(
        "/Project",
        project
    );

    return response.data;
};

export const createProjectFull = async (data) => {
    const response = await api.post(
        "/Project/Pipeline",
        data
    );

    return response.data;
};

export const updateProject = async (id, data) => {
    const targetId = typeof id === "object" ? id.id : id;
    const body = typeof id === "object" ? id : data;
    const response = await api.put(
        `/Project/${targetId}`,
        body
    );

    return response.data;
};

export const deleteProject = async (id) => {
    const response = await api.delete(
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

// Languages:
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

export const createProjectNamespace = async (projectId, name) => {
    const response = await api.post(
        `/Project/${projectId}/namespaces`,
        {
            Name: name
        }
    );

    return response.data;
};

export const updateProjectNamespace = async (
    projectId,
    namespaceId,
    name
) => {

    const response = await api.put(
        `/Project/${projectId}/namespaces/${namespaceId}`,
        {
            ProjectId: projectId,
            Name: name
        }
    );

    return response.data;
};

export const deleteProjectNamespace = async (
    projectId,
    namespaceId
) => {

    const response = await api.delete(
        `/Project/${projectId}/namespaces/${namespaceId}`
    );

    return response.data;
};
