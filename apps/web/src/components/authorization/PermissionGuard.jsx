import { useAuth } from "../../contexts/AuthContext";

export default function PermissionGuard({
    permission,
    children,
    fallback = null
}) {
    const { user } = useAuth();

    const hasPermission = user?.permissions?.includes(permission);

    if (!hasPermission) {
        return fallback;
    }

    return children;
}