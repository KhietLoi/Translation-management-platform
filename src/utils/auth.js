import { jwtDecode } from "jwt-decode";


export const saveToken = (token) => {

    localStorage.setItem(
        "accessToken",
        token
    );

};

export const getCurrentUser = () => {

    const token = localStorage.getItem("accessToken");

    if(!token)
        return null;


    return jwtDecode(token);
};

export const getPermissions = () => {

    const user = getCurrentUser();

    if(!user)
        return [];


    return user.permission || [];

};

export const hasPermission = (permission) => {

    const permissions = getPermissions();

    return permissions.includes(permission);

};