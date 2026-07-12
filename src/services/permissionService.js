import axios from "axios";

const API_URL =
  "http://localhost:5182/api/Permission";

export const permissionService = {
  getPermissions: (
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
  
     getAllPermissions: () =>
    axios.get(API_URL, {
      params: {
        Page: 1,
        Limit: 10000,
      },
    }),


  getPermissionById: (id) =>
    axios.get(`${API_URL}/${id}`),

  createPermission: (data) =>
    axios.post(API_URL, data),

  updatePermission: (
    id,
    data
  ) =>
    axios.put(
      `${API_URL}/${id}`,
      data
    ),

  deletePermission: (id) =>
    axios.delete(
      `${API_URL}/${id}`
    ),
};