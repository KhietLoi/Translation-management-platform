import api from "./api";

export const getDashboardData = async () => {
  const response = await api.get("/Dashboard");
  return response.data;
};
