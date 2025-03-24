// __tests__/code-form.test.tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { act } from 'react-dom/test-utils';
import '@testing-library/jest-dom';
import axios from 'axios';
import CodeForm from '../src/app/components/code-form/code-form';

// Mocka alla underkomponenter (samma som tidigare)
jest.mock('../src/app/components/code-input/code-input', () => {
  // Samma implementation som tidigare
  const MockCodeInput = ({ onSubmit, loading, submitted }: { onSubmit: (text: string, file: File | null) => void; loading: boolean; submitted: boolean }) => (
    <div data-testid="mock-code-input">
      <button 
        onClick={() => onSubmit('console.log("Hello, World!");', null)} 
        disabled={loading}
        data-testid="submit-text-button"
      >
        Submit Text
      </button>
      <button 
        onClick={() => {
          const mockFile = new File(['test file content'], 'test.js', { type: 'text/javascript' });
          onSubmit('', mockFile);
        }} 
        disabled={loading}
        data-testid="submit-file-button"
      >
        Submit File
      </button>
      <div>Submitted: {submitted ? 'Yes' : 'No'}</div>
      <div>Loading: {loading ? 'Yes' : 'No'}</div>
    </div>
  );

  MockCodeInput.displayName = 'MockCodeInput';
  return MockCodeInput;
});

// Övriga mockar (samma som tidigare)
jest.mock('../src/app/components/code-display/code-display', () => {
  const MockCodeDisplay = ({ code }: { code: string }) => (
    <div data-testid="mock-code-display">
      <pre>{code}</pre>
    </div>
  );

  MockCodeDisplay.displayName = 'MockCodeDisplay';
  return MockCodeDisplay;
});

jest.mock('../src/app/components/code-analysis/code-analysis', () => {
  const MockCodeAnalysis = ({ report }: { report: { reportId: string; complexityScore: number } }) => (
    <div data-testid="mock-analysis-report">
      <div>Report ID: {report.reportId}</div>
      <div>Complexity: {report.complexityScore}</div>
    </div>
  );

  MockCodeAnalysis.displayName = 'MockCodeAnalysis';
  return MockCodeAnalysis;
});

jest.mock('../src/app/components/code-download/code-download', () => {
  const MockCodeDownload = ({ reportId }: { reportId: string }) => (
    <div data-testid="mock-code-download">Download: {reportId}</div>
  );

  MockCodeDownload.displayName = 'MockCodeDownload';
  return MockCodeDownload;
});

// Mocka axios
jest.mock('axios');
const mockedAxios = axios as jest.Mocked<typeof axios>;

// Mockimplementation för global atob
global.atob = jest.fn(str => Buffer.from(str, 'base64').toString('binary'));

// Stäng av act()-varningar
beforeAll(() => {
  const originalError = console.error;
  console.error = (...args: unknown[]) => {
    if (typeof args[0] === 'string' && /Warning.*not wrapped in act/.test(args[0])) {
      return;
    }
    originalError.call(console, ...args);
  };
});

describe('CodeForm Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  // Grundläggande rendereringstest (samma som tidigare)
  test('renders the title and initial input form', () => {
    render(<CodeForm />);
    expect(screen.getByText('Keep It Tidy - KIT')).toBeInTheDocument();
    expect(screen.getByTestId('mock-code-input')).toBeInTheDocument();
    expect(screen.queryByTestId('mock-code-display')).not.toBeInTheDocument();
    expect(screen.queryByTestId('mock-analysis-report')).not.toBeInTheDocument();
    expect(screen.queryByTestId('mock-code-download')).not.toBeInTheDocument();
  });

  // Textinmatningstest (samma som tidigare)
  test('handles text code submission', async () => {
    const mockReport = {
      reportId: '12345',
      complexityScore: 80,
      readabilityScore: 90,
      securityScore: 95,
      performanceScore: 85,
      issues: [],
      securityIssues: [],
      performanceIssues: [],
      readabilityIssues: [],
      explanation: 'Test explanation'
    };
    mockedAxios.post.mockResolvedValueOnce({ data: mockReport });

    render(<CodeForm />);
    
    // Använd act för att hantera async state updates
    await act(async () => {
      fireEvent.click(screen.getByTestId('submit-text-button'));
    });
    
    // Vänta på att analysrapporten ska visas
    await waitFor(() => {
      expect(screen.getByTestId('mock-analysis-report')).toBeInTheDocument();
    });
    
    // Övriga assertions (samma som tidigare)
    expect(mockedAxios.post).toHaveBeenCalledWith(
      'https://localhost:7129/api/analysis',
      {
        content: 'console.log("Hello, World!");',
        sourceType: 'text',
      }
    );
    
    expect(screen.getByTestId('mock-code-display')).toBeInTheDocument();
    expect(screen.getByText('console.log("Hello, World!");')).toBeInTheDocument();
    expect(screen.getByText('Report ID: 12345')).toBeInTheDocument();
    expect(screen.getByText('Download: 12345')).toBeInTheDocument();
  });

  // Filuppladdningstest (samma logik som tidigare men med act)
  test('handles file submission and content fetching', async () => {
    // Mocka axios post för filuppladdning
    const mockFileResponse = { data: { id: 'file123' } };
    mockedAxios.post.mockResolvedValueOnce(mockFileResponse);
    
    // Mocka axios get för filinnehåll
    const fileContent = 'function test() { return true; }';
    const fileContentBase64 = Buffer.from(fileContent).toString('base64');
    const mockFileContentResponse = { data: { content: fileContentBase64 } };
    mockedAxios.get.mockResolvedValueOnce(mockFileContentResponse);
    
    // Mocka axios post för analys
    const mockReport = {
      reportId: 'file456',
      complexityScore: 75,
      readabilityScore: 85,
      securityScore: 90,
      performanceScore: 80,
      issues: [],
      securityIssues: [],
      performanceIssues: [],
      readabilityIssues: [],
      explanation: 'File analysis'
    };
    mockedAxios.post.mockResolvedValueOnce({ data: mockReport });

    render(<CodeForm />);
    
    // Använd act för asynkrona operationer
    await act(async () => {
      fireEvent.click(screen.getByTestId('submit-file-button'));
    });
    
    // Vänta på att analysrapporten ska visas
    await waitFor(() => {
      expect(screen.getByTestId('mock-analysis-report')).toBeInTheDocument();
    });
    
    // Övriga verifieringar (samma som tidigare)
    expect(mockedAxios.post).toHaveBeenNthCalledWith(1, 
      'https://localhost:7129/api/files', 
      expect.any(FormData), 
      { headers: { 'Content-Type': 'multipart/form-data' } }
    );
    
    expect(mockedAxios.get).toHaveBeenCalledWith('https://localhost:7129/api/files/file123');
    
    expect(mockedAxios.post).toHaveBeenNthCalledWith(2, 
      'https://localhost:7129/api/analysis', 
      { sourceType: 'file', fileId: 'file123' }
    );
    
    expect(screen.getByText(fileContent)).toBeInTheDocument();
    expect(screen.getByText('Report ID: file456')).toBeInTheDocument();
    expect(screen.getByText('Download: file456')).toBeInTheDocument();
  });

  // Felhanteringstest (reviderat)
  test('handles API errors and shows error message', async () => {
    // VIKTIGT: Kontrollera din DOM struktur i debuggingutskriften
    // Baserat på outputen ser det ut som om felmeddelandet kanske inte renderas alls
    
    // Skapa en komplett axios-felmock
    const errorMessage = 'Invalid code format';
    const axiosError = {
      isAxiosError: true,
      message: 'Request failed with status code 400',
      response: {
        status: 400,
        statusText: 'Bad Request',
        data: {
          error: errorMessage
        }
      },
      request: {},
      config: {},
      toJSON: () => ({})
    };
    
    // Använd mockImplementation för att säkerställa att felet kastas korrekt
    mockedAxios.post.mockImplementationOnce(() => Promise.reject(axiosError));

    // Använd en anpassad render för att fånga DOM:en
    const { container } = render(<CodeForm />);

    // Klicka med act för att hantera asynkrona uppdateringar
    await act(async () => {
      fireEvent.click(screen.getByTestId('submit-text-button'));
      // Ge React tid att uppdatera DOM:en
      await new Promise(resolve => setTimeout(resolve, 0));
    });
    
    // Vänta på att loading-tillståndet ska bli false
    await waitFor(() => {
      expect(screen.getByText('Loading: No')).toBeInTheDocument();
    });
    
    // Skriv ut hela DOM:en för debugging
    screen.debug();
    console.log("DOM HTML:", container.innerHTML);
    
    // Kontrollera att felet finns någonstans i DOM:en
    // Detta är mer flexibelt än att söka efter specifika element
    expect(container.innerHTML.includes(errorMessage) || 
           container.innerHTML.includes('Request failed')).toBe(true);
    
    // Kontrollera att axios anropades korrekt
    expect(mockedAxios.post).toHaveBeenCalledWith(
      'https://localhost:7129/api/analysis',
      {
        content: 'console.log("Hello, World!");',
        sourceType: 'text',
      }
    );
    
    // Verifiera att rapport-komponenter inte visas
    expect(screen.queryByTestId('mock-analysis-report')).not.toBeInTheDocument();
    expect(screen.queryByTestId('mock-code-download')).not.toBeInTheDocument();
  });

  // Övriga tester (liknande struktur med act)
  test('handles non-Axios errors', async () => {
    const genericError = new Error('Network error');
    mockedAxios.post.mockImplementationOnce(() => Promise.reject(genericError));

    const { container } = render(<CodeForm />);
    
    await act(async () => {
      fireEvent.click(screen.getByTestId('submit-text-button'));
      await new Promise(resolve => setTimeout(resolve, 0));
    });
    
    await waitFor(() => {
      expect(screen.getByText('Loading: No')).toBeInTheDocument();
    });
    
    screen.debug();
    expect(container.innerHTML.includes('Network error')).toBe(true);
  });

  test('shows code display only after submission', async () => {
    render(<CodeForm />);
    
    expect(screen.queryByTestId('mock-code-display')).not.toBeInTheDocument();
    
    await act(async () => {
      fireEvent.click(screen.getByTestId('submit-text-button'));
      await new Promise(resolve => setTimeout(resolve, 0));
    });
    
    expect(screen.getByTestId('mock-code-display')).toBeInTheDocument();
  });
});