import * as signalR from "@microsoft/signalr";
import { toast } from "react-toastify";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5182/api";
// Derive hub URL from API_URL (replace /api suffix with /hubs/translation)
const HUB_URL = API_URL.replace(/\/api\/?$/, "") + "/hubs/translation";

class SignalRService {
  connection = null;
  currentProjectId = null;
  listeners = new Set();
  processedIds = new Set();

  async startConnection() {
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => localStorage.getItem("accessToken") || "",
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    // Register NotificationReceived handler (off first to prevent duplicates)
    this.connection.off("NotificationReceived");
    this.connection.on("NotificationReceived", (data) => {
      if (!data) return;

      // Deduplicate by notificationId
      const notifId = data.notificationId || data.NotificationId;
      if (notifId) {
        if (this.processedIds.has(notifId)) {
          return;
        }
        this.processedIds.add(notifId);
        setTimeout(() => this.processedIds.delete(notifId), 10000);
      }

      console.log("🔔 [SignalR] NotificationReceived:", data);

      const title = data.title || data.Title || "";
      const message = data.message || data.Message || "";
      const type = (data.type || data.Type || "Info").toString().toLowerCase();
      const createdBy = data.createdBy || data.CreatedBy;

      const displayMessage = createdBy ? `${message} (by ${createdBy})` : message;

      // Toast handling based on NotificationType
      if (title || message) {
        if (type === "error" || type === "failed" || title.toLowerCase().includes("failed")) {
          toast.error(`${title}: ${displayMessage}`);
        } else if (type === "warning") {
          toast.warning(`${title}: ${displayMessage}`);
        } else if (
          type === "success" ||
          title.toLowerCase().includes("completed") ||
          title.toLowerCase().includes("success")
        ) {
          toast.success(`${title}: ${displayMessage}`);
        } else {
          toast.info(`${title}: ${displayMessage}`);
        }
      }

      // Notify window event listeners for pages (ImportExport, Publish, Management)
      window.dispatchEvent(
        new CustomEvent("translationNotification", { detail: data })
      );

      // Notify registered subscribers
      this.listeners.forEach((callback) => callback(data));
    });

    try {
      await this.connection.start();
      console.log("✅ [SignalR] Connected to TranslationHub");
      if (this.currentProjectId) {
        await this.joinProject(this.currentProjectId);
      }
    } catch (err) {
      console.error("❌ [SignalR] Connection Error:", err);
    }
  }

  async stopConnection() {
    if (this.connection) {
      if (this.currentProjectId) {
        await this.leaveProject(this.currentProjectId);
      }
      await this.connection.stop();
      this.connection = null;
      console.log("⏹️ [SignalR] Connection stopped");
    }
  }

  async joinProject(projectId) {
    if (!projectId) return;
    if (this.currentProjectId && this.currentProjectId !== projectId) {
      await this.leaveProject(this.currentProjectId);
    }
    this.currentProjectId = projectId;

    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke("JoinProject", projectId);
        console.log(`📌 [SignalR] Joined project group: ${projectId}`);
      } catch (err) {
        console.warn("⚠️ [SignalR] JoinProject call notice:", err?.message || err);
      }
    }
  }

  async leaveProject(projectId) {
    if (!projectId) return;
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke("LeaveProject", projectId);
        console.log(`👋 [SignalR] Left project group: ${projectId}`);
      } catch (err) {
        console.warn("⚠️ [SignalR] LeaveProject call notice:", err?.message || err);
      }
    }
    if (this.currentProjectId === projectId) {
      this.currentProjectId = null;
    }
  }

  subscribe(callback) {
    this.listeners.add(callback);
    return () => this.listeners.delete(callback);
  }
}

export const signalRService = new SignalRService();
export default signalRService;
