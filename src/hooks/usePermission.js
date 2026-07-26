import { useAuth } from "../contexts/AuthContext";

export default function usePermission() {
    const { user } = useAuth();
    const hasPermission = (permission) => user?.permissions?.includes(permission);

    return {
        hasPermission
    };
}