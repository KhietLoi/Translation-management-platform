import React, { useState, useEffect } from "react";

function MiniAppPreview({ translations, language, version, loading, onSwitchLanguage }) {
  const [selectedKey, setSelectedKey] = useState("");
  const [pulse, setPulse] = useState(false);

  // Trigger highlight pulse when translations change
  useEffect(() => {
    if (translations) {
      setPulse(true);
      const timer = setTimeout(() => setPulse(false), 800);
      return () => clearTimeout(timer);
    }
  }, [translations, language]);

  const keys = translations ? Object.keys(translations) : [];

  // Default selected key
  useEffect(() => {
    if (keys.length > 0 && (!selectedKey || !keys.includes(selectedKey))) {
      setSelectedKey(keys[0]);
    }
  }, [translations]);

  // Helper t(key, fallback)
  const t = (key, fallback = "") => {
    if (!translations) return fallback;
    return translations[key] !== undefined ? translations[key] : fallback;
  };

  const languagesList = [
    { code: "vi-VN", label: "Vietnamese (vi-VN)" },
    { code: "en-US", label: "English (en-US)" },
    { code: "ja-JP", label: "Japanese (ja-JP)" },
    { code: "ko-KR", label: "Korean (ko-KR)" },
    { code: "zh-TW", label: "Traditional Chinese (zh-TW)" },
    { code: "fr-FR", label: "French (fr-FR)" },
    { code: "de-DE", label: "German (de-DE)" }
  ];

  return (
    <div className={`card mini-app-card ${pulse ? "pulse-highlight" : ""}`}>
      <div className="card-header">
        <div>
          <h3>User Profile i18n Live Demo</h3>
          <p className="card-subtitle">
            Sample UI component displaying dynamic rendering of <code>profile.*</code> translation keys.
          </p>
        </div>
        {language && (
          <div className="language-badge">
            <span className="lang-active-dot"></span>
            <span>Active: <strong>{language}</strong></span>
          </div>
        )}
      </div>

      {/* QUICK LANGUAGE SWITCHER */}
      <div className="mini-lang-switcher">
        <span className="mini-switcher-label">Select Language:</span>
        <div className="mini-switcher-buttons">
          {languagesList.map((item) => (
            <button
              key={item.code}
              type="button"
              className={`btn-chip ${language === item.code ? "active" : ""}`}
              onClick={() => onSwitchLanguage && onSwitchLanguage(item.code)}
              disabled={Boolean(loading)}
            >
              {item.label}
            </button>
          ))}
        </div>
      </div>

      {!translations ? (
        <div className="mini-app-placeholder">
          <h4>No translation data loaded</h4>
          <p>Click <strong>Load Translations</strong> in the panel above to populate the User Profile preview with live API data.</p>
        </div>
      ) : (
        <div className="mini-app-content">
          {/* USER PROFILE MOCK UI CARD */}
          <div className="profile-ui-shell">
            {/* AVATAR & HEADER SUMMARY */}
            <div className="profile-top-banner">
              <div className="profile-avatar-box">
                <div className="profile-avatar">JD</div>
                <div className="profile-user-details">
                  <h3 className="user-display-name">John Doe</h3>
                  <p className="user-email-text">john.doe@example.com</p>
                  <span className="member-badge">
                    {t("profile.member_since", "Member since January 2025")}
                  </span>
                </div>
              </div>
              <button type="button" className="btn btn-secondary btn-sm">
                {t("profile.edit_profile", "Edit Profile")}
              </button>
            </div>

            <div className="profile-title-bar">
              <h2>{t("profile.title", "User Profile")}</h2>
              <p className="subtitle-desc">
                {t("profile.subtitle", "Manage your personal information and account settings.")}
              </p>
            </div>

            {/* SECTION 1: PERSONAL INFORMATION */}
            <div className="profile-card-section">
              <h4 className="section-title">
                {t("profile.personal_information", "Personal Information")}
              </h4>
              <div className="form-grid-two">
                <div className="form-group">
                  <label>{t("profile.full_name", "Full Name")}</label>
                  <input type="text" className="input-field" defaultValue="John Doe" readOnly />
                </div>
                <div className="form-group">
                  <label>{t("profile.email", "Email Address")}</label>
                  <input type="email" className="input-field" defaultValue="john.doe@example.com" readOnly />
                </div>
                <div className="form-group">
                  <label>{t("profile.phone_number", "Phone Number")}</label>
                  <input type="text" className="input-field" defaultValue="+1 555 123 4567" readOnly />
                </div>
                <div className="form-group">
                  <label>{t("profile.date_of_birth", "Date of Birth")}</label>
                  <input type="text" className="input-field" defaultValue="January 15, 1998" readOnly />
                </div>
              </div>
            </div>

            {/* SECTION 2: PREFERENCES */}
            <div className="profile-card-section">
              <h4 className="section-title">
                {t("profile.preferences", "Preferences")}
              </h4>
              <div className="form-grid-two">
                <div className="form-group">
                  <label>{t("profile.language", "Language")}</label>
                  <select
                    className="input-field"
                    value={language}
                    onChange={(e) => onSwitchLanguage && onSwitchLanguage(e.target.value)}
                  >
                    <option value="vi-VN">Vietnamese (vi-VN)</option>
                    <option value="en-US">English (en-US)</option>
                    <option value="ja-JP">Japanese (ja-JP)</option>
                    <option value="ko-KR">Korean (ko-KR)</option>
                    <option value="zh-TW">Traditional Chinese (zh-TW)</option>
                    <option value="fr-FR">French (fr-FR)</option>
                    <option value="de-DE">German (de-DE)</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>{t("profile.time_zone", "Time Zone")}</label>
                  <input type="text" className="input-field" defaultValue="UTC+07:00 (Indochina Time)" readOnly />
                </div>
              </div>
              <div className="form-group checkbox-field mt-12">
                <label className="checkbox-label">
                  <input type="checkbox" defaultChecked />
                  <span>
                    <strong>{t("profile.notifications", "Notifications")}</strong>: {t("profile.enable_notifications", "Enable notifications")}
                  </span>
                </label>
              </div>
            </div>

            {/* SECTION 3: SECURITY */}
            <div className="profile-card-section">
              <h4 className="section-title">
                {t("profile.security", "Security")}
              </h4>
              <div className="form-grid-two align-items-end">
                <div className="form-group">
                  <label>{t("profile.password", "Password")}</label>
                  <input type="password" className="input-field" defaultValue="supersecret123" readOnly />
                </div>
                <div className="form-group">
                  <button type="button" className="btn btn-secondary">
                    {t("profile.change_password", "Change Password")}
                  </button>
                </div>
              </div>
            </div>

            {/* ACTIONS BAR */}
            <div className="profile-actions-row">
              <button type="button" className="btn btn-primary">
                {t("profile.save_changes", "Save Changes")}
              </button>
              <button type="button" className="btn btn-outline">
                {t("profile.cancel", "Cancel")}
              </button>
            </div>

            {/* DANGER ZONE */}
            <div className="danger-zone-box">
              <h4 className="danger-title">
                {t("profile.danger_zone", "Danger Zone")}
              </h4>
              <p className="danger-desc">
                {t("profile.delete_warning", "Deleting your account is permanent and cannot be undone.")}
              </p>
              <button type="button" className="btn btn-danger">
                {t("profile.delete_account", "Delete Account")}
              </button>
            </div>
          </div>

          {/* DYNAMIC KEY INSPECTOR */}
          <div className="key-inspector-box">
            <h4>Key Inspector ({keys.length} keys loaded)</h4>
            <div className="inspector-controls">
              <div className="form-group flex-1">
                <label>Inspect key value live:</label>
                <select
                  className="input-field code-font"
                  value={selectedKey}
                  onChange={(e) => setSelectedKey(e.target.value)}
                >
                  {keys.map((k) => (
                    <option key={k} value={k}>
                      {k} ({String(translations[k]).substring(0, 30)})
                    </option>
                  ))}
                </select>
              </div>
            </div>

            {selectedKey && (
              <div className="inspector-result-card">
                <div className="inspector-row">
                  <span className="insp-label">Key Name:</span>
                  <code className="insp-key">{selectedKey}</code>
                </div>
                <div className="inspector-row">
                  <span className="insp-label">Translated Value ({language}):</span>
                  <span className="insp-value">{String(translations[selectedKey])}</span>
                </div>
                <div className="inspector-row">
                  <span className="insp-label">Metadata:</span>
                  <span className="insp-meta">
                    {typeof translations[selectedKey]} &bull; {String(translations[selectedKey]).length} chars
                  </span>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

export default MiniAppPreview;
