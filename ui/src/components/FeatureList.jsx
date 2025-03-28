import React, { useState, useEffect } from 'react';
import { getFeatures, toggleFeature, deleteFeature } from '../api/featureService';
import FeatureItem from './FeatureItem';
import Loading from './Loading';

/**
 * List of features component
 */
const FeatureList = () => {
  const [features, setFeatures] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchFeatures();
  }, []);

  const fetchFeatures = async () => {
    try {
      setLoading(true);
      const data = await getFeatures();
      setFeatures(data);
      setError(null);
    } catch (err) {
      setError('Failed to load features. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleToggle = async (id, enabled) => {
    try {
      await toggleFeature(id, enabled);
      setFeatures(features.map(feature => 
        feature.id === id ? { ...feature, enabled } : feature
      ));
    } catch (err) {
      setError('Failed to update feature status. Please try again.');
      console.error(err);
    }
  };

  const handleDelete = async (id) => {
    try {
      await deleteFeature(id);
      setFeatures(features.filter(feature => feature.id !== id));
    } catch (err) {
      setError('Failed to delete feature. Please try again.');
      console.error(err);
    }
  };

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="feature-list">
      {error && (
        <div className="error-message">{error}</div>
      )}
      
      {features.length === 0 ? (
        <div className="empty-state">
          <p>No features found. Create your first feature to get started.</p>
        </div>
      ) : (
        features.map(feature => (
          <FeatureItem 
            key={feature.id}
            feature={feature}
            onToggle={handleToggle}
            onDelete={handleDelete}
          />
        ))
      )}
    </div>
  );
};

export default FeatureList;