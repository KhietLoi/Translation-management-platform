import { useAuth } from "../contexts/AuthContext";

export function usePermission() {
    const { user } = useAuth();
    const permissions = user?.permissions || [];
    const hasPermission = (permission) => {
        return permissions.includes(permission);
    };
    const hasAnyPermission = (permissionList) => {
        return permissionList.some(permission =>
            permissions.includes(permission)
        );
    };
    const hasAllPermissions = (permissionList) => {
        return permissionList.every(permission =>
            permissions.includes(permission)
        );
    };

    return {
        permissions,
        hasPermission,
        hasAnyPermission,
        hasAllPermissions
    };
}