'use client';

import { useState } from 'react';
import CodeForm from "./components/code-form/code-form";
import FeedbackButton from "./components/feedback-button/feedback-button";
import LoadingScreen from "./components/loading-screen/loading-screen";

export default function Home() {
  const [loading, setLoading] = useState(true);

  const handleLoaded = () => {
    setLoading(false);
  };

  if (loading) {
    return <LoadingScreen onLoaded={handleLoaded} />;
  }

  return (
    <div className="flex flex-col items-center justify-center min-h-screen font-[family-name:var(--font-geist-sans)] overflow-hidden">
      <main className="flex flex-col items-center justify-center w-full">
        <div className="w-full max-w-6xl p-2">
          <CodeForm />
        </div>
        <div className="w-full max-w-lg p-2">
          <FeedbackButton />
        </div>
      </main>
      <footer className="flex flex-wrap items-center justify-center w-full">
      </footer>
    </div>
  );
}