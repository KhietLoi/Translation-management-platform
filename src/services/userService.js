import axios from "axios";

const API_URL = "http://localhost:5182/api/User";

export const userService = {
  getUsers: (
    page = 1,
    limit = 5,
    search = ""
  ) =>
    axios.get(API_URL, {
      params: {
        Page: page,
        Limit: limit,
        Search: search,
      },
    }),

  getUserById: (id) =>
    axios.get(`${API_URL}/${id}`),

  createUser: (data) =>
    axios.post(API_URL, data),

  updateUser: (id, data) =>
    axios.put(`${API_URL}/${id}`, data),

  deleteUser: (id) =>
    axios.delete(`${API_URL}/${id}`),
};