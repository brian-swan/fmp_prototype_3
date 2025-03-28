import { render, screen } from '@testing-library/react';
import App from './App';

// Mock components
jest.mock('./components/Header', () => () => <div data-testid="header">Header</div>);
jest.mock('./components/FeatureList', () => () => <div data-testid="feature-list">Feature List</div>);
jest.mock('./components/FeatureForm', () => () => <div data-testid="feature-form">Feature Form</div>);

// Mock react-router-dom
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  BrowserRouter: ({ children }) => <div>{children}</div>,
  Routes: ({ children }) => <div>{children}</div>,
  Route: () => <div>Route</div>,
}));

test('renders the app with header', () => {
  render(<App />);
  const headerElement = screen.getByTestId('header');
  expect(headerElement).toBeInTheDocument();
});