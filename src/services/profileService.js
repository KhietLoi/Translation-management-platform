import api from "./api";

export const getProfileById = (userId) => {
    return api.get(`/UserProfile/${userId}`);
};

export const updateProfile = (userId, payload) => {
    return api.put(`/UserProfile/${userId}`, payload);
};

export const uploadAvatar = (userId, file) => {

    const formData = new FormData();

    formData.append("avatarFile", file);

    return api.post(
        `/UserProfile/avatar?userId=${userId}`,
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }
    );
};