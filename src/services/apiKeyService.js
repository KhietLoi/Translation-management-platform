import api from "./api";

// Get API Key Grid with filtering & pagination
export const getApiKeyGrid = async (params = {}) => {
    const response = await api.get("/ApiKey/grid", { params });
    return response.data;
};

// Generate a new API Key for a specific application
export const generateApiKey = async (applicationId, payload) => {
    const response = await api.post(
        `/Application/applications/${applicationId}/api-keys`,
        payload
    );
    return response.data;
};

// Rotate an API Key (invalidates existing key and creates a fresh key)
export const rotateApiKey = async (apiKeyId) => {
    const response = await api.post(`/ApiKey/${apiKeyId}/rotate`);
    return response.data;
};

// Revoke an API Key
export const revokeApiKey = async (apiKeyId) => {
    const response = await api.put(`/ApiKey/${apiKeyId}/revoke`);
    return response.data;
};

// Assign permissions to an API Key
export const assignApiKeyPermissions = async (apiKeyId, permissions) => {
    const response = await api.put(`/ApiKey/${apiKeyId}/permissions`, {
        permissions,
    });
    return response.data;
};
