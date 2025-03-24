const nextJest = require('next/jest')

const createJestConfig = nextJest({
  // Sökväg till din Next.js app för att ladda next.config.js och .env-filer
  dir: './',
})

// Jest-konfiguration
const customJestConfig = {
  setupFilesAfterEnv: ['<rootDir>/jest.setup.js'],
  testEnvironment: 'jest-environment-jsdom',
  moduleNameMapper: {
    // Hantera modulalias, om du använder dem i din Next.js-app
    '^@/components/(.*)$': '<rootDir>/components/$1',
    '^@/pages/(.*)$': '<rootDir>/pages/$1',
  },
  testPathIgnorePatterns: ['<rootDir>/node_modules/', '<rootDir>/.next/'],
}

// createJestConfig exporterar en Next.js-specific konfiguration som automatiskt
// konfigurerar babel, behandlar next.config.js, etc.
module.exports = createJestConfig(customJestConfig)