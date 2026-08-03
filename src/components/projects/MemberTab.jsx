import { useEffect, useState } from "react";
import { userService } from "../../services/userService";
import { updateProjectMembers } from "../../services/projectService";
import { toast } from "react-toastify";
import "./MemberTab.css";

function MembersTab({ projectId, members = [], onUpdated }) {
    const [allUsers, setAllUsers] = useState([]);
    const [selectedMembers, setSelectedMembers] = useState([]);
    const [selectedUsers, setSelectedUsers] = useState([]); // Chuyển thành mảng cho multi-select
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        loadUsers();
    }, []);

    useEffect(() => {
        setSelectedMembers(
            members.map((x) => ({
                userId: x.userId || x.id,
                username: x.username,
                email: x.email,
            }))
        );
    }, [members]);

    const loadUsers = async () => {
        try {
            const result = await userService.getUsers(1, 1000);
            const users = result.data?.data?.users || result.data?.users || [];

            setAllUsers(
                users.map((x) => ({
                    id: x.id || x.userId,
                    username: x.username || x.userName,
                    email: x.email,
                }))
            );
        } catch (error) {
            console.error(error);
        }
    };

    const handleAddMember = () => {
        if (selectedUsers.length === 0) return;

        // Lọc ra các user hợp lệ (đã chọn và chưa tồn tại trong project)
        const usersToAdd = allUsers
            .filter((user) => selectedUsers.includes(String(user.id)))
            .filter((user) => !selectedMembers.some((member) => member.userId === user.id));

        if (usersToAdd.length === 0) return;

        const newMemberships = usersToAdd.map((user) => ({
            userId: user.id,
            username: user.username,
            email: user.email,
        }));

        setSelectedMembers((prev) => [...prev, ...newMemberships]);
        setSelectedUsers([]); // Reset form sau khi add
    };

    const removeMember = (userId) => {
        setSelectedMembers((prev) => prev.filter((x) => x.userId !== userId));
    };

    const handleSave = async () => {
        try {
            setSaving(true);
            await updateProjectMembers(
                projectId,
                selectedMembers.map((x) => x.userId)
            );

            if (onUpdated) onUpdated();
            toast.success("Members updated successfully");
        } catch (error) {
            console.error(error);
            toast.error(error?.response?.data?.message || "Update failed");
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="card shadow-sm border-0">
            <div className="card-header bg-white">
                <div className="d-flex justify-content-between align-items-center">
                    <h5 className="mb-0">Project Members</h5>
                    <button
                        className="btn btn-primary"
                        disabled={saving}
                        onClick={handleSave}
                    >
                        {saving ? "Saving..." : "Save Changes"}
                    </button>
                </div>
            </div>

            <div className="card-body">
                {/* Add member section */}
                <div className="row mb-4">
                    <div className="col-md-8">
                        <select
                            multiple
                            className="form-select"
                            value={selectedUsers}
                            onChange={(e) => {
                                const options = Array.from(e.target.selectedOptions);
                                setSelectedUsers(options.map((o) => o.value));
                            }}
                            style={{ height: "150px" }} // Giúp hiển thị danh sách nhiều item hơn
                        >
                            {allUsers
                                .filter(
                                    (user) =>
                                        !selectedMembers.some((member) => member.userId === user.id)
                                )
                                .map((user) => (
                                    <option key={user.id} value={user.id}>
                                        {user.username} - {user.email}
                                    </option>
                                ))}
                        </select>
                        <small className="text-muted mt-1 d-block">
                            * Giữ phím Ctrl (hoặc Cmd trên Mac) và click chuột để chọn nhiều người cùng lúc.
                        </small>
                    </div>

                    <div className="col-md-4 d-flex align-items-start">
                        <button className="btn btn-success" onClick={handleAddMember}>
                            Add Selected Members
                        </button>
                    </div>
                </div>

                {/* Member table */}
                <table className="table table-hover">
                    <thead>
                        <tr>
                            <th>Username</th>
                            <th>Email</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {selectedMembers.map((member) => (
                            <tr key={member.userId}>
                                <td className="align-middle">{member.username}</td>
                                <td className="align-middle">{member.email}</td>
                                <td className="align-middle">
                                    <button
                                        className="btn btn-sm btn-danger"
                                        onClick={() => removeMember(member.userId)}
                                    >
                                        Remove
                                    </button>
                                </td>
                            </tr>
                        ))}
                        {selectedMembers.length === 0 && (
                            <tr>
                                <td colSpan="3" className="text-center text-muted">
                                    No members added yet.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default MembersTab;