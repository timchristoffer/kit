'use client';
import { useState } from 'react';

interface CodeInputFormProps {
  onSubmit: (code: string, file: File | null) => void;
  loading: boolean;
  submitted: boolean;
}

export default function CodeInputForm({ onSubmit, loading, submitted }: CodeInputFormProps) {
  const [code, setCode] = useState('');
  const [file, setFile] = useState<File | null>(null);

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onSubmit(code, file);
  };

  return (
    <form
      onSubmit={handleSubmit}
      className={`space-y-4 p-2 md:p-4 bg-slate-900 rounded-md max-w-full mx-auto transition-all duration-300 ${
        submitted ? 'md:max-w-md' : 'md:max-w-2xl'
      }`}
    >
      <textarea
        value={code}
        onChange={(e) => setCode(e.target.value)}
        rows={12}  // Adjust the number of rows based on the submitted state
        className="w-full p-2 border border-gray-300 rounded-md text-[#142d55] h-64 md:h-auto" // Adjust height
        placeholder="Paste your code here"
      />
      <input
        type="file"
        onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
        className="block w-full text-sm text-[#ffffff] file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-[#142d55] hover:file:bg-blue-100"
      />
      <button
        type="submit"
        disabled={loading}
        className={`w-full py-2 px-4 rounded-md text-white ${loading ? 'bg-gray-400' : 'bg-[#142d55] hover:bg-[#1e3a8a]'}`}
      >
        {loading ? 'Analyzing...' : 'Analyze'}
      </button>
    </form>
  );
}