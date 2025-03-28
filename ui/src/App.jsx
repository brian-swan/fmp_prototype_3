import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import FeatureList from './components/FeatureList';
import FeatureForm from './components/FeatureForm';

function App() {
  return (
    <Router>
      <div className="app">
        <Header />
        <main className="content">
          <Routes>
            <Route path="/" element={<FeatureList />} />
            <Route path="/create" element={<FeatureForm />} />
            <Route path="/edit/:id" element={<FeatureForm />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;