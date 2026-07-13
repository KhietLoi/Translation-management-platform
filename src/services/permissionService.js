import api from "./api";

const API_URL = "/Permission";

export const permissionService = {

  getPermissions: (
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


  getAllPermissions: () =>
    api.get(API_URL, {
      params: {
        Page: 1,
        Limit: 10000,
      },
    }),


  getPermissionById: (id) =>
    api.get(`${API_URL}/${id}`),


  createPermission: (data) =>
    api.post(API_URL, data),


  updatePermission: (id, data) =>
    api.put(
      `${API_URL}/${id}`,
      data
    ),


  deletePermission: (id) =>
    api.delete(
      `${API_URL}/${id}`
    ),
};