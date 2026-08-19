import { useEffect, useState } from "react";
import signalRService from "../services/signalrService";

export function useTranslationLocks() {
  const [locks, setLocks] = useState({});

  useEffect(() => {
    const unsubscribe = signalRService.subscribeLock((event) => {
      if (!event) return;

      if (event.type === "LOCKED") {
        const { lockInfo } = event;
        const rawId = lockInfo?.translationValueId || lockInfo?.TranslationValueId || lockInfo?.id || lockInfo?.Id;
        if (rawId) {
          const valId = rawId.toString().toLowerCase();
          const uId = lockInfo.userId || lockInfo.UserId;
          const uName = lockInfo.username || lockInfo.Username || lockInfo.userName;

          setLocks((prev) => ({
            ...prev,
            [valId]: {
              translationValueId: valId,
              userId: uId,
              username: uName,
              lockedAt: lockInfo.lockedAt || lockInfo.LockedAt,
            },
            [rawId]: {
              translationValueId: valId,
              userId: uId,
              username: uName,
              lockedAt: lockInfo.lockedAt || lockInfo.LockedAt,
            },
          }));
        }
      } else if (event.type === "UNLOCKED") {
        let rawVal = null;
        if (typeof event.translationValueId === "string") {
          rawVal = event.translationValueId;
        } else if (typeof event.translationValueId === "object" && event.translationValueId !== null) {
          rawVal = event.translationValueId.translationValueId ||
                   event.translationValueId.TranslationValueId ||
                   event.translationValueId.id ||
                   event.translationValueId.Id;
        } else if (event.id || event.Id) {
          rawVal = event.id || event.Id;
        }

        if (rawVal) {
          const lowerVal = rawVal.toString().toLowerCase();
          setLocks((prev) => {
            const next = { ...prev };
            delete next[lowerVal];
            delete next[rawVal];
            return next;
          });
        }
      } else if (event.type === "LOCK_FAILED") {
        const { existingLock } = event;
        const rawId = existingLock?.translationValueId || existingLock?.TranslationValueId || existingLock?.id || existingLock?.Id;
        if (rawId) {
          const valId = rawId.toString().toLowerCase();
          const uId = existingLock.userId || existingLock.UserId;
          const uName = existingLock.username || existingLock.Username;

          setLocks((prev) => ({
            ...prev,
            [valId]: {
              translationValueId: valId,
              userId: uId,
              username: uName,
              isFailedLock: true,
            },
            [rawId]: {
              translationValueId: valId,
              userId: uId,
              username: uName,
              isFailedLock: true,
            },
          }));
        }
      }
    });

    return () => {
      unsubscribe();
    };
  }, []);

  return { locks };
}
