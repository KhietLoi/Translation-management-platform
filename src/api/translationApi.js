import axios from "axios";

// Helper to construct request headers
const getHeaders = (apiKey) => {
    const headers = {
        "Content-Type": "application/json"
    };
    // If apiKey is explicitly null/empty string (for testing missing key 401), don't send header
    if (apiKey !== null && apiKey !== undefined && apiKey !== "") {
        headers["X-API-KEY"] = apiKey;
    }
    return headers;
};

export const getTranslations = async (language, options = {}) => {
    const baseURL = options.baseUrl || import.meta.env.VITE_API_URL || "https://localhost:7185";
    const apiKey = options.apiKey !== undefined ? options.apiKey : import.meta.env.VITE_API_KEY;

    const url = `${baseURL}/api/sdk/projects/translations`;
    
    const response = await axios.get(url, {
        params: { language },
        headers: getHeaders(apiKey)
    });

    return response.data;
};

export const getVersion = async (projectId, options = {}) => {
    const baseURL = options.baseUrl || import.meta.env.VITE_API_URL || "https://localhost:7185";
    const apiKey = options.apiKey !== undefined ? options.apiKey : import.meta.env.VITE_API_KEY;

    const url = `${baseURL}/api/sdk/projects/${projectId}/version`;

    const response = await axios.get(url, {
        headers: getHeaders(apiKey)
    });

    return response.data;
};

export const getPackage = async (projectId, options = {}) => {
    const baseURL = options.baseUrl || import.meta.env.VITE_API_URL || "https://localhost:7185";
    const apiKey = options.apiKey !== undefined ? options.apiKey : import.meta.env.VITE_API_KEY;

    const url = `${baseURL}/api/sdk/projects/${projectId}/package`;

    const response = await axios.get(url, {
        headers: getHeaders(apiKey),
        responseType: 'blob' // package is a zip file
    });

    return response;
};
