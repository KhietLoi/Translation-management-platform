export const getOverallStatus = (values = []) => {

    if (!values.length) {
        return 0;
    }

    const statuses =
        values.map(x => x.status);

    if (statuses.some(x => x === 0)) {
        return 0; // Missing
    }

    if (statuses.some(x => x === 1)) {
        return 1; // Draft
    }

    if (statuses.some(x => x === 3)) {
        return 3; // Rejected
    }

    if (statuses.some(x => x === 2)) {
        return 2; // Translated
    }

    if (statuses.every(x => x === 5)) {
        return 5; // Published
    }

    if (statuses.every(x => x === 4 || x === 5)) {
        return 4; // Reviewed
    }

    return 0;
};