import { useQuery, useInfiniteQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getNotifications,
  getUnreadCount,
  markNotificationAsRead,
  markAllNotificationsAsRead,
  getNotificationDetail,
} from "../services/notificationService";

export const NOTIFICATION_KEYS = {
  all: ["notifications"],
  unreadCount: (projectId) => ["notifications", "unread-count", projectId || "all"],
  list: (filters) => ["notifications", "list", filters?.projectId || "all", filters],
  infinite: (filters) => ["notifications", "infinite", filters?.projectId || "all", filters],
};

/**
 * Hook to fetch unread notification count
 */
export function useUnreadCountQuery(projectId) {
  return useQuery({
    queryKey: NOTIFICATION_KEYS.unreadCount(projectId),
    queryFn: async () => {
      const res = await getUnreadCount(projectId);
      console.log("[useUnreadCountQuery] Unread count server response:", res);

      if (typeof res === "number") return res;
      if (typeof res?.data === "number") return res.data;
      if (typeof res?.count === "number") return res.count;
      if (typeof res?.unreadCount === "number") return res.unreadCount;
      if (typeof res?.data?.count === "number") return res.data.count;
      if (typeof res?.data?.unreadCount === "number") return res.data.unreadCount;
      if (typeof res?.data?.totalCount === "number") return res.data.totalCount;

      if (Array.isArray(res?.data)) {
        return res.data.filter((n) => !n.isRead).length;
      }
      if (Array.isArray(res)) {
        return res.filter((n) => !n.isRead).length;
      }

      return res?.count ?? res?.unreadCount ?? 0;
    },
    staleTime: 1000 * 30, // 30 seconds
    refetchOnWindowFocus: true,
  });
}

/**
 * Hook for standard paginated notification list (Page-by-page)
 */
export function useNotificationsQuery(params = {}) {
  return useQuery({
    queryKey: NOTIFICATION_KEYS.list(params),
    queryFn: async () => {
      const res = await getNotifications(params);
      const data = res?.data ?? res ?? {};
      const rawItems = Array.isArray(data.notifications)
        ? data.notifications
        : Array.isArray(data.items)
        ? data.items
        : Array.isArray(data)
        ? data
        : [];

      // Deduplicate by ID
      const items = Array.from(
        new Map(rawItems.filter((i) => i && i.id).map((i) => [i.id, i])).values()
      );

      const totalCount = data.totalCount ?? items.length;
      const pageSize = data.pageSize || params.pageSize || 15;
      const pageNumber = data.pageNumber || params.pageNumber || params.pageIndex || 1;
      const totalPages =
        data.totalPages || (totalCount > 0 ? Math.ceil(totalCount / pageSize) : 1);

      return {
        items,
        totalCount,
        pageNumber,
        pageSize,
        totalPages,
      };
    },
    staleTime: 1000 * 30,
  });
}

/**
 * Hook for infinite scroll notification list
 */
export function useNotificationsInfiniteQuery(filters = {}) {
  const pageSize = filters.pageSize || 10;

  return useInfiniteQuery({
    queryKey: NOTIFICATION_KEYS.infinite(filters),
    queryFn: async ({ pageParam = 1 }) => {
      const params = {
        pageNumber: pageParam,
        pageIndex: pageParam,
        pageSize,
        ...filters,
      };
      if (filters.projectId) {
        params.ProjectId = filters.projectId;
      }
      const res = await getNotifications(params);
      const data = res?.data ?? res ?? {};

      // Handle items list formats & deduplicate
      const rawItems = Array.isArray(data.notifications)
        ? data.notifications
        : Array.isArray(data.items)
        ? data.items
        : Array.isArray(data)
        ? data
        : [];

      const items = Array.from(
        new Map(rawItems.filter((i) => i && i.id).map((i) => [i.id, i])).values()
      );

      const totalCount = data.totalCount ?? items.length;
      const actualPageSize = data.pageSize || pageSize;
      const totalPages =
        data.totalPages || (totalCount > 0 ? Math.ceil(totalCount / actualPageSize) : 1);
      const currentPage = data.pageNumber || pageParam;
      const hasNextPage = currentPage < totalPages || data.hasNextPage === true;

      return {
        items,
        pageIndex: currentPage,
        totalPages,
        totalCount,
        hasNextPage,
      };
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) => {
      if (lastPage.hasNextPage) {
        return lastPage.pageIndex + 1;
      }
      return undefined;
    },
    staleTime: 1000 * 30,
  });
}

/**
 * Hook to mark single notification as read
 */
export function useMarkAsReadMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (notificationId) => markNotificationAsRead(notificationId),
    onSuccess: (_, notificationId) => {
      // 1. Instantly decrement unread count badge (-1)
      queryClient.setQueriesData(
        { queryKey: ["notifications", "unread-count"] },
        (oldData) => {
          const currentCount = typeof oldData === "number" ? oldData : 1;
          return Math.max(0, currentCount - 1);
        }
      );

      // 2. Mark item as isRead: true in cached notification lists
      queryClient.setQueriesData(
        { queryKey: NOTIFICATION_KEYS.all },
        (oldData) => {
          if (typeof oldData === "number" || !oldData) return oldData;

          if (oldData.pages) {
            return {
              ...oldData,
              pages: oldData.pages.map((page) => ({
                ...page,
                items: page.items.map((item) =>
                  item.id === notificationId ? { ...item, isRead: true } : item
                ),
              })),
            };
          }

          if (Array.isArray(oldData.items)) {
            return {
              ...oldData,
              items: oldData.items.map((item) =>
                item.id === notificationId ? { ...item, isRead: true } : item
              ),
            };
          }

          if (Array.isArray(oldData)) {
            return oldData.map((item) =>
              item.id === notificationId ? { ...item, isRead: true } : item
            );
          }

          return oldData;
        }
      );

      // 3. Invalidate queries in background to sync with backend
      queryClient.invalidateQueries({ queryKey: NOTIFICATION_KEYS.all });
    },
  });
}

/**
 * Hook to mark all notifications as read
 */
export function useMarkAllAsReadMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => markAllNotificationsAsRead(),
    onSuccess: () => {
      // 1. Instantly clear unread count badge to 0
      queryClient.setQueriesData(
        { queryKey: ["notifications", "unread-count"] },
        () => 0
      );

      // 2. Mark all items as isRead: true in cached notification lists
      queryClient.setQueriesData(
        { queryKey: NOTIFICATION_KEYS.all },
        (oldData) => {
          if (typeof oldData === "number") return 0;
          if (!oldData) return oldData;

          if (oldData.pages) {
            return {
              ...oldData,
              pages: oldData.pages.map((page) => ({
                ...page,
                items: page.items.map((item) => ({ ...item, isRead: true })),
              })),
            };
          }

          if (Array.isArray(oldData.items)) {
            return {
              ...oldData,
              items: oldData.items.map((item) => ({ ...item, isRead: true })),
            };
          }

          if (Array.isArray(oldData)) {
            return oldData.map((item) => ({ ...item, isRead: true }));
          }

          return oldData;
        }
      );

      // 3. Invalidate queries in background to sync with backend
      queryClient.invalidateQueries({ queryKey: NOTIFICATION_KEYS.all });
    },
  });
}

/**
 * Hook to retrieve a single notification's full details
 */
export function useNotificationDetailQuery(notificationId) {
  return useQuery({
    queryKey: ["notifications", "detail", notificationId],
    queryFn: async () => {
      if (!notificationId) return null;
      const res = await getNotificationDetail(notificationId);
      return res?.data ?? res;
    },
    enabled: !!notificationId,
    staleTime: 1000 * 30, // 30 seconds
  });
}

