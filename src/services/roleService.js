import axios from "axios";

const API_URL =
  "http://localhost:5182/api/Role";

export const roleService = {
  getRoles: () =>
    axios.get(API_URL, {
      params: {
        Page: 1,
        Limit: 1000,
      },
    }),

  getRolesPaging: (
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

  getRoleById: (id) =>
    axios.get(`${API_URL}/${id}`),

  createRole: (payload) =>
    axios.post(API_URL, payload),

  updateRole: (id, payload) =>
    axios.put(
      `${API_URL}/${id}`,
      payload
    ),

  deleteRole: (id) =>
    axios.delete(`${API_URL}/${id}`),

  updatePermissions: (payload) =>
    axios.put(
      `${API_URL}/permissions`,
      payload
    ),
};