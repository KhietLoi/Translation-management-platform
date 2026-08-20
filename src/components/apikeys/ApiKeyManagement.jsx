import React, { useEffect, useState, useCallback } from "react";
import { toast } from "react-toastify";
import ApiKeyFilter from "./ApiKeyFilter";
import ApiKeyGrid from "./ApiKeyGrid";
import ApiKeyPagination from "./ApiKeyPagination";
import {
  CreateApplicationModal,
  GenerateApiKeyModal,
  ApiKeyCreatedModal,
  RotateApiKeyModal,
  AssignApiKeyPermissionModal,
  RevokeApiKeyModal,
} from "./ApiKeyModals";

import { getApplications, createApplication } from "../../services/applicationService";
import { getProjects } from "../../services/projectService";
import {
  getApiKeyGrid,
  generateApiKey,
  rotateApiKey,
  revokeApiKey,
  assignApiKeyPermissions,
} from "../../services/apiKeyService";

import "./ApiKeyManagement.css";

function ApiKeyManagement() {
  const [loading, setLoading] = useState(false);
  const [applications, setApplications] = useState([]);
  const [projects, setProjects] = useState([]);

  const [selectedApplicationId, setSelectedApplicationId] = useState("");
  const [envFilter, setEnvFilter] = useState("all");
  const [keyword, setKeyword] = useState("");
  const [isRevoked, setIsRevoked] = useState(false);
  const [page, setPage] = useState(1);
  const [limit, setLimit] = useState(20);

  const [items, setItems] = useState([]);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const [showCreateAppModal, setShowCreateAppModal] = useState(false);
  const [showGenerateKeyModal, setShowGenerateKeyModal] = useState(false);
  const [showCreatedModal, setShowCreatedModal] = useState(false);
  const [generatedData, setGeneratedData] = useState(null);
  const [isRotation, setIsRotation] = useState(false);

  const [showRotateModal, setShowRotateModal] = useState(false);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showRevokeModal, setShowRevokeModal] = useState(false);
  const [selectedApiKey, setSelectedApiKey] = useState(null);

  const loadInitialData = async () => {
    try {
      const [appsRes, projRes] = await Promise.allSettled([
        getApplications(),
        getProjects(),
      ]);

      if (appsRes.status === "fulfilled" && appsRes.value?.data) {
        const rawApps = appsRes.value.data;
        const appList = Array.isArray(rawApps)
          ? rawApps
          : rawApps.applications || rawApps.items || [];
        setApplications(appList);
      } else {
        setApplications([]);
      }

      if (projRes.status === "fulfilled" && projRes.value?.data) {
        const rawProj = projRes.value.data;
        const projList = Array.isArray(rawProj)
          ? rawProj
          : rawProj.projects || rawProj.items || [];
        setProjects(projList);
      } else {
        setProjects([]);
      }
    } catch (err) {
      console.error("Error loading initial dropdown data:", err);
      setApplications([]);
      setProjects([]);
    }
  };

  const loadGrid = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getApiKeyGrid({
        applicationId: selectedApplicationId || null,
        keyword: keyword.trim() || null,
        isRevoked: isRevoked ? true : null,
        page,
        limit,
      });

      const gridData = res?.data || res || {};
      const gridItems = gridData.items || [];
      const paging = gridData.paging || {};

      setItems(gridItems);
      setTotalItems(paging.totalItem ?? gridItems.length);
      setTotalPages(paging.totalPage ?? 1);
    } catch (error) {
      console.error("Error loading API Key grid:", error);
      setItems([]);
      setTotalItems(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [selectedApplicationId, keyword, isRevoked, page, limit]);

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    loadGrid();
  }, [loadGrid]);

  const handleCreateApplication = async (payload) => {
    try {
      const res = await createApplication(payload);
      toast.success("New application registered successfully!");
      setShowCreateAppModal(false);
      await loadInitialData();
      if (res?.data?.id) {
        setSelectedApplicationId(res.data.id);
      }
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Failed to register application."
      );
    }
  };

  const handleGenerateApiKey = async (appId, payload) => {
    try {
      const res = await generateApiKey(appId, payload);
      toast.success("API Key generated successfully!");
      setShowGenerateKeyModal(false);

      if (res?.data) {
        setGeneratedData(res.data);
        setIsRotation(false);
        setShowCreatedModal(true);
      }

      await loadGrid();
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Failed to generate API Key."
      );
    }
  };

  const handleConfirmRotate = async (apiKeyId) => {
    try {
      const res = await rotateApiKey(apiKeyId);
      toast.success("API Key rotated successfully!");
      setShowRotateModal(false);

      if (res?.data) {
        setGeneratedData(res.data);
        setIsRotation(true);
        setShowCreatedModal(true);
      }

      await loadGrid();
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Failed to rotate API Key."
      );
    }
  };

  const handleAssignPermissions = async (apiKeyId, permissions) => {
    try {
      await assignApiKeyPermissions(apiKeyId, permissions);
      toast.success("Permissions updated successfully!");
      setShowAssignModal(false);
      setSelectedApiKey(null);
      await loadGrid();
    } catch (error) {
      console.error(error);
      toast.error("Failed to update permissions.");
    }
  };

  const handleConfirmRevoke = async (apiKeyId) => {
    try {
      await revokeApiKey(apiKeyId);
      toast.success("API Key revoked successfully!");
      setShowRevokeModal(false);
      setSelectedApiKey(null);
      await loadGrid();
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Failed to revoke API Key."
      );
    }
  };

  return (
    <div className="apikey-page-wrapper p-3 p-md-4">
      <ApiKeyFilter
        applications={applications}
        selectedApplicationId={selectedApplicationId}
        onApplicationChange={(appId) => {
          setSelectedApplicationId(appId);
          setPage(1);
        }}
        envFilter={envFilter}
        onEnvFilterChange={setEnvFilter}
        keyword={keyword}
        onKeywordChange={setKeyword}
        isRevoked={isRevoked}
        onIsRevokedChange={(rev) => {
          setIsRevoked(rev);
          setPage(1);
        }}
        onSearch={() => {
          setPage(1);
          loadGrid();
        }}
        onOpenCreateAppModal={() => setShowCreateAppModal(true)}
        onOpenGenerateKeyModal={() => setShowGenerateKeyModal(true)}
      />

      <ApiKeyGrid
        loading={loading}
        items={items}
        onRotate={(item) => {
          setSelectedApiKey(item);
          setShowRotateModal(true);
        }}
        onRevoke={(item) => {
          setSelectedApiKey(item);
          setShowRevokeModal(true);
        }}
        onAssignPermissions={(item) => {
          setSelectedApiKey(item);
          setShowAssignModal(true);
        }}
      />

      <ApiKeyPagination
        page={page}
        totalPages={totalPages}
        totalItems={totalItems}
        limit={limit}
        onPageChange={setPage}
        onLimitChange={(l) => {
          setLimit(l);
          setPage(1);
        }}
      />

      <CreateApplicationModal
        show={showCreateAppModal}
        projects={projects}
        onClose={() => setShowCreateAppModal(false)}
        onSubmit={handleCreateApplication}
      />

      <GenerateApiKeyModal
        show={showGenerateKeyModal}
        applications={applications}
        defaultApplicationId={selectedApplicationId}
        onClose={() => setShowGenerateKeyModal(false)}
        onSubmit={handleGenerateApiKey}
      />

      <ApiKeyCreatedModal
        show={showCreatedModal}
        generatedData={generatedData}
        isRotation={isRotation}
        onClose={() => {
          setShowCreatedModal(false);
          setGeneratedData(null);
        }}
      />

      <RotateApiKeyModal
        show={showRotateModal}
        item={selectedApiKey}
        onClose={() => {
          setShowRotateModal(false);
          setSelectedApiKey(null);
        }}
        onConfirm={handleConfirmRotate}
      />

      <AssignApiKeyPermissionModal
        show={showAssignModal}
        item={selectedApiKey}
        onClose={() => {
          setShowAssignModal(false);
          setSelectedApiKey(null);
        }}
        onSubmit={handleAssignPermissions}
      />

      <RevokeApiKeyModal
        show={showRevokeModal}
        item={selectedApiKey}
        onClose={() => {
          setShowRevokeModal(false);
          setSelectedApiKey(null);
        }}
        onConfirm={handleConfirmRevoke}
      />
    </div>
  );
}

export default ApiKeyManagement;
