import { useEffect, useState } from "react";
import signalRService from "../services/signalrService";

export function useTranslationLocks() {
  const [locks, setLocks] = useState({});

  useEffect(() => {
    const unsubscribe = signalRService.subscribeLock((event) => {
      if (!event) return;

      if (event.type === "LOCKED") {
        const { lockInfo } = event;
        if (lockInfo?.translationValueId || lockInfo?.TranslationValueId) {
          const valId = lockInfo.translationValueId || lockInfo.TranslationValueId;
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
          }));
        }
      } else if (event.type === "UNLOCKED") {
        const valId =
          typeof event.translationValueId === "string"
            ? event.translationValueId
            : event.translationValueId?.translationValueId || event.translationValueId?.TranslationValueId;

        if (valId) {
          setLocks((prev) => {
            const next = { ...prev };
            delete next[valId];
            return next;
          });
        }
      } else if (event.type === "LOCK_FAILED") {
        const { existingLock } = event;
        if (existingLock?.translationValueId || existingLock?.TranslationValueId) {
          const valId = existingLock.translationValueId || existingLock.TranslationValueId;
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
