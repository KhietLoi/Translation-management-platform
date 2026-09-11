import api from "./api";

const API_URL = "/Role";

export const roleService = {

  getRoles: () =>
    api.get(API_URL, {
      params: {
        Page: 1,
        Limit: 1000,
      },
    }),

  getRolesPaging: (
    page = 1,
    limit = 10,
    search = ""
  ) =>
    api.get(API_URL, {
      params: {
        Page: page,
        Limit: limit,
        Search: search,
      },
    }),

  getRoleById: (id) =>
    api.get(`${API_URL}/${id}`),

  createRole: (payload) =>
    api.post(API_URL, payload),

  updateRole: (id, payload) =>
    api.put(
      `${API_URL}/${id}`,
      payload
    ),

  deleteRole: (id) =>
    api.delete(`${API_URL}/${id}`),

  updatePermissions: (payload) =>
    api.put(
      `${API_URL}/permissions`,
      payload
    ),
};