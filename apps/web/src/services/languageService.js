import api from "./api";

export const getLanguages = async () => {
    const response = await api.get("/Language");

    return response.data;
};