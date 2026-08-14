import * as signalR from "@microsoft/signalr";
import { toast } from "react-toastify";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5182/api";
// Derive hub URL from API_URL (replace /api suffix with /hubs/translation)
const HUB_URL = API_URL.replace(/\/api\/?$/, "") + "/hubs/translation";

class SignalRService {
  connection = null;
  joinedProjectIds = new Set();
  listeners = new Set();
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
          console.log("[SignalR] Reconnected to Hub. Re-joining all project groups...", connectionId);
          if (this.joinedProjectIds.size > 0) {
            await this.joinProjects(Array.from(this.joinedProjectIds));
          }
        });

        const handleSignalRNotification = (data) => {
          if (!data) return;

          console.log("[SignalR] 🔥 Incoming notification raw payload:", data);

          // Deduplicate by notificationId
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

          // Toast notification handling
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

          // Notify window custom event listeners
          window.dispatchEvent(
            new CustomEvent("translationNotification", { detail: data })
          );

          // Notify registered subscriber hooks
          this.listeners.forEach((callback) => {
            try {
              callback(data);
            } catch (err) {
              console.error("[SignalR] Error in subscriber callback:", err);
            }
          });
        };

        // Register handlers for various event casing conventions & hub method names
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
      }

      if (
        this.connection.state === signalR.HubConnectionState.Disconnected
      ) {
        try {
          await this.connection.start();
          console.log("[SignalR] Connected successfully to TranslationHub");
          if (this.joinedProjectIds.size > 0) {
            await this.joinProjects(Array.from(this.joinedProjectIds));
          }
        } catch (err) {
          if (
            err?.name === "AbortError" ||
            err?.message?.includes("stopped during negotiation")
          ) {
            console.log(
              "[SignalR] Connection start aborted during negotiation (React StrictMode mount reset)"
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
      } catch (e) {
        // Safe catch on aborted connection
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
   * Join multiple project groups on SignalR server so notifications
   * for all user's projects arrive real-time regardless of current active project
   * @param {string[]|string} projectIds
   */
  async joinProjects(projectIds) {
    const ids = Array.isArray(projectIds) ? projectIds : [projectIds];
    const validIds = ids.filter(Boolean);
    if (validIds.length === 0) return;

    validIds.forEach((id) => this.joinedProjectIds.add(id));

    if (this.startPromise) {
      await this.startPromise;
    } else if (!this.connection || this.connection.state === signalR.HubConnectionState.Disconnected) {
      await this.startConnection();
    }

    if (
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      for (const projectId of validIds) {
        try {
          await this.connection.invoke("JoinProject", projectId);
          console.log(`[SignalR] ✅ Joined project group: ${projectId}`);
        } catch (err) {
          console.warn(`[SignalR] ⚠️ JoinProject invoke warning for ${projectId}:`, err?.message || err);
        }
      }
    }
  }

  async joinProject(projectId) {
    if (!projectId) return;
    await this.joinProjects([projectId]);
  }

  async leaveProject(projectId) {
    if (!projectId) return;
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

  subscribe(callback) {
    this.listeners.add(callback);
    return () => this.listeners.delete(callback);
  }
}

export const signalRService = new SignalRService();
export default signalRService;
