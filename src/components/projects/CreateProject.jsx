// src/components/project/CreateProjectModal.jsx
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createProjectFull } from "../../services/projectService";
import { getLanguages } from "../../services/languageService";
import { userService } from "../../services/userService";
import { toast } from "react-toastify";

function CreateProjectModal({ show, onClose, onSuccess }) {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [languages, setLanguages] = useState([]);
    const [users, setUsers] = useState([]);

    const [form, setForm] = useState({
        name: "",
        description: "",
        languageIds: [],
        namespaces: [{ name: "" }],
        members: [],
    });

    // Reset form và load dữ liệu mỗi khi mở Modal
    useEffect(() => {
        if (show) {
            setForm({
                name: "",
                description: "",
                languageIds: [],
                namespaces: [{ name: "" }],
                members: [],
            });
            loadLanguages();
            loadUsers();
        }
    }, [show]);

    // =========================
    // LOAD DATA
    // =========================
    const loadLanguages = async () => {
        try {
            const result = await getLanguages();
            const rawList = result.data?.languages || result.languages || (Array.isArray(result.data) ? result.data : []);
            const items = rawList.map(x => ({
                id: x.languageId || x.id,
                code: x.code,
                name: x.name
            }));
            setLanguages(items);
        } catch (error) {
            console.error(error);
        }
    };

    const loadUsers = async () => {
        try {
            const response = await userService.getUsers(1, 1000);
            const data = response.data?.data?.users || response.data?.users || [];
            setUsers(
                data.map((user) => ({
                    id: user.id || user.userId,
                    username: user.username || user.userName,
                    email: user.email,
                }))
            );
        } catch (error) {
            console.error(error);
        }
    };

    // =========================
    // HANDLERS
    // =========================
    const handleProjectChange = (field, value) => {
        setForm((prev) => ({ ...prev, [field]: value }));
    };

    const toggleLanguage = (id) => {
        setForm((prev) => ({
            ...prev,
            languageIds: prev.languageIds.includes(id)
                ? prev.languageIds.filter((x) => x !== id)
                : [...prev.languageIds, id],
        }));
    };

    const addNamespace = () => {
        setForm((prev) => ({
            ...prev,
            namespaces: [...prev.namespaces, { name: "" }],
        }));
    };

    const changeNamespace = (index, value) => {
        const list = [...form.namespaces];
        list[index].name = value;
        setForm((prev) => ({ ...prev, namespaces: list }));
    };

    const removeNamespace = (index) => {
        setForm((prev) => ({
            ...prev,
            namespaces: prev.namespaces.filter((_, i) => i !== index),
        }));
    };

    const addMember = (userId) => {
        const user = users.find((x) => x.id === userId);
        if (!user) return;
        const exists = form.members.some((x) => x.id === userId);
        if (exists) return;
        setForm((prev) => ({ ...prev, members: [...prev.members, user] }));
    };

    const removeMember = (id) => {
        setForm((prev) => ({
            ...prev,
            members: prev.members.filter((x) => x.id !== id),
        }));
    };

    // =========================
    // SUBMIT
    // =========================
    const handleSubmit = async () => {
        try {
            setLoading(true);

            const payload = {
                name: form.name,
                description: form.description,
                languageIds: form.languageIds,
                memberIds: form.members.map((x) => x.id),
                namespaces: form.namespaces
                    .filter((n) => n.name.trim() !== "")
                    .map((n) => ({ name: n.name.trim() })),
            };

            const projectResponse = await createProjectFull(payload);
            const projectId = projectResponse.data?.id || projectResponse.data || projectResponse.id;

            toast.success("Create project successfully");

            // Gọi callback onSuccess (để parent load lại list) hoặc chuyển hướng
            if (onSuccess) {
                onSuccess(projectId);
            } else {
                navigate(`/projects/${projectId}`);
            }

            onClose(); // Đóng modal
        } catch (error) {
            console.error(error);
            toast.error(error?.response?.data?.message || "Create project failed");
        } finally {
            setLoading(false);
        }
    };

    // Ẩn modal nếu show = false
    if (!show) return null;

    return (
        <div
            className="modal d-block"
            tabIndex="-1"
            style={{ backgroundColor: "rgba(0, 0, 0, 0.5)" }} // Lớp nền đen mờ
        >
            <div className="modal-dialog modal-lg modal-dialog-scrollable">
                <div className="modal-content">

                    {/* HEADER */}
                    <div className="modal-header">
                        <h5 className="modal-title">Create Project</h5>
                        <button
                            type="button"
                            className="btn-close"
                            onClick={onClose}
                            disabled={loading}
                        ></button>
                    </div>

                    {/* BODY */}
                    <div className="modal-body">
                        {/* PROJECT INFO */}
                        <div className="card mb-3">
                            <div className="card-body">
                                <h5>Project Information</h5>
                                <input
                                    className="form-control mb-3"
                                    placeholder="Project name"
                                    value={form.name}
                                    onChange={(e) => handleProjectChange("name", e.target.value)}
                                />
                                <textarea
                                    className="form-control mt-2"
                                    placeholder="Description"
                                    rows="3"
                                    value={form.description}
                                    onChange={(e) => handleProjectChange("description", e.target.value)}
                                />
                            </div>
                        </div>

                        {/* LANGUAGE */}
                        <div className="card mb-3">
                            <div className="card-body">
                                <h5>Languages</h5>
                                <div className="d-flex flex-wrap gap-3 mt-2">
                                    {languages.map((lang) => (
                                        <div key={lang.id} className="form-check">
                                            <input
                                                className="form-check-input"
                                                type="checkbox"
                                                id={`lang-${lang.id}`}
                                                checked={form.languageIds.includes(lang.id)}
                                                onChange={() => toggleLanguage(lang.id)}
                                            />
                                            <label className="form-check-label" htmlFor={`lang-${lang.id}`}>
                                                {lang.name}
                                            </label>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>

                        {/* NAMESPACE */}
                        <div className="card mb-3">
                            <div className="card-body">
                                <h5>Namespaces</h5>
                                {form.namespaces.map((item, index) => (
                                    <div className="d-flex mb-2" key={index}>
                                        <input
                                            className="form-control"
                                            placeholder="Namespace name"
                                            value={item.name}
                                            onChange={(e) => changeNamespace(index, e.target.value)}
                                        />
                                        <button
                                            className="btn btn-outline-danger ms-2"
                                            onClick={() => removeNamespace(index)}
                                        >
                                            X
                                        </button>
                                    </div>
                                ))}
                                <button className="btn btn-sm btn-secondary mt-1" onClick={addNamespace}>
                                    + Add Namespace
                                </button>
                            </div>
                        </div>

                        {/* MEMBER */}
                        <div className="card mb-3">
                            <div className="card-body">
                                <h5>Members</h5>
                                <select
                                    className="form-select"
                                    onChange={(e) => {
                                        addMember(e.target.value);
                                        e.target.value = "";
                                    }}
                                >
                                    <option value="">Select member to add</option>
                                    {users
                                        .filter((user) => !form.members.some((x) => x.id === user.id))
                                        .map((user) => (
                                            <option key={user.id} value={user.id}>
                                                {user.username} - {user.email}
                                            </option>
                                        ))}
                                </select>

                                {form.members.length > 0 && (
                                    <table className="table mt-3 mb-0">
                                        <tbody>
                                            {form.members.map((member) => (
                                                <tr key={member.id}>
                                                    <td className="align-middle">{member.username}</td>
                                                    <td className="align-middle text-muted">{member.email}</td>
                                                    <td className="text-end">
                                                        <button
                                                            className="btn btn-outline-danger btn-sm"
                                                            onClick={() => removeMember(member.id)}
                                                        >
                                                            Remove
                                                        </button>
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* FOOTER */}
                    <div className="modal-footer">
                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={onClose}
                            disabled={loading}
                        >
                            Cancel
                        </button>
                        <button
                            type="button"
                            className="btn btn-primary"
                            disabled={loading || !form.name.trim()}
                            onClick={handleSubmit}
                        >
                            {loading ? "Creating..." : "Create Project"}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    );
}

export default CreateProjectModal;