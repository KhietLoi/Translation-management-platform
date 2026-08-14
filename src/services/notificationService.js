import api from "./api";

/**
 * Notification API Service
 * Handles API calls to ASP.NET Core NotificationController
 */

/**
 * Fetch paginated list of notifications
 * GET /api/notification
 * @param {Object} params - { projectId, pageNumber, pageIndex, pageSize, isRead, type, search }
 */
export const getNotifications = async (params = {}) => {
  const normalizedParams = { ...params };

  // Normalize page number parameters for backend CQRS compatibility
  const currentPage = normalizedParams.pageNumber || normalizedParams.pageIndex || 1;
  normalizedParams.pageNumber = currentPage;
  normalizedParams.PageNumber = currentPage;
  normalizedParams.pageIndex = currentPage;
  normalizedParams.PageIndex = currentPage;

  if (normalizedParams.pageSize) {
    normalizedParams.PageSize = normalizedParams.pageSize;
  }

  if (normalizedParams.projectId) {
    normalizedParams.ProjectId = normalizedParams.projectId;
  }

  const response = await api.get("/notification", { params: normalizedParams });
  return response.data;
};

/**
 * Fetch count of unread notifications
 * GET /api/notification/unread-count
 * @param {string} [projectId]
 */
export const getUnreadCount = async (projectId) => {
  const params = projectId ? { projectId, ProjectId: projectId } : {};
  const response = await api.get("/notification/unread-count", { params });
  return response.data;
};

/**
 * Mark single notification as read
 * PUT /api/notification/{id}/read
 * @param {string} notificationId
 */
export const markNotificationAsRead = async (notificationId) => {
  const response = await api.put(`/notification/${notificationId}/read`);
  return response.data;
};

/**
 * Mark all notifications as read for current user
 * PUT /api/notification/read-all
 */
export const markAllNotificationsAsRead = async () => {
  const response = await api.put("/notification/read-all");
  return response.data;
};
