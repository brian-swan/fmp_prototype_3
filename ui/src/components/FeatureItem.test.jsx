import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import FeatureItem from './FeatureItem';

// Mock window.confirm
window.confirm = jest.fn();

const mockFeature = {
  id: '1',
  name: 'Test Feature',
  key: 'test-feature',
  description: 'This is a test feature',
  enabled: true
};

const renderWithRouter = (ui) => {
  return render(ui, { wrapper: BrowserRouter });
};

describe('FeatureItem Component', () => {
  const mockToggle = jest.fn();
  const mockDelete = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('renders feature details correctly', () => {
    renderWithRouter(
      <FeatureItem 
        feature={mockFeature} 
        onToggle={mockToggle} 
        onDelete={mockDelete} 
      />
    );

    expect(screen.getByText('Test Feature')).toBeInTheDocument();
    expect(screen.getByText('test-feature')).toBeInTheDocument();
    expect(screen.getByText('This is a test feature')).toBeInTheDocument();
    
    // Check if checkbox exists and is checked
    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).toBeInTheDocument();
    expect(checkbox).toBeChecked();
  });

  test('calls onToggle when toggle is clicked', () => {
    renderWithRouter(
      <FeatureItem 
        feature={mockFeature} 
        onToggle={mockToggle} 
        onDelete={mockDelete} 
      />
    );

    const checkbox = screen.getByRole('checkbox');
    fireEvent.click(checkbox);
    
    expect(mockToggle).toHaveBeenCalledWith('1', false);
  });

  test('calls onDelete after confirmation when delete button is clicked', () => {
    window.confirm.mockImplementation(() => true);
    
    renderWithRouter(
      <FeatureItem 
        feature={mockFeature} 
        onToggle={mockToggle} 
        onDelete={mockDelete} 
      />
    );

    const deleteButton = screen.getByText('Delete');
    fireEvent.click(deleteButton);
    
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete Test Feature?');
    expect(mockDelete).toHaveBeenCalledWith('1');
  });

  test('does not call onDelete when confirmation is cancelled', () => {
    window.confirm.mockImplementation(() => false);
    
    renderWithRouter(
      <FeatureItem 
        feature={mockFeature} 
        onToggle={mockToggle} 
        onDelete={mockDelete} 
      />
    );

    const deleteButton = screen.getByText('Delete');
    fireEvent.click(deleteButton);
    
    expect(window.confirm).toHaveBeenCalled();
    expect(mockDelete).not.toHaveBeenCalled();
  });
});