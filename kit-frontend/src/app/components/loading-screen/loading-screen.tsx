'use client';

import { useEffect } from 'react';

interface LoadingScreenProps {
  onLoaded: () => void;
}

export default function LoadingScreen({ onLoaded }: LoadingScreenProps) {
  useEffect(() => {
    const timer = setTimeout(() => {
      onLoaded();
    }, 3000); // Simulate a 3-second loading time
    return () => clearTimeout(timer);
  }, [onLoaded]);

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-900">
      <div className="loader">
        <div className="spinner"></div>
        <div className="pulse"></div>
      </div>
      <style jsx>{`
        .loader {
          position: relative;
          width: 100px;
          height: 100px;
        }
        .spinner {
          box-sizing: border-box;
          position: absolute;
          width: 100px;
          height: 100px;
          border: 8px solid #3498db;
          border-top: 8px solid transparent;
          border-radius: 50%;
          animation: spin 1s linear infinite;
        }
        .pulse {
          position: absolute;
          width: 100px;
          height: 100px;
          background-color: rgba(52, 152, 219, 0.5);
          border-radius: 50%;
          animation: pulse 2s infinite;
        }
        @keyframes spin {
          0% {
            transform: rotate(0deg);
          }
          100% {
            transform: rotate(360deg);
          }
        }
        @keyframes pulse {
          0% {
            transform: scale(0.9);
            opacity: 1;
          }
          100% {
            transform: scale(1.5);
            opacity: 0;
          }
        }
      `}</style>
    </div>
  );
}