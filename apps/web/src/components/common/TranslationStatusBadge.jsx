import { getStatusText, getStatusBadgeClass } from "../../utils/translationStatus";

function TranslationStatusBadge({ status }) {

    return (
        <span
            className={`badge ${getStatusBadgeClass(status)}`}
        >
            {getStatusText(status)}
        </span>
    );
}

export default TranslationStatusBadge;