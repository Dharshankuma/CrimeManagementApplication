import React from 'react';

const Pagination = ({
  currentPage, // 1-based index
  pageSize,
  totalCount,
  onPageChange,
}) => {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const startItem = totalCount > 0 ? (currentPage - 1) * pageSize + 1 : 0;
  const endItem = Math.min(currentPage * pageSize, totalCount);

  // Generate page numbers to display
  const getPageNumbers = () => {
    const pages = [];
    // Show up to 5 page numbers (e.g., current, and 2 before/after if available)
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, startPage + 4);

    if (endPage - startPage < 4) {
      startPage = Math.max(1, endPage - 4);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  };

  const pageNumbers = getPageNumbers();

  return (
    <div className="card-footer bg-white border-0 py-3 d-flex flex-column flex-md-row justify-content-between align-items-center">
      <div className="small text-muted mb-2 mb-md-0">
        Showing {startItem}–{endItem} of {totalCount} entries
      </div>
      <nav>
        <ul className="pagination pagination-sm justify-content-center mb-0">
          <li className={`page-item ${currentPage <= 1 ? 'disabled' : ''}`}>
            <button
              className="page-link"
              onClick={() => onPageChange(Math.max(1, currentPage - 1))}
              disabled={currentPage <= 1}
            >
              Previous
            </button>
          </li>

          {pageNumbers.map(page => (
            <li key={page} className={`page-item ${currentPage === page ? 'active' : ''}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(page)}
              >
                {page}
              </button>
            </li>
          ))}

          <li className={`page-item ${currentPage >= totalPages ? 'disabled' : ''}`}>
            <button
              className="page-link"
              onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
              disabled={currentPage >= totalPages}
            >
              Next
            </button>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Pagination;
