import TranslationGridRow from "./TranslationGridRow";

function TranslationGrid({ loading, gridData, languages, onEdit, onDelete, onCellClick }) {
    const showActions = Boolean(onEdit || onDelete);
    const colSpan = (languages?.length || 0) + (showActions ? 2 : 1);

    return (
        <div className="card border-0 shadow-sm mb-3">
            <div className="card-body p-0">
                <div className="table-responsive">
                    <table className="table table-bordered table-hover align-middle mb-0">
                        <thead className="table-light">
                            <tr className="table-warning">
                                <th className="py-3 px-3">Translation Key</th>
                                {languages.map(language => (
                                    <th key={language.languageId} className="py-3 px-3">
                                        <span style={{ color: '#a0aabf' }} className="me-2">●</span>
                                        {language.languageCode}
                                    </th>
                                ))}
                                <th className="py-3 text-center px-3" style={{ width: '1%', whiteSpace: 'nowrap' }}>
                                    Status
                                </th>
                                {showActions && (
                                    <th className="py-3 text-center px-3" style={{ width: '1%', whiteSpace: 'nowrap' }}>
                                        Actions
                                    </th>
                                )}
                            </tr>
                        </thead>

                        <tbody>
                            {loading ? (
                                <tr>
                                    <td
                                        colSpan={colSpan}
                                        className="text-center py-4 text-muted"
                                    >
                                        Loading...
                                    </td>
                                </tr>
                            ) : !gridData?.items || gridData.items.length === 0 ? (
                                <tr>
                                    <td
                                        colSpan={colSpan}
                                        className="text-center py-4 text-muted"
                                    >
                                        No translations found
                                    </td>
                                </tr>
                            ) : (
                                gridData.items.map(item => (
                                    <TranslationGridRow
                                        key={item.translationKeyId}
                                        item={item}
                                        languages={languages}
                                        onEdit={onEdit}
                                        onDelete={onDelete}
                                        onCellClick={onCellClick}
                                        showActions={showActions}
                                    />
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default TranslationGrid;