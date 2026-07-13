import api from "./api"

const API_URL = "/User";

export const userService = {
  getUsers: (
    page = 1,
    limit = 5,
    search = ""
  ) =>
    api.get(API_URL, {
      params: {
        Page: page,
        Limit: limit,
        Search: search,
      },
    }),

  getUserById: (id) =>
    api.get(`${API_URL}/${id}`),

  createUser: (data) =>
    api.post(API_URL, data),

  updateUser: (id, data) =>
    api.put(`${API_URL}/${id}`, data),

  deleteUser: (id) =>
    api.delete(`${API_URL}/${id}`),
};