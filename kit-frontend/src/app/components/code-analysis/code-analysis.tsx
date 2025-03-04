import { useState } from 'react';
import { Bar } from 'react-chartjs-2';
import { Chart, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { ChevronDownIcon, ChevronUpIcon, InformationCircleIcon, XMarkIcon } from '@heroicons/react/24/solid';
import { lato } from '@/app/styles/fonts';

Chart.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

interface AnalysisReportProps {
  report: {
    complexityScore: number;
    readabilityScore: number;
    securityScore: number;
    performanceScore: number;
    issues: string[];
    securityIssues: string[];
    performanceIssues: string[];
    readabilityIssues: string[];
    // explanation: string; // Lägg till detta fält
  };
}

export default function AnalysisReport({ report }: AnalysisReportProps) {
  const [isIssuesVisible, setIsIssuesVisible] = useState(false);
  const [isModalVisible, setIsModalVisible] = useState(false);

  const toggleIssuesVisibility = () => {
    setIsIssuesVisible(!isIssuesVisible);
  };

  const toggleModalVisibility = () => {
    setIsModalVisible(!isModalVisible);
  };

  const handleCloseModal = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      setIsModalVisible(false);
    }
  };

  const chartData = {
    labels: ['Complexity', 'Readability', 'Security', 'Performance'],
    datasets: [
      {
        label: 'Scores',
        data: [
          report.complexityScore,
          report.readabilityScore,
          report.securityScore,
          report.performanceScore,
        ],
        backgroundColor: [
          'rgba(255, 99, 132, 0.2)',
          'rgba(54, 162, 235, 0.2)',
          'rgba(255, 206, 86, 0.2)',
          'rgba(75, 192, 192, 0.2)',
        ],
        borderColor: [
          'rgba(255, 99, 132, 1)',
          'rgba(54, 162, 235, 1)',
          'rgba(255, 206, 86, 1)',
          'rgba(75, 192, 192, 1)',
        ],
        borderWidth: 1,
      },
    ],
  };

  // const explanationSections = report.explanation.split(';').reduce((acc, item) => {
  //   const [key, value] = item.split(':').map(str => str.trim());
  //   if (key && value) {
  //     acc[key] = value;
  //   }
  //   return acc;
  // }, {} as Record<string, string>);

  return (
    <div className={lato.className + " p-4 bg-slate-900 mt-4 rounded-md shadow-md"}>
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-xl font-bold text-white">Analysis Report</h2>
        <button onClick={toggleModalVisibility} className="text-white hover:text-blue-800 focus:outline-none">
          <InformationCircleIcon className="w-8 h-8" />
        </button>
      </div>
      <div className="relative w-full h-64 md:h-96">
        <Bar data={chartData} options={{ maintainAspectRatio: false }} />
      </div>
      <div className="mt-4">
        <button
          onClick={toggleIssuesVisibility}
          className="flex items-center text-lg font-semibold text-blue-600 hover:text-blue-800 focus:outline-none"
        >
          {isIssuesVisible ? 'Hide Issues' : 'Show Issues'}
          {isIssuesVisible ? (
            <ChevronUpIcon className="w-5 h-5 ml-2" />
          ) : (
            <ChevronDownIcon className="w-5 h-5 ml-2" />
          )}
        </button>
        {isIssuesVisible && (
          <div className="mt-2 text-white mt-4 p-4 bg-gray-800 rounded-md shadow-md">
            {report.issues.length > 0 && (
              <div>
                <h3 className="font-semibold">General Issues:</h3>
                <ul className="list-disc list-inside">
                  {report.issues.map((issue, index) => (
                    <li key={index}>{issue}</li>
                  ))}
                </ul>
              </div>
            )}
            {report.securityIssues.length > 0 && (
              <div>
                <h3 className="font-semibold">Security Issues:</h3>
                <ul className="list-disc list-inside">
                  {report.securityIssues.map((issue, index) => (
                    <li key={index}>{issue}</li>
                  ))}
                </ul>
              </div>
            )}
            {report.performanceIssues.length > 0 && (
              <div>
                <h3 className="font-semibold">Performance Issues:</h3>
                <ul className="list-disc list-inside">
                  {report.performanceIssues.map((issue, index) => (
                    <li key={index}>{issue}</li>
                  ))}
                </ul>
              </div>
            )}
            {report.readabilityIssues.length > 0 && (
              <div>
                <h3 className="font-semibold">Readability Issues:</h3>
                <ul className="list-disc list-inside">
                  {report.readabilityIssues.map((issue, index) => (
                    <li key={index}>{issue}</li>
                  ))}
                </ul>
              </div>
            )}
            {report.issues.length === 0 &&
              report.securityIssues.length === 0 &&
              report.performanceIssues.length === 0 &&
              report.readabilityIssues.length === 0 && (
                <p>No issues found</p>
              )}
          </div>
        )}
      </div>

      {isModalVisible && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50" onClick={handleCloseModal}>
          <div className="bg-white p-6 rounded-md shadow-md max-w-md w-full relative">
            <button onClick={toggleModalVisibility} className="absolute top-2 right-2 text-gray-600 hover:text-gray-800 focus:outline-none">
              <XMarkIcon className="w-6 h-6" />
            </button>
            <h3 className="text-xl font-bold mb-4" style={{ color: '#142d55' }}>Score Information</h3>
            <ul className="list-disc list-inside space-y-2" style={{ color: '#142d55' }}>
              <li><strong>Readability Score:</strong> A score of 100 means the code is highly readable, with good naming conventions, proper indentation, comments, and no long lines or deeply nested structures.</li>
              <li><strong>Security Score:</strong> A score of 100 means the code is secure, with no hardcoded passwords, insecure cryptographic practices, or potential vulnerabilities like SQL injection or XSS.</li>
              <li><strong>Performance Score:</strong> A score of 100 means the code is performant, with no inefficient loops, excessive memory usage, or unnecessary object creation.</li>
              <li><strong>Complexity Score:</strong> A score of 100 means the code has low complexity, with low cyclomatic complexity, a manageable number of functions, and shallow inheritance.</li>
            </ul>
          </div>
        </div>
      )}
      {/* Lägg till detta för att visa Explanation */}
      {/* {report.explanation && (
        <div className="mt-4 p-4 bg-gray-800 rounded-md shadow-md">
          <h3 className="text-lg font-semibold text-white">Explanation:</h3>
          {Object.entries(explanationSections).map(([key, value]) => (
            <div key={key} className="mb-2">
              <h4 className="font-semibold text-white">{key}:</h4>
              <p className="text-white">{value}</p>
            </div>
          ))}
        </div>
      )} */}
    </div>
  );
}