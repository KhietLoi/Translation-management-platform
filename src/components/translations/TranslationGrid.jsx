import TranslationGridRow from "./TranslationGridRow";

function TranslationGrid({ loading, gridData, languages }) {
    return (
        <div className="card border-0 shadow-sm mb-3">
            <div className="card-body p-0">
                <div className="table-responsive">
                    <table className="table table-bordered table-hover align-middle mb-0">
                        <thead className="table-light">
                            <tr>
                                <th className="py-3 px-3">Translation Key</th>
                                {languages.map(language => (
                                    <th key={language.languageId} className="py-3 px-3">
                                        <span style={{ color: '#a0aabf' }} className="me-2">●</span>
                                        {language.languageCode}
                                    </th>
                                ))}
                                <th className="py-3 text-center px-3" style={{ width: '120px' }}>
                                    Status
                                </th>
                            </tr>
                        </thead>

                        <tbody>
                            {loading ? (
                                <tr>
                                    <td
                                        colSpan={languages.length + 2}
                                        className="text-center py-4 text-muted"
                                    >
                                        Loading...
                                    </td>
                                </tr>
                            ) : !gridData?.items || gridData.items.length === 0 ? (
                                <tr>
                                    <td
                                        colSpan={languages.length + 2}
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