import React from 'react';
import axios from 'axios';

interface CodeDownloadProps {
  reportId: string;
}

const CodeDownload: React.FC<CodeDownloadProps> = ({ reportId }) => {
  const handleDownload = async () => {
    try {
      const response = await axios.get(`https://kit-backend.onrender.com/api/analysis/download/${reportId}`, {
        responseType: 'blob', // Viktigt för att hantera binära data
      });

      // Skapa en länk och klicka på den för att ladda ner filen
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${reportId}.pdf`); // Filnamn
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      console.error('Error downloading the report:', error);
    }
  };

  return (
    <button
      onClick={handleDownload}
      className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 mt-2 rounded focus:outline-none focus:shadow-outline"
    >
      Currently not working :D 
    </button>
  );
};

export default CodeDownload;