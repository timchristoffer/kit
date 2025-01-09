import CodeForm from "./components/code-form/code-form";
import FeedbackButton from "./components/feedback-button/feedback-button";

export default function Home() {
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