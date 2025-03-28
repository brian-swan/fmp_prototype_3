import React from 'react';
import { Link } from 'react-router-dom';

/**
 * Simple header component
 */
const Header = () => {
  return (
    <header className="app-header">
      <div className="header-content">
        <h1>Feature Management</h1>
        <nav>
          <ul>
            <li><Link to="/">Features</Link></li>
            <li><Link to="/create">Create Feature</Link></li>
          </ul>
        </nav>
      </div>
    </header>
  );
};

export default Header;