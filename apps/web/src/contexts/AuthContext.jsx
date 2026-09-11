import { createContext, useContext, useState } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {

    const [user, setUser] = useState(() => {
        const storedUser = localStorage.getItem("currentUser");

        return storedUser ? JSON.parse(storedUser) : null;
    });

    const hasPermission = (permission) => {
        if (!user) {
            return false;
        }

        return user.permissions.includes(permission);
    }

    return (
        <AuthContext.Provider value={{ user, setUser, hasPermission }}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => useContext(AuthContext);