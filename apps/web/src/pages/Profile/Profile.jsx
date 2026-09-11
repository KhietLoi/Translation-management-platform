import { useEffect, useState, useRef } from "react";
import { useAuth } from "../../contexts/AuthContext";
import {
    getProfileById,
    updateProfile,
    uploadAvatar,
} from "../../services/profileService";
import { toast } from "react-toastify";
import { getAvatarUrl } from "../../utils/blob";
import {
    CameraIcon,
    EnvelopeIcon,
    PhoneIcon,
    MapPinIcon,
    CalendarIcon,
} from "@heroicons/react/24/outline";
import "./Profile.css";

export default function Profile() {
    const { user } = useAuth();
    const fileInputRef = useRef(null);

    const [loading, setLoading] = useState(true);
    const [profile, setProfile] = useState({
        fullName: "",
        phoneNumber: "",
        birthDate: "",
        address: "",
        avatarBlobName: "",
        avatarUrl: "",
    });

    useEffect(() => {
        if (user?.userId) {
            loadProfile();
        }
    }, [user]);

    const loadProfile = async () => {
        if (!user?.userId) return;

        try {
            const response = await getProfileById(user.userId);
            const data = response.data.data;

            setProfile({
                ...data,
                avatarUrl: getAvatarUrl(data.avatarBlobName),
                birthDate: data.birthDate ? data.birthDate.split("T")[0] : "",
            });
        } catch {
            toast.error("Cannot load profile");
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => {
        setProfile({
            ...profile,
            [e.target.name]: e.target.value,
        });
    };

    const handleSave = async () => {
        try {
            await updateProfile(user.userId, {
                fullName: profile.fullName,
                phoneNumber: profile.phoneNumber,
                birthDate: profile.birthDate,
                address: profile.address,
            });
            toast.success("Profile updated successfully");
        } catch {
            toast.error("Update failed");
        }
    };

    const handleAvatarUpload = async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        try {
            await uploadAvatar(user.userId, file);
            toast.success("Avatar updated successfully");
            await loadProfile();
        } catch {
            toast.error("Avatar upload failed");
        }
    };

    const triggerFileInput = () => {
        fileInputRef.current.click();
    };

    if (loading) {
        return (
            <div className="d-flex justify-content-center align-items-center h-100">
                <div className="spinner-border text-warning" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    return (
        <div className="profile-page">
            {/* Banner */}
            <div className="profile-banner" />

            {/* Header: Avatar + Name */}
            <div className="profile-header d-flex flex-column flex-md-row align-items-center align-items-md-end gap-3 gap-md-4">
                <div className="avatar-wrapper">
                    <img
                        src={
                            profile.avatarUrl ||
                            "https://placehold.co/200x200?text=Avatar"
                        }
                        alt="User Avatar"
                        className="avatar-img"
                    />
                    <button
                        type="button"
                        className="btn btn-warning avatar-upload-btn"
                        onClick={triggerFileInput}
                        title="Upload new avatar"
                    >
                        <CameraIcon width={18} height={18} className="text-dark" />
                    </button>
                    <input
                        type="file"
                        className="d-none"
                        ref={fileInputRef}
                        onChange={handleAvatarUpload}
                        accept="image/*"
                    />
                </div>

                <div className="text-center text-md-start pb-2">
                    <h2 className="fw-bold mb-1 text-dark">
                        {profile.fullName || "User Profile"}
                    </h2>
                    <p className="text-muted mb-0 small fw-medium">
                        {user?.roles?.join(", ") || "Administrator"}
                    </p>
                </div>
            </div>

            {/* Content */}
            <div className="profile-content">
                <div className="row g-5">
                    {/* Left - Contact Info */}
                    <div className="col-12 col-lg-4">
                        <div className="section-title">Contact Information</div>

                        <div className="info-list">
                            <div className="info-item">
                                <div className="icon-box">
                                    <EnvelopeIcon width={18} height={18} className="text-secondary" />
                                </div>
                                <div>
                                    <div className="info-label">Email</div>
                                    <div className="info-value">
                                        {user?.email || "Not provided"}
                                    </div>
                                </div>
                            </div>

                            <div className="info-item">
                                <div className="icon-box">
                                    <PhoneIcon width={18} height={18} className="text-secondary" />
                                </div>
                                <div>
                                    <div className="info-label">Phone</div>
                                    <div className="info-value">
                                        {profile.phoneNumber || "Not provided"}
                                    </div>
                                </div>
                            </div>

                            <div className="info-item">
                                <div className="icon-box">
                                    <CalendarIcon width={18} height={18} className="text-secondary" />
                                </div>
                                <div>
                                    <div className="info-label">Birth Date</div>
                                    <div className="info-value">
                                        {profile.birthDate || "Not provided"}
                                    </div>
                                </div>
                            </div>

                            <div className="info-item">
                                <div className="icon-box">
                                    <MapPinIcon width={18} height={18} className="text-secondary" />
                                </div>
                                <div>
                                    <div className="info-label">Address</div>
                                    <div className="info-value">
                                        {profile.address || "Not provided"}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Vertical divider (desktop) */}
                    <div className="col-auto d-none d-lg-flex">
                        <div className="vertical-divider" />
                    </div>

                    {/* Right - Edit Form */}
                    <div className="col-12 col-lg">
                        <div className="section-title">Edit Personal Details</div>

                        <div className="row g-4">
                            <div className="col-md-6">
                                <label className="form-label-custom">Full Name</label>
                                <input
                                    type="text"
                                    name="fullName"
                                    className="form-control form-control-lg"
                                    placeholder="Enter full name"
                                    value={profile.fullName ?? ""}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="col-md-6">
                                <label className="form-label-custom">Phone Number</label>
                                <input
                                    type="tel"
                                    name="phoneNumber"
                                    className="form-control form-control-lg"
                                    placeholder="Enter phone number"
                                    value={profile.phoneNumber ?? ""}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="col-md-6">
                                <label className="form-label-custom">Birth Date</label>
                                <input
                                    type="date"
                                    name="birthDate"
                                    className="form-control form-control-lg"
                                    value={profile.birthDate ?? ""}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="col-12">
                                <label className="form-label-custom">Address</label>
                                <textarea
                                    rows="3"
                                    name="address"
                                    className="form-control form-control-lg"
                                    placeholder="Enter your current address"
                                    value={profile.address ?? ""}
                                    onChange={handleChange}
                                />
                            </div>
                        </div>

                        <div className="mt-5 d-flex justify-content-end">
                            <button
                                type="button"
                                className="btn btn-warning btn-save px-4 py-2 text-dark shadow-sm"
                                onClick={handleSave}
                            >
                                Save Changes
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}