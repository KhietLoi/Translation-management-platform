const BLOB_BASE_URL =
    "https://wmtstorageaccdevsa.blob.core.windows.net/documents/";

export const getAvatarUrl = (blobName) => {

    if (!blobName) {
        return null;
    }

    return `${BLOB_BASE_URL}${blobName}`;
};