import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import axios from 'axios';
import CodeDownload from '../src/app/components/code-download/code-download';

// Mocka axios
jest.mock('axios');
const mockedAxios = axios as jest.Mocked<typeof axios>;

describe('CodeDownload Component', () => {
  const reportId = '12345';

  beforeEach(() => {
    // Återställ mockar innan varje test
    jest.clearAllMocks();
  });

  test('renders the download button', () => {
    render(<CodeDownload reportId={reportId} />);
    expect(screen.getByText('Ladda ner rapport som PDF')).toBeInTheDocument();
  });

  test('calls axios and downloads the file when button is clicked', async () => {
    // Mocka axios GET-anrop
    const mockBlob = new Blob(['test content'], { type: 'application/pdf' });
    mockedAxios.get.mockResolvedValueOnce({ data: mockBlob });

    // Rendera först komponenten
    render(<CodeDownload reportId={reportId} />);

    // Skapa alla mockar EFTER rendering
    // Mocka window.URL.createObjectURL
    const createObjectURLMock = jest.fn(() => 'blob:http://localhost:3000/test');
    const originalCreateObjectURL = window.URL.createObjectURL;
    window.URL.createObjectURL = createObjectURLMock;

    // Skapa ett mock-länk objekt
    const mockLink = {
      href: '',
      setAttribute: jest.fn(),
      click: jest.fn(),
      remove: jest.fn()
    };

    // Mocka document createElement EFTER rendering
    const originalCreateElement = document.createElement;
    document.createElement = jest.fn((tag) => {
      if (tag === 'a') {
        return mockLink as unknown as HTMLElement;
      }
      // För andra element, används den ursprungliga metoden
      return originalCreateElement.call(document, tag);
    }) as typeof document.createElement;

    // Mocka appendChild för att undvika DOM-manipulering
    const originalAppendChild = document.body.appendChild;
    document.body.appendChild = jest.fn(() => null) as jest.Mock;

    // Klicka på nedladdningsknappen
    fireEvent.click(screen.getByText('Ladda ner rapport som PDF'));

    // Vänta på asynkrona operationer
    await waitFor(() => {
      // Kontrollera att axios-anropet gjordes
      expect(mockedAxios.get).toHaveBeenCalledWith(`https://localhost:7129/api/analysis/download/${reportId}`, {
        responseType: 'blob',
      });
    });
      
    // Kontrolla att URL.createObjectURL har anropats
    expect(createObjectURLMock).toHaveBeenCalled();
    
    // Kontrollera att createElement anropades
    expect(document.createElement).toHaveBeenCalledWith('a');
    
    // Kontrollera att append och manipulering av länken gjordes
    expect(mockLink.setAttribute).toHaveBeenCalledWith('download', `${reportId}.pdf`);
    expect(document.body.appendChild).toHaveBeenCalled();
    expect(mockLink.click).toHaveBeenCalled();
    expect(mockLink.remove).toHaveBeenCalled();

    // Återställ mockar
    window.URL.createObjectURL = originalCreateObjectURL;
    document.createElement = originalCreateElement;
    document.body.appendChild = originalAppendChild;
  });

  test('logs an error if the download fails', async () => {
    // Mocka axios GET-anrop för att kasta ett fel
    const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
    mockedAxios.get.mockRejectedValueOnce(new Error('Download failed'));

    // Rendera komponenten
    render(<CodeDownload reportId={reportId} />);

    // Klicka på nedladdningsknappen
    fireEvent.click(screen.getByText('Ladda ner rapport som PDF'));

    // Vänta på att det asynkrona felflödet körs klart
    await waitFor(() => {
      // Kontrollera att axios-anropet gjordes
      expect(mockedAxios.get).toHaveBeenCalledWith(`https://localhost:7129/api/analysis/download/${reportId}`, {
        responseType: 'blob',
      });
      
      // Kontrollera att felet loggades
      expect(consoleErrorSpy).toHaveBeenCalledWith('Error downloading the report:', expect.any(Error));
    });

    // Återställ console.error spy
    consoleErrorSpy.mockRestore();
  });
});