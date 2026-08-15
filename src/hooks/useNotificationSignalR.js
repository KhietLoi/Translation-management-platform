import { useEffect, useState, useCallback, useRef } from "react";
import { useQueryClient } from "@tanstack/react-query";
import signalRService from "../services/signalrService";
import { NOTIFICATION_KEYS } from "./useNotifications";

/**
 * Hook to handle real-time SignalR notification events with ZERO LATENCY.
 * Ensures EXACTLY-ONCE processing per notification to prevent double-counting.
 */
export function useNotificationSignalR() {
  const queryClient = useQueryClient();
  const [hasNewNotificationAnimation, setHasNewNotificationAnimation] = useState(false);
  const processedIdsRef = useRef(new Set());

  const handleIncomingNotification = useCallback(
    (data) => {
      if (!data) return;

      // Extract ID & deduplicate to prevent double-incrementing count
      const id = data.id || data.NotificationId || data.notificationId || `temp-${Date.now()}`;
      if (processedIdsRef.current.has(id)) {
        console.log("[useNotificationSignalR] Duplicate notification skipped:", id);
        return;
      }
      processedIdsRef.current.add(id);
      setTimeout(() => processedIdsRef.current.delete(id), 10000);

      console.log("[useNotificationSignalR] ⚡ Real-time notification received (Exactly once):", data);

      const title = data.title || data.Title || "Thông báo mới";
      const message = data.message || data.Message || "";
      const type = data.type ?? data.Type ?? "Info";
      const navigationUrl = data.navigationUrl || data.NavigationUrl || null;
      const createdAt = data.createdAt || data.CreatedAt || new Date().toISOString();
      const createdBy = data.createdBy || data.CreatedBy || data.triggeredByUserName;

      const newNotification = {
        id,
        title,
        message,
        type,
        isRead: false,
        navigationUrl,
        createdAt,
        createdBy,
        isNew: true, // Tag for entrance animation
      };

      // 1. INCREMENT UNREAD COUNT BADGE BY EXACTLY 1 (+1)
      queryClient.setQueriesData(
        { queryKey: ["notifications", "unread-count"] },
        (oldData) => {
          const currentCount = typeof oldData === "number" ? oldData : 0;
          return currentCount + 1;
        }
      );

      // 2. PREPEND NEW NOTIFICATION TO LISTS EXACTLY ONCE
      queryClient.setQueriesData(
        { queryKey: NOTIFICATION_KEYS.all },
        (oldData) => {
          if (!oldData) return oldData;

          // Skip unread count queries here (which are numbers) since step 1 handled it
          if (typeof oldData === "number") {
            return oldData;
          }

          // Update infinite query pages
          if (oldData.pages) {
            const updatedPages = [...oldData.pages];
            if (updatedPages.length > 0) {
              const firstPage = updatedPages[0];
              const exists = firstPage.items.some((i) => i.id === newNotification.id);
              if (!exists) {
                updatedPages[0] = {
                  ...firstPage,
                  items: [newNotification, ...firstPage.items],
                  totalCount: (firstPage.totalCount || 0) + 1,
                };
              }
            } else {
              updatedPages.push({
                items: [newNotification],
                pageIndex: 1,
                totalPages: 1,
                totalCount: 1,
                hasNextPage: false,
              });
            }
            return { ...oldData, pages: updatedPages };
          }

          // Update standard list items array
          if (Array.isArray(oldData.items)) {
            const exists = oldData.items.some((i) => i.id === newNotification.id);
            if (exists) return oldData;
            return {
              ...oldData,
              items: [newNotification, ...oldData.items],
              totalCount: (oldData.totalCount || 0) + 1,
            };
          }

          if (Array.isArray(oldData)) {
            const exists = oldData.some((i) => i.id === newNotification.id);
            if (exists) return oldData;
            return [newNotification, ...oldData];
          }

          return oldData;
        }
      );

      // 3. Mark queries as stale for smooth background refetch without UI flash
      queryClient.invalidateQueries({ queryKey: NOTIFICATION_KEYS.all, refetchType: "none" });

      // 4. Trigger Bell icon shake & badge animation
      setHasNewNotificationAnimation(true);
      const timer = setTimeout(() => {
        setHasNewNotificationAnimation(false);
      }, 3500);

      return () => clearTimeout(timer);
    },
    [queryClient]
  );

  useEffect(() => {
    // Subscribe ONLY to signalRService.subscribe callback (avoids double listening via window events)
    const unsubscribeSignalR = signalRService.subscribe(handleIncomingNotification);
    return () => {
      unsubscribeSignalR();
    };
  }, [handleIncomingNotification]);

  return {
    hasNewNotificationAnimation,
  };
}
