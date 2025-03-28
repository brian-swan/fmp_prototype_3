# Feature Management UI

This is a simple React-based UI for managing feature flags that interacts with the Feature Management API.

## Getting Started

### Prerequisites

- Node.js (v16 or later)
- npm 
- Docker and Docker Compose (for containerized deployment)

### Installation

1. Clone the repository
```bash
git clone <repository-url>
cd feature-management-ui
```

2. Install dependencies
```bash
npm install
```

3. Create a `.env` file based on `.env.example` and configure your API URL
```bash
cp .env.example .env
# Edit .env with your API URL
```

### Development

Start the development server:

```bash
npm start
```

The application will be available at http://localhost:3000.

### Testing

Run tests:

```bash
npm test
```

### Building for Production

Build the application for production:

```bash
npm run build
```

### Docker Deployment

Build and run the application using Docker Compose:

```bash
docker-compose up -d
```

This will start both the UI and API containers and make the application available at http://localhost.

## Features

- View all feature flags
- Toggle feature flags on/off
- Create new feature flags
- Edit existing feature flags
- Delete feature flags

## API Integration

This UI is designed to work with the Feature Management API at https://github.com/brian-swan/fmp_prototype_3.

# Feature Management UI - Updated File List

I've added several previously omitted files that are essential for a complete React application:

## Added Public Files
- `public/index.html` - The main HTML template
- `public/manifest.json` - Web app manifest for PWA support
- `public/robots.txt` - Instructions for web crawlers
- `public/favicon.svg` - Vector icon for the application

## Added Source Files
- `src/setupTests.js` - Jest setup for testing
- `src/reportWebVitals.js` - Performance measurement utilities
- `src/App.test.js` - Basic test for the App component
- `src/api/authService.js` - Authentication service (for future expansion)

## File Structure

The complete project structure now looks like this:

```
feature-management-ui/
├── public/
│   ├── index.html
│   ├── favicon.svg
│   ├── manifest.json
│   └── robots.txt
├── src/
│   ├── components/
│   │   ├── FeatureList.jsx
│   │   ├── FeatureItem.jsx
│   │   ├── FeatureForm.jsx
│   │   ├── Header.jsx
│   │   └── Loading.jsx
│   ├── api/
│   │   ├── featureService.js
│   │   └── authService.js
│   ├── tests/
│   │   └── FeatureItem.test.jsx
│   ├── App.jsx
│   ├── App.test.js
│   ├── index.jsx
│   ├── index.css
│   ├── setupTests.js
│   └── reportWebVitals.js
├── Dockerfile
├── docker-compose.yml
├── .env
├── .env.example
├── .gitignore
├── package.json
└── README.md
```

## What's Next

These files complete the basic structure needed for a functional React application. You may want to create actual image files for the logo and favicon, but the SVG version provided will work for now.

For the icons referenced in the manifest.json:
- For a quick start, copy the favicon.svg to favicon.ico
- Create PNG versions of the logo in the sizes 192x192 and 512x512 for logo192.png and logo512.png