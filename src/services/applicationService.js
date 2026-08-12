import api from "./api";

// Get all applications
export const getApplications = async () => {
    const response = await api.get("/Application");
    return response.data;
};

// Get application by ID
export const getApplicationById = async (id) => {
    const response = await api.get(`/Application/${id}`);
    return response.data;
};

// Create new application
export const createApplication = async (data) => {
    const response = await api.post("/Application", data);
    return response.data;
};

// Update existing application
export const updateApplication = async (id, data) => {
    const response = await api.put(`/Application/${id}`, data);
    return response.data;
};

// Delete application
export const deleteApplication = async (id) => {
    const response = await api.delete(`/Application/${id}`);
    return response.data;
};
