import axios from 'axios';

// Get API URL from environment variable or use default
const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Create axios instance
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json, text/plain, */*'
  }
});

/**
 * Get all features
 * @returns {Promise<Array>} Features array
 */
export const getFeatures = async () => {
  try {
    const response = await api.get('/FeatureFlags');
    return response.data;
  } catch (error) {
    console.error('Error fetching features:', error);
    throw error;
  }
};

/**
 * Get a single feature by ID
 * @param {string} id - Feature ID
 * @returns {Promise<Object>} Feature object
 */
export const getFeature = async (id) => {
  try {
    const response = await api.get(`/FeatureFlags/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching feature ${id}:`, error);
    throw error;
  }
};

/**
 * Create a new feature
 * @param {Object} featureData - Feature data
 * @returns {Promise<Object>} Created feature
 */
export const createFeature = async (featureData) => {
  try {
    const response = await api.post('/FeatureFlags', featureData);
    return response.data;
  } catch (error) {
    console.error('Error creating feature:', error);
    throw error;
  }
};

/**
 * Update an existing feature
 * @param {string} id - Feature ID
 * @param {Object} featureData - Updated feature data
 * @returns {Promise<Object>} Updated feature
 */
export const updateFeature = async (id, featureData) => {
  try {
    const response = await api.put(`/FeatureFlags/${id}`, featureData);
    return response.data;
  } catch (error) {
    console.error(`Error updating feature ${id}:`, error);
    throw error;
  }
};

/**
 * Delete a feature
 * @param {string} id - Feature ID
 * @returns {Promise<void>}
 */
export const deleteFeature = async (id) => {
  try {
    await api.delete(`/FeatureFlags/${id}`);
    return true;
  } catch (error) {
    console.error(`Error deleting feature ${id}:`, error);
    throw error;
  }
};

/**
 * Toggle feature on/off
 * @param {string} id - Feature ID
 * @param {boolean} enabled - Whether to enable or disable
 * @returns {Promise<Object>} Updated feature
 */
export const toggleFeature = async (id, enabled) => {
  try {
    // Since your API might not have a specific toggle endpoint,
    // we'll get the current feature, update its enabled status, and save it
    const feature = await getFeature(id);
    feature.enabled = enabled;
    const response = await updateFeature(id, feature);
    return response;
  } catch (error) {
    console.error(`Error toggling feature ${id}:`, error);
    throw error;
  }
};