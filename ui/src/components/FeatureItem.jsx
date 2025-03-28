import React from 'react';
import { Link } from 'react-router-dom';

/**
 * Individual feature item component
 * @param {Object} props
 * @param {Object} props.feature - Feature data
 * @param {Function} props.onToggle - Toggle function
 * @param {Function} props.onDelete - Delete function
 */
const FeatureItem = ({ feature, onToggle, onDelete }) => {
  const handleToggle = () => {
    onToggle(feature.id, !feature.enabled);
  };

  const handleDelete = () => {
    if (window.confirm(`Are you sure you want to delete ${feature.name}?`)) {
      onDelete(feature.id);
    }
  };

  return (
    <div className="feature-item">
      <div className="feature-header">
        <h3>{feature.name}</h3>
        <div className="feature-controls">
          <label className="toggle-switch">
            <input 
              type="checkbox" 
              checked={feature.enabled} 
              onChange={handleToggle} 
            />
            <span className="toggle-slider"></span>
          </label>
        </div>
      </div>
      
      <div className="feature-key">
        <code>{feature.key}</code>
      </div>
      
      {feature.description && (
        <p className="feature-description">{feature.description}</p>
      )}
      
      <div className="feature-actions">
        <Link to={`/edit/${feature.id}`} className="btn btn-edit">
          Edit
        </Link>
        <button 
          onClick={handleDelete} 
          className="btn btn-delete"
        >
          Delete
        </button>
      </div>
    </div>
  );
};

export default FeatureItem;