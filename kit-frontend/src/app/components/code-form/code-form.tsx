'use client';
import { useState } from 'react';
import axios from 'axios';
import CodeInputForm from '../code-input/code-input';
import AnalysisReport from '../code-analysis/code-analysis';    
import CodeDisplay from '../code-display/code-display';
import CodeDownload from '../code-download/code-download';
import { space_grotesk, lato } from '@/app/styles/fonts';

interface Report {
  reportId: string; // Lägg till reportId här
  complexityScore: number;
  readabilityScore: number;
  securityScore: number;
  performanceScore: number;
  issues: string[];
  securityIssues: string[];
  performanceIssues: string[];
  readabilityIssues: string[];
  explanation: string;
}

interface ApiResponse {
  data: Report;
}

export default function CodeForm() {
  const [report, setReport] = useState<Report | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [code, setCode] = useState('');
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = async (submittedCode: string, file: File | null) => {
    setLoading(true);
    setError(null);
    setCode(submittedCode);
    setSubmitted(true);

    try {
      let response: ApiResponse;

      if (file) {
        const formData = new FormData();
        formData.append('file', file);

        // Logga information om filen som skickas
        console.log('Uploading file:', file);

        // Filuppladdning
        const fileResponse = await axios.post('https://kit-backend.onrender.com/api/files', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });

        if (!fileResponse.data.id) {
          throw new Error('File upload failed: No file ID returned.');
        }

        const fileId = fileResponse.data.id;
        console.log('File uploaded, fileId:', fileId); // Debugging

        // Hämta filens innehåll
        const fileContentResponse = await axios.get(`https://kit-backend.onrender.com/api/files/${fileId}`);
        const fileContentBase64 = fileContentResponse.data.content;
        if (!fileContentBase64) {
          throw new Error('No content found for the uploaded file.');
        }

        const fileContent = atob(fileContentBase64); // Decode base64 content
        console.log('Fetched file content:', fileContent); // Debugging
        setCode(fileContent);

        // Skicka analysbegäran med fileId
        response = await axios.post('https://kit-backend.onrender.com/api/analysis', {
          sourceType: 'file',
          fileId: fileId,
        });
      } else {
        // Skicka begäran med textinnehåll
        response = await axios.post('https://kit-backend.onrender.com/api/analysis', {
          content: submittedCode,
          sourceType: 'text',
        });
      }

      if (!response.data) {
        throw new Error('No data returned from the analysis API.');
      }

      setReport(response.data);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        // Logga detaljer om Axios-fel
        console.error('Axios error:', error.response?.data || error.message);
        setError(error.response?.data?.error || error.message);
      } else {
        // Logga vanliga fel
        console.error('Error:', error);
        setError((error as Error).message);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4">
    <div className="flex items-center justify-center mb-4">
      <h1 className={space_grotesk.className + " text-2xl font-bold select-none"}>Keep It Tidy</h1>
      <div className="">
        <h1 className={lato.className + " transform rotate-12 hover:rotate-0 transition-transform duration-300 text-xl text-red-600 select-none"}>BETA</h1>
      </div>
    </div>
      <div className={`flex flex-col ${code ? 'lg:flex-row' : ''} w-full space-y-4 lg:space-y-0 lg:space-x-4`}>
        <div className={`flex-1 ${code ? '' : 'mx-auto'}`}>
          <CodeInputForm onSubmit={handleSubmit} loading={loading} submitted={submitted} />
        </div>
        {code && (
          <div className="flex-1">
            <CodeDisplay code={code} />
          </div>
        )}
      </div>
      {error && <p className="text-red-500 mt-4">{error}</p>}
      {report ? (
        <div className="w-full">
          <AnalysisReport report={report} />
          <CodeDownload reportId={report.reportId} />
        </div>
      ) : (
        <p className="text-gray-500 mt-4">Remember that this project is a work in progress, any feedback is appreciated</p>
      )}
    </div>
  );
}
