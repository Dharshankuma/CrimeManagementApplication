import React from 'react';

const StatusBadge = ({ status }) => {
  const getBadgeClass = () => {
    switch (status) {
      case 'New': return 'bg-info text-dark';
      case 'Open': return 'bg-primary';
      case 'Under Investigation': return 'bg-warning text-dark';
      case 'Resolved': return 'bg-success';
      case 'Closed': return 'bg-secondary';
      case 'Closed - No Action': return 'bg-danger';
      default: return 'bg-light text-dark';
    }
  };

  return (
    <span className={`badge rounded-pill ${getBadgeClass()}`} style={{ fontWeight: 500, padding: '0.5em 0.8em' }}>
      {status}
    </span>
  );
};

export default StatusBadge;
