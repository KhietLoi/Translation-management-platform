import React, { useState, useRef } from "react";
import Header from "./components/Header";
import ApiConfigPanel from "./components/ApiConfigPanel";
import TranslationViewer from "./components/TranslationViewer";
import MiniAppPreview from "./components/MiniAppPreview";
import VersionPackageViewer from "./components/VersionPackageViewer";
import NetworkInspector from "./components/NetworkInspector";
import "./App.css";

function App() {
  const [config, setConfig] = useState({
    baseUrl: import.meta.env.VITE_API_URL || "http://localhost:5182",
    apiKey: import.meta.env.VITE_API_KEY || "",
    projectId: import.meta.env.VITE_PROJECT_ID || "01a00d6f-864a-78ed-8bd4-70bcbda5a552"
  });

  const [isLoading, setIsLoading] = useState(false);
  const [networkLogs, setNetworkLogs] = useState([]);
  const [translationState, setTranslationState] = useState({
    translations: null,
    language: "vi-VN",
    version: null
  });

  const translationViewerRef = useRef(null);

  const addNetworkLog = (log) => {
    setNetworkLogs((prev) => [log, ...prev]);
  };

  const clearNetworkLogs = () => {
    setNetworkLogs([]);
  };

  const handleTranslationsLoaded = ({ translations, language, version }) => {
    setTranslationState({
      translations,
      language,
      version
    });
  };

  const handleSwitchLanguage = (langCode) => {
    if (translationViewerRef.current) {
      translationViewerRef.current.loadLanguage(langCode);
    }
  };

  return (
    <div className="app-layout">
      <Header />

      <main className="main-content">
        <ApiConfigPanel
          config={config}
          setConfig={setConfig}
        />

        <TranslationViewer
          ref={translationViewerRef}
          config={config}
          onNetworkLog={addNetworkLog}
          onTranslationsLoaded={handleTranslationsLoaded}
          onLoadingChange={setIsLoading}
        />

        <MiniAppPreview
          translations={translationState.translations}
          language={translationState.language}
          version={translationState.version}
          loading={isLoading}
          onSwitchLanguage={handleSwitchLanguage}
        />

        <VersionPackageViewer
          config={config}
          onNetworkLog={addNetworkLog}
        />

        <NetworkInspector
          logs={networkLogs}
          onClear={clearNetworkLogs}
        />
      </main>

      <footer className="app-footer">
        <p>TMS External React Demo &bull; Powered by MySolution Translation Management Platform</p>
      </footer>
    </div>
  );
}

export default App;
