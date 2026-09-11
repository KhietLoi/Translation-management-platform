function TranslationPagination({
    pageNumber,
    totalPages,
    onPrevious,
    onNext
}) {

    return (

        <div
            className="
                d-flex
                justify-content-end
                mt-3
                gap-2
            "
        >

            <button
                className="btn btn-outline-secondary"
                disabled={pageNumber === 1}
                onClick={onPrevious}
            >
                Previous
            </button>

            <span
                className="align-self-center"
            >
                {pageNumber} / {totalPages || 1}
            </span>

            <button
                className="btn btn-outline-secondary"
                disabled={pageNumber >= totalPages}
                onClick={onNext}
            >
                Next
            </button>

        </div>

    );
}

export default TranslationPagination;