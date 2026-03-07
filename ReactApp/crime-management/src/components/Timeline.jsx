import React from 'react';
import '../styles/Timeline.css';

const Timeline = ({ items }) => {
  return (
    <div className="timeline-container">
      {items.map((item, index) => (
        <div key={index} className="timeline-item">
          <div className="timeline-dot"></div>
          <div className="timeline-content">
            <div className="d-flex justify-content-between align-items-start">
              <h6 className="mb-1 text-primary">
                {item.from} <i className="bi bi-arrow-right mx-1 text-muted"></i> {item.to}
              </h6>
              <span className="small text-muted">{item.date}</span>
            </div>
            <p className="mb-0 small text-secondary">Changed by {item.changedBy}</p>
          </div>
        </div>
      ))}
    </div>
  );
};

export default Timeline;
