import api from "./api";

// Import Translations (multipart/form-data)
export const importTranslations = async (formData) => {
  const response = await api.post("/TranslationPipeline/import", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
  return response.data;
};

// Export Translations (JSON payload)
export const exportTranslations = async ({ projectId, format = 1 }) => {
  const response = await api.post("/TranslationPipeline/export", {
    projectId,
    format: Number(format),
  });
  return response.data;
};

// Publish Translations
export const publishTranslations = async ({ projectId, notes = "" }) => {
  const response = await api.post("/TranslationPipeline/publish", {
    projectId,
    notes,
  });
  return response.data;
};

// Get Release History
export const getReleaseHistory = async (params) => {
  const response = await api.get("/TranslationPipeline/release-history", {
    params,
  });
  return response.data;
};

// Get Translation Jobs History
export const getTranslationHistory = async (params) => {
  const response = await api.get("/TranslationPipeline/translations-history", {
    params,
  });
  return response.data;
};

// Rollback Release
export const rollbackRelease = async (releaseId) => {
  const response = await api.post(`/TranslationPipeline/releases/${releaseId}/rollback`);
  return response.data;
};

// Get Release Diff
export const getReleaseDiff = async (targetReleaseId) => {
  const response = await api.get("/TranslationPipeline/release-diff", {
    params: {
      targetReleaseId,
    },
  });
  return response.data;
};
