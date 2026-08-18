import * as signalR from "@microsoft/signalr";
import { toast } from "react-toastify";

const API_URL = import.meta.env.VITE_API_URL;
console.log(API_URL, "env");
// Derive hub URL from API_URL (replace /api suffix with /hubs/translation)
const HUB_URL = API_URL.replace(/\/api\/?$/, "") + "/hubs/translation";

// Helper regex to validate Guid format before sending to C# Hub
const GUID_REGEX = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
export function isValidGuid(id) {
  return typeof id === "string" && GUID_REGEX.test(id);
}

/**
 * Decode user ID and username directly from JWT access token if user object is incomplete
 */
export function getAuthUser(user) {
  let userId = user?.id || user?.userId || user?.sub;
  let username = user?.userName || user?.username || user?.name;

  if (isValidGuid(userId) && username) {
    return { userId, username };
  }

  const token = localStorage.getItem("accessToken");
  if (token) {
    try {
      const payloadBase64 = token.split(".")[1];
      if (payloadBase64) {
        const decodedJson = atob(payloadBase64.replace(/-/g, "+").replace(/_/g, "/"));
        const payload = JSON.parse(decodedJson);
        const tokenUserId =
          payload.sub ||
          payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
          payload.nameidentifier ||
          payload.id;
        const tokenUsername =
          payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
          payload.name ||
          payload.username ||
          "User";

        if (isValidGuid(tokenUserId)) {
          return {
            userId: tokenUserId,
            username: username || tokenUsername,
          };
        }
      }
    } catch (e) {
      console.warn("[Auth] Failed to decode JWT token payload:", e);
    }
  }

  return isValidGuid(userId) ? { userId, username: username || "User" } : null;
}

class SignalRService {
  connection = null;
  joinedProjectIds = new Set();
  listeners = new Set();
  presenceListeners = new Set();
  lockListeners = new Set();
  publishProgressListeners = new Set();
  typingListeners = new Set();
  processedIds = new Set();
  startPromise = null;

  async startConnection() {
    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return;
    }

    if (this.startPromise) {
      return this.startPromise;
    }

