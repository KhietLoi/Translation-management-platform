import TranslationStatusBadge from "./TranslationStatusBadge";
import { getOverallStatus } from "../../helper/calcStatus";

// Hàm helper để map status ra class màu Bootstrap cho viền dọc
const getStatusColor = (statusCode) => {
    switch (Number(statusCode)) {
        case 0: return "danger";   // Missing (Đỏ)
        case 1: return "warning";  // Draft (Vàng)
        case 2: return "success";  // Translated/Reviewed (Xanh)
        case 3: return "secondary"; // Rejected
        default: return "danger";
    }
};

function TranslationGridRow({ item, languages }) {
    const overallStatus = getOverallStatus(item.values);

    return (
        <tr className="bg-white">

            {/* CỘT KEY */}
            <td className="py-3 px-3 align-middle" style={{ width: '25%' }}>
                <div className="fw-semibold text-dark fs-6" style={{ fontFamily: 'monospace' }}>
                    {item.key}
                </div>
                <small className="text-muted opacity-75">
                    {item.namespaceName}
                </small>
            </td>

            {/* CÁC CỘT NGÔN NGỮ */}
            {languages.map((language) => {
                const valueObj = item.values.find(
                    (x) => x.languageId === language.languageId
                );

                // Màu viền (nếu chưa có giá trị sẽ mặc định là đỏ - danger)
                const cellColor = valueObj ? getStatusColor(valueObj.status) : "danger";
                const textValue = valueObj?.value;

                return (
                    <td key={language.languageId} className="py-3 align-middle" style={{ width: '20%' }}>
                        <div className={`border-start border-3 border-${cellColor} ps-3 py-1`}>
                            {textValue ? (
                                <span className="text-dark fs-6" style={{ wordBreak: 'break-word' }}>
                                    {textValue}
                                </span>
                            ) : (
                                <span className="text-muted fst-italic opacity-50">
                                    - chưa dịch -
                                </span>
                            )}
                        </div>
                    </td>
                );
            })}

            {/* CỘT TRẠNG THÁI TỔNG THỂ CUỐI CÙNG */}
            <td className="text-center py-3 align-middle" style={{ width: '120px' }}>
                <TranslationStatusBadge status={overallStatus} />
            </td>

        </tr>
    );
}

export default TranslationGridRow;