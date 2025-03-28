import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getFeature, createFeature, updateFeature } from '../api/featureService';
import Loading from './Loading';

/**
 * Form for creating and editing features
 */
const FeatureForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditing = !!id;

  const [feature, setFeature] = useState({
    name: '',
    key: '',
    description: '',
    enabled: false
  });
  
  const [loading, setLoading] = useState(isEditing);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    // If editing, fetch feature details
    if (isEditing) {
      const fetchFeature = async () => {
        try {
          const data = await getFeature(id);
          setFeature(data);
        } catch (err) {
          setError('Failed to load feature. Please try again.');
          console.error(err);
        } finally {
          setLoading(false);
        }
      };

      fetchFeature();
    }
  }, [id, isEditing]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFeature(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      if (isEditing) {
        await updateFeature(id, feature);
      } else {
        await createFeature(feature);
      }
      
      navigate('/');
    } catch (err) {
      setError('Failed to save feature. Please check form and try again.');
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="feature-form-container">
      <h2>{isEditing ? 'Edit Feature' : 'Create New Feature'}</h2>
      
      {error && (
        <div className="error-message">{error}</div>
      )}
      
      <form onSubmit={handleSubmit} className="feature-form">
        <div className="form-group">
          <label htmlFor="name">Name</label>
          <input
            type="text"
            id="name"
            name="name"
            value={feature.name}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="key">Key</label>
          <input
            type="text"
            id="key"
            name="key"
            value={feature.key}
            onChange={handleChange}
            required
            pattern="[A-Za-z0-9_-]+"
            title="Only letters, numbers, underscores, and hyphens allowed"
          />
          <small>Only letters, numbers, underscores, and hyphens</small>
        </div>

        <div className="form-group">
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            name="description"
            value={feature.description}
            onChange={handleChange}
            rows="3"
          />
        </div>

        <div className="form-group checkbox">
          <label>
            <input
              type="checkbox"
              name="enabled"
              checked={feature.enabled}
              onChange={handleChange}
            />
            Enabled
          </label>
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            onClick={() => navigate('/')}
            className="btn btn-secondary"
            disabled={submitting}
          >
            Cancel
          </button>
          <button 
            type="submit" 
            className="btn btn-primary"
            disabled={submitting}
          >
            {submitting ? 'Saving...' : isEditing ? 'Update Feature' : 'Create Feature'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default FeatureForm;