import api from "./api";

export const login = async (payload) => {
  const response = await api.post("/auth/login", payload);
  return response.data;
};

export const register = async (payload) => {
  const response = await api.post("/auth/register", payload);
  return response.data;
}

export const logout = () => {
  localStorage.removeItem("accessToken");
  window.location.href = "/";
}

export const verifyEmail = async (token) => {
  const response = await api.get(`/auth/verify-email/${token}`);
  return response.data;
}

export const resendVerificationEmail = async (email) => {
  const response = await api.post(`/auth/resend-verify-email`, { email: email });
  return response.data;
}

export const forgotPassword = async (email) => {
  const response = await api.post(`/auth/forgot-password`, { email: email });
  return response.data;
}

export const resetPassword = async (payload) => {
  const response = await api.post(`/auth/reset-password`, payload);
  return response.data;
}