import api from "./api";

// Helper to round project progress percentage to an integer
const processProjectProgress = (p) => {
    if (!p) return p;
    const completed = Number(
        p.completedTranslationCount ??
        p.CompletedTranslationCount ??
        p.completedTranslationsCount ??
        p.CompletedTranslationsCount ??
        p.completedTranslations ??
        0
    );
    const total = Number(
        p.totalTranslationCount ??
        p.TotalTranslationCount ??
        p.totalTranslationsCount ??
        p.TotalTranslationsCount ??
        p.totalTranslations ??
        0
    );
    let rawPercent = Number(
        p.progressPercentage ??
        p.ProgressPercentage ??
        (total > 0 ? (completed / total) * 100 : 0)
    );
    if (total > 0 && completed >= total && rawPercent > 100) {
        rawPercent = 100;
    }
    const percent = Math.min(100, Math.max(0, Math.round(rawPercent)));
    return {
        ...p,
        progressPercentage: percent,
        ProgressPercentage: percent
    };
};

//Project:
export const getProjects = async () => {
    const response = await api.get("/Project");
    const data = response.data;

    if (data?.projects && Array.isArray(data.projects)) {
        data.projects = data.projects.map(processProjectProgress);
    } else if (Array.isArray(data)) {
        return data.map(processProjectProgress);
    }

    return data;
};

export const getProjectById = async (id) => {
    const response = await api.get(
        `/Project/${id}`
    );
    const data = response.data;
    if (data?.project) {
        data.project = processProjectProgress(data.project);
    } else if (data) {
        return processProjectProgress(data);
    }
    return data;
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