    this.startPromise = (async () => {
      if (!this.connection) {
        this.connection = new signalR.HubConnectionBuilder()
          .withUrl(HUB_URL, {
            accessTokenFactory: () => localStorage.getItem("accessToken") || "",
            skipNegotiation: false,
            transport:
              signalR.HttpTransportType.WebSockets |
              signalR.HttpTransportType.LongPolling,
          })
          .withAutomaticReconnect()
          .configureLogging(signalR.LogLevel.Information)
          .build();

        // Handle auto-reconnect event
        this.connection.onreconnected(async (connectionId) => {
          console.log("[SignalR] Reconnected to Hub. ConnectionId:", connectionId);
        });

        // 1. Notification Received Handler
        const handleSignalRNotification = (data) => {
          if (!data) return;

          console.log("[SignalR] Incoming notification raw payload:", data);

          const notifId = data.notificationId || data.NotificationId || data.id;
          if (notifId) {
            if (this.processedIds.has(notifId)) {
              console.log("[SignalR] Duplicate notification ignored:", notifId);
              return;
            }
            this.processedIds.add(notifId);
            setTimeout(() => this.processedIds.delete(notifId), 10000);
          }

          const title = data.title || data.Title || "";
          const message = data.message || data.Message || "";
          const type = (data.type ?? data.Type ?? "Info")
            .toString()
            .toLowerCase();
          const createdBy =
            data.createdBy || data.CreatedBy || data.triggeredByUserName;

          const displayMessage = createdBy ? `${message} (bởi ${createdBy})` : message;

          if (title || message) {
            if (
              type === "error" ||
              type === "failed" ||
              title.toLowerCase().includes("failed")
            ) {
              toast.error(`${title}: ${displayMessage}`);
            } else if (type === "warning") {
              toast.warning(`${title}: ${displayMessage}`);
            } else if (
              type === "success" ||
              type === "2" ||
              title.toLowerCase().includes("completed") ||
              title.toLowerCase().includes("success")
            ) {
              toast.success(`${title}: ${displayMessage}`);
            } else {
              toast.info(`${title}: ${displayMessage}`);
            }
          }

          window.dispatchEvent(
            new CustomEvent("translationNotification", { detail: data })
          );

          this.listeners.forEach((callback) => {
            try {
              callback(data);
            } catch (err) {
              console.error("[SignalR] Error in notification callback:", err);
            }
          });
        };

        const eventNames = [
          "NotificationReceived",
          "ReceiveNotification",
          "notificationReceived",
          "ImportCompleted",
          "ExportCompleted",
          "PublishCompleted",
          "Notification",
          "ReceiveMessage",
        ];

        eventNames.forEach((evt) => {
          this.connection.off(evt);
          this.connection.on(evt, handleSignalRNotification);
        });

        // 2. Presence Event Handler: OnlineUsersUpdated
        this.connection.off("OnlineUsersUpdated");
        this.connection.on("OnlineUsersUpdated", (onlineUsers) => {
          console.log("[SignalR] OnlineUsersUpdated received:", onlineUsers);
          this.presenceListeners.forEach((cb) => {
            try {
              cb(onlineUsers);
            } catch (err) {
              console.error("[SignalR] Error in presence callback:", err);
            }
          });
        });

        // 3. Translation Lock Event Handlers
        this.connection.off("TranslationLocked");
        this.connection.on("TranslationLocked", (lockInfo) => {
          console.log("[SignalR] TranslationLocked received:", lockInfo);
          this.lockListeners.forEach((cb) => {
            try {
              cb({ type: "LOCKED", lockInfo });
            } catch (err) {
              console.error("[SignalR] Error in lock callback:", err);
            }
          });
        });

        this.connection.off("TranslationUnlocked");
        this.connection.on("TranslationUnlocked", (translationValueId) => {
          console.log("[SignalR] TranslationUnlocked received:", translationValueId);
          this.lockListeners.forEach((cb) => {
            try {
              cb({ type: "UNLOCKED", translationValueId });
            } catch (err) {
              console.error("[SignalR] Error in lock callback:", err);
            }
          });
        });

        this.connection.off("LockFailed");
        this.connection.on("LockFailed", (existingLock) => {
          console.log("[SignalR] LockFailed received:", existingLock);
          this.lockListeners.forEach((cb) => {
            try {
              cb({ type: "LOCK_FAILED", existingLock });
            } catch (err) {
              console.error("[SignalR] Error in lock callback:", err);
            }
          });
        });

        // 4. Publish Progress Event Handler
        const handlePublishProgress = (progressInfo) => {
          console.log("[SignalR] PublishProgress received:", progressInfo);
          this.publishProgressListeners.forEach((cb) => {
            try {
              cb(progressInfo);
            } catch (err) {
              console.error("[SignalR] Error in publish progress callback:", err);
            }
          });
        };

        const progressEvents = [
          "PublishProgress",
          "ReceivePublishProgress",
          "publishProgress",
          "ReceivePublishProgressAsync"
        ];

        progressEvents.forEach((evt) => {
          this.connection.off(evt);
          this.connection.on(evt, handlePublishProgress);
        });

        // 5. Typing Event Handler
        this.connection.off("UserTyping");
        this.connection.on("UserTyping", (...args) => {
          let translationValueId = null;
          let userId, username, value;
          if (args.length === 4) {
            [translationValueId, userId, username, value] = args;
          } else {
            [userId, username, value] = args;
          }
          console.log("[SignalR] UserTyping received:", { translationValueId, userId, username, value });
          this.typingListeners.forEach((cb) => {
            try {
              cb({ translationValueId, userId, username, value });
            } catch (err) {
              console.error("[SignalR] Error in typing callback:", err);
            }
          });
        });
      }

      if (
        this.connection.state === signalR.HubConnectionState.Disconnected
      ) {
        try {
          await this.connection.start();
          console.log("[SignalR] Connected successfully to TranslationHub");
        } catch (err) {
          if (
            err?.name === "AbortError" ||
            err?.message?.includes("stopped during negotiation")
          ) {
            console.log(
              "[SignalR] Connection start aborted during negotiation (React StrictMode reset)"
            );
          } else {
            console.error("[SignalR] Connection Error:", err);
          }
        }
      }
    })();

    try {
      await this.startPromise;
    } finally {
      this.startPromise = null;
    }
  }

  async stopConnection() {
    if (this.startPromise) {
      try {
        await this.startPromise;
      } catch {
        // Safe catch
      }
    }

    if (this.connection) {
      if (
        this.connection.state !== signalR.HubConnectionState.Disconnected
      ) {
        try {
          await this.connection.stop();
          console.log("[SignalR] Connection stopped gracefully");
        } catch (err) {
          console.warn("[SignalR] Stop connection notice:", err?.message || err);
        }
      }
      this.connection = null;
    }
  }

  /**
   * Join project on SignalR Hub matching C# TranslationHub:
   * public async Task JoinProject(Guid projectId, Guid userId, string username)
   */
  async joinProject(projectId, userId, username) {
    if (!isValidGuid(projectId) || !isValidGuid(userId)) {
      console.warn("[SignalR] joinProject skipped: invalid projectId or userId", { projectId, userId });
      return;
    }

    if (this.startPromise) {
      await this.startPromise;
    } else if (
      !this.connection ||
      this.connection.state === signalR.HubConnectionState.Disconnected
    ) {
      await this.startConnection();
    }

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        const uName = username || "User";
        await this.connection.invoke("JoinProject", projectId, userId, uName);
        this.joinedProjectIds.add(projectId);
        console.log(`[SignalR] Joined project group: ${projectId} as user ${uName}`);
      } catch (err) {
        if (!err?.message?.includes("connection being closed") && !err?.message?.includes("error on close")) {
          console.warn(`[SignalR] JoinProject invoke warning for ${projectId}:`, err?.message || err);
        }
      }
    }
  }

  async leaveProject(projectId) {
    if (!isValidGuid(projectId)) return;
    this.joinedProjectIds.delete(projectId);

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke("LeaveProject", projectId);
        console.log(`[SignalR] Left project group: ${projectId}`);
      } catch (err) {
        console.warn("[SignalR] LeaveProject call notice:", err?.message || err);
      }
    }
  }

  /**
   * Acquire lock on a translation value matching C# TranslationHub:
   * public async Task AcquireLock(Guid translationValueId, Guid userId, string username)
   */
  async acquireLock(translationValueId, userId, username) {
    if (!isValidGuid(translationValueId) || !isValidGuid(userId)) {
      console.log("[SignalR] acquireLock skipped: invalid translationValueId or userId", { translationValueId, userId });
      return;
    }

    if (
      !this.connection ||
      this.connection.state !== signalR.HubConnectionState.Connected
    ) {
      await this.startConnection();
    }

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        const uName = username || "User";
        await this.connection.invoke("AcquireLock", translationValueId, userId, uName);
        console.log(`[SignalR] Requested lock for translation value: ${translationValueId}`);
      } catch (err) {
        console.warn("[SignalR] AcquireLock invoke error:", err?.message || err);
      }
    }
  }

  /**
   * Release lock on a translation value matching C# TranslationHub:
   * public async Task ReleaseLock(Guid translationValueId, Guid userId)
   */
  async releaseLock(translationValueId, userId) {
    if (!isValidGuid(translationValueId) || !isValidGuid(userId)) return;

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke("ReleaseLock", translationValueId, userId);
        console.log(`[SignalR] Released lock for translation value: ${translationValueId}`);
      } catch (err) {
        console.warn("[SignalR] ReleaseLock invoke error:", err?.message || err);
      }
    }
  }

  async joinTranslationValueGroup(translationValueId) {
    if (!isValidGuid(translationValueId)) return;

    if (
      !this.connection ||
      this.connection.state !== signalR.HubConnectionState.Connected
    ) {
      await this.startConnection();
    }

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke("JoinTranslationValueGroup", translationValueId);
        console.log(`[SignalR] Joined translation value group: ${translationValueId}`);
      } catch (err) {
        console.warn("[SignalR] JoinTranslationValueGroup invoke error:", err?.message || err);
      }
    }
  }

  async leaveTranslationValueGroup(translationValueId) {
    if (!isValidGuid(translationValueId)) return;

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke("LeaveTranslationValueGroup", translationValueId);
        console.log(`[SignalR] Left translation value group: ${translationValueId}`);
      } catch (err) {
        console.warn("[SignalR] LeaveTranslationValueGroup invoke error:", err?.message || err);
      }
    }
  }

  async sendTyping(translationValueId, value) {
    if (!isValidGuid(translationValueId)) return;

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke("Typing", translationValueId, value);
        console.log(`[SignalR] Sent typing status for translation value: ${translationValueId}`);
      } catch (err) {
        console.warn("[SignalR] Typing invoke error:", err?.message || err);
      }
    }
  }

  subscribe(callback) {
    this.listeners.add(callback);
    return () => this.listeners.delete(callback);
  }

  subscribePresence(callback) {
    this.presenceListeners.add(callback);
    return () => this.presenceListeners.delete(callback);
  }

  subscribeLock(callback) {
    this.lockListeners.add(callback);
    return () => this.lockListeners.delete(callback);
  }

  subscribePublishProgress(callback) {
    this.publishProgressListeners.add(callback);
    if (!this.connection || this.connection.state === signalR.HubConnectionState.Disconnected) {
      this.startConnection();
    }
    return () => this.publishProgressListeners.delete(callback);
  }

  subscribeTyping(callback) {
    this.typingListeners.add(callback);
    return () => this.typingListeners.delete(callback);
  }
}

export const signalRService = new SignalRService();
export default signalRService;
