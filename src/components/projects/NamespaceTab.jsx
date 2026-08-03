import { useEffect, useState } from "react";
import { toast } from "react-toastify";

import {
    createProjectNamespace,
    updateProjectNamespace,
    deleteProjectNamespace
} from "../../services/projectService";

function NamespacesTab({
    projectId,
    namespaces
}) {

    const [items, setItems] = useState([]);
    const [newName, setNewName] = useState("");
    const [editingId, setEditingId] = useState(null);
    const [editingName, setEditingName] = useState("");
    const [saving, setSaving] = useState(false);

    useEffect(() => {

        setItems(namespaces);

    }, [namespaces]);

    const handleCreate = async () => {

        if (!newName.trim()) {
            return;
        }

        try {

            setSaving(true);

            const result =
                await createProjectNamespace(
                    projectId,
                    newName
                );

            const namespace =
                result.data;

            setItems(prev => [

                ...prev,

                {
                    id: namespace.id,
                    name: namespace.name
                }

            ]);

            setNewName("");
            toast.success("Namespace created successfully");

        }
        catch (error) {

            console.error(error);

            toast.error(
                error?.response?.data?.message || "Create namespace failed"
            );

        }
        finally {

            setSaving(false);

        }
    };

    const handleUpdate = async () => {

        if (!editingName.trim()) {
            return;
        }

        try {

            setSaving(true);

            await updateProjectNamespace(
                editingId,
                editingName
            );

            setItems(prev =>
                prev.map(x =>
                    x.id === editingId
                        ? {
                            ...x,
                            name: editingName
                        }
                        : x
                )
            );

            setEditingId(null);
            setEditingName("");
            toast.success("Namespace updated successfully");

        }
        catch (error) {

            console.error(error);

            toast.error(
                error?.response?.data?.message || "Update namespace failed"
            );

        }
        finally {

            setSaving(false);

        }
    };

    const handleDelete = async (id) => {

        const confirmed =
            window.confirm(
                "Delete namespace?"
            );

        if (!confirmed) {
            return;
        }

        try {

            setSaving(true);

            await deleteProjectNamespace(id);

            setItems(prev =>
                prev.filter(x => x.id !== id)
            );

            toast.success("Namespace deleted successfully");

        }
        catch (error) {

            console.error(error);

            toast.error(
                error?.response?.data?.message || "Delete namespace failed"
            );

        }
        finally {

            setSaving(false);

        }
    };

    return (

        <div className="card border-0 shadow-sm">

            <div className="card-header bg-white">

                <div className="d-flex gap-2">

                    <input
                        className="form-control"
                        placeholder="Namespace name"
                        value={newName}
                        onChange={(e) =>
                            setNewName(
                                e.target.value
                            )
                        }
                    />

                    <button
                        className="btn btn-primary"
                        disabled={saving}
                        onClick={handleCreate}
                    >
                        {
                            saving
                                ? "Saving..."
                                : "Add"
                        }
                    </button>

                </div>

            </div>

            <div className="card-body">

                <table className="table align-middle">

                    <thead>

                        <tr>

                            <th>Name</th>

                            <th width="220">
                                Actions
                            </th>

                        </tr>

                    </thead>

                    <tbody>

                        {
                            items.length === 0
                                ? (
                                    <tr>

                                        <td
                                            colSpan="2"
                                            className="text-center text-muted"
                                        >
                                            No namespaces found
                                        </td>

                                    </tr>
                                )
                                : (
                                    items.map(namespace => (

                                        <tr
                                            key={namespace.id}
                                        >

                                            <td>

                                                {
                                                    editingId === namespace.id
                                                        ? (
                                                            <input
                                                                className="form-control"
                                                                value={editingName}
                                                                onChange={(e) =>
                                                                    setEditingName(
                                                                        e.target.value
                                                                    )
                                                                }
                                                            />
                                                        )
                                                        : (
                                                            namespace.name
                                                        )
                                                }

                                            </td>

                                            <td>

                                                {
                                                    editingId === namespace.id
                                                        ? (
                                                            <>
                                                                <button
                                                                    className="btn btn-success btn-sm"
                                                                    disabled={saving}
                                                                    onClick={handleUpdate}
                                                                >
                                                                    Save
                                                                </button>

                                                                <button
                                                                    className="btn btn-secondary btn-sm ms-2"
                                                                    onClick={() => {

                                                                        setEditingId(null);
                                                                        setEditingName("");

                                                                    }}
                                                                >
                                                                    Cancel
                                                                </button>
                                                            </>
                                                        )
                                                        : (
                                                            <>
                                                                <button
                                                                    className="btn btn-warning btn-sm me-2"
                                                                    onClick={() => {

                                                                        setEditingId(
                                                                            namespace.id
                                                                        );

                                                                        setEditingName(
                                                                            namespace.name
                                                                        );

                                                                    }}
                                                                >
                                                                    Edit
                                                                </button>

                                                                <button
                                                                    className="btn btn-danger btn-sm"
                                                                    disabled={saving}
                                                                    onClick={() =>
                                                                        handleDelete(
                                                                            namespace.id
                                                                        )
                                                                    }
                                                                >
                                                                    Delete
                                                                </button>
                                                            </>
                                                        )
                                                }

                                            </td>

                                        </tr>

                                    ))
                                )
                        }

                    </tbody>

                </table>

            </div>

        </div>

    );
}

export default NamespacesTab;