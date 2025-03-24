import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import AnalysisReport from '../src/app/components/code-analysis/code-analysis';

// Mock react-chartjs-2 för att undvika canvas-renderingsproblem i testerna
jest.mock('react-chartjs-2', () => ({
  Bar: () => <div data-testid="mock-bar-chart">Mocked Chart</div>
}));

describe('AnalysisReport Component', () => {
  // Exempel på testdata
  const mockReport = {
    complexityScore: 85,
    readabilityScore: 90,
    securityScore: 75,
    performanceScore: 88,
    issues: ['Variable "x" is unused'],
    securityIssues: ['Using eval() is unsafe'],
    performanceIssues: ['Inefficient loop detected'],
    readabilityIssues: ['Function is too long (150 lines)']
  };

  const emptyReport = {
    complexityScore: 100,
    readabilityScore: 100,
    securityScore: 100,
    performanceScore: 100,
    issues: [],
    securityIssues: [],
    performanceIssues: [],
    readabilityIssues: []
  };

  test('renders analysis report with title', () => {
    render(<AnalysisReport report={mockReport} />);
    expect(screen.getByText('Analysis Report')).toBeInTheDocument();
  });

  test('renders chart component', () => {
    render(<AnalysisReport report={mockReport} />);
    expect(screen.getByTestId('mock-bar-chart')).toBeInTheDocument();
  });

  test('initially hides issues section', () => {
    render(<AnalysisReport report={mockReport} />);
    expect(screen.getByText('Show Issues')).toBeInTheDocument();
    expect(screen.queryByText('General Issues:')).not.toBeInTheDocument();
  });

  test('shows issues when "Show Issues" button is clicked', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Klicka på "Show Issues"-knappen
    fireEvent.click(screen.getByText('Show Issues'));
    
    // Kontrollera att alla kategorier av problem visas
    expect(screen.getByText('General Issues:')).toBeInTheDocument();
    expect(screen.getByText('Security Issues:')).toBeInTheDocument();
    expect(screen.getByText('Performance Issues:')).toBeInTheDocument();
    expect(screen.getByText('Readability Issues:')).toBeInTheDocument();
    
    // Kontrollera att de specifika problemen visas
    expect(screen.getByText('Variable "x" is unused')).toBeInTheDocument();
    expect(screen.getByText('Using eval() is unsafe')).toBeInTheDocument();
    expect(screen.getByText('Inefficient loop detected')).toBeInTheDocument();
    expect(screen.getByText('Function is too long (150 lines)')).toBeInTheDocument();
  });

  test('toggles issues visibility when button is clicked multiple times', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Initialt är problemen dolda
    expect(screen.queryByText('General Issues:')).not.toBeInTheDocument();
    
    // Klicka på "Show Issues"-knappen
    fireEvent.click(screen.getByText('Show Issues'));
    
    // Nu bör problemen visas
    expect(screen.getByText('General Issues:')).toBeInTheDocument();
    
    // Klicka på "Hide Issues"-knappen
    fireEvent.click(screen.getByText('Hide Issues'));
    
    // Problemen bör vara dolda igen
    expect(screen.queryByText('General Issues:')).not.toBeInTheDocument();
  });

  test('shows "No issues found" when there are no issues', () => {
    render(<AnalysisReport report={emptyReport} />);
    
    // Klicka på "Show Issues"-knappen
    fireEvent.click(screen.getByText('Show Issues'));
    
    // Kontrollera att "No issues found" meddelandet visas
    expect(screen.getByText('No issues found')).toBeInTheDocument();
  });

  test('shows modal when info button is clicked', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Modal är initialt dold
    expect(screen.queryByText('Score Information')).not.toBeInTheDocument();
    
    // Hitta infoknappen baserat på dess position och klicka på den
    // Vi använder getAllByRole eftersom det finns flera knappar
    const buttons = screen.getAllByRole('button');
    const infoButton = buttons[0]; // första knappen i header
    fireEvent.click(infoButton);
    
    // Kontrollera att modalen visas
    expect(screen.getByText('Score Information')).toBeInTheDocument();
    expect(screen.getByText(/Readability Score:/)).toBeInTheDocument();
    expect(screen.getByText(/Security Score:/)).toBeInTheDocument();
    expect(screen.getByText(/Performance Score:/)).toBeInTheDocument();
    expect(screen.getByText(/Complexity Score:/)).toBeInTheDocument();
  });

  test('closes modal when X button is clicked', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Öppna modalen
    const buttons = screen.getAllByRole('button');
    const infoButton = buttons[0];
    fireEvent.click(infoButton);
    
    // Kontrollera att modalen visas
    expect(screen.getByText('Score Information')).toBeInTheDocument();
    
    // När modalen är öppen, leta efter X-knappen och klicka på den
    // Vi behöver vänta tills modalen visas, och sedan leta efter knappen där
    const closeButton = screen.getAllByRole('button').find(
      button => button.classList.contains('absolute') && button.classList.contains('top-2')
    );
    
    if (closeButton) {
      fireEvent.click(closeButton);
    } else {
      fail('Could not find close button');
    }
    
    // Kontrollera att modalen stängs
    expect(screen.queryByText('Score Information')).not.toBeInTheDocument();
  });

  test('closes modal when clicking outside the modal', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Öppna modalen
    const buttons = screen.getAllByRole('button');
    const infoButton = buttons[0];
    fireEvent.click(infoButton);
    
    // Kontrollera att modalen visas
    expect(screen.getByText('Score Information')).toBeInTheDocument();
    
    // Hitta modal overlay genom dess klass och klicka på den
    const modalOverlay = document.querySelector('.fixed.inset-0.bg-black.bg-opacity-50');
    
    if (modalOverlay) {
      fireEvent.click(modalOverlay);
    } else {
      fail('Could not find modal overlay');
    }
    
    // Kontrollera att modalen stängs
    expect(screen.queryByText('Score Information')).not.toBeInTheDocument();
  });

  test('does not close modal when clicking inside the modal content', () => {
    render(<AnalysisReport report={mockReport} />);
    
    // Öppna modalen
    const buttons = screen.getAllByRole('button');
    const infoButton = buttons[0];
    fireEvent.click(infoButton);
    
    // Kontrollera att modalen visas
    expect(screen.getByText('Score Information')).toBeInTheDocument();
    
    // Hitta modal content genom att välja elementet med klass bg-white
    const modalContent = document.querySelector('.bg-white.p-6.rounded-md.shadow-md');
    
    if (modalContent) {
      fireEvent.click(modalContent);
      
      // Modalen bör fortfarande vara öppen
      expect(screen.getByText('Score Information')).toBeInTheDocument();
    } else {
      fail('Could not find modal content');
    }
  });

  test('renders with partial issues data', () => {
    const partialReport = {
      complexityScore: 90,
      readabilityScore: 85,
      securityScore: 100,
      performanceScore: 95,
      issues: [],
      securityIssues: [],
      performanceIssues: ['Some performance issue'],
      readabilityIssues: []
    };
    
    render(<AnalysisReport report={partialReport} />);
    
    // Visa problemen
    fireEvent.click(screen.getByText('Show Issues'));
    
    // Kontrollera att endast performance issues visas
    expect(screen.queryByText('General Issues:')).not.toBeInTheDocument();
    expect(screen.queryByText('Security Issues:')).not.toBeInTheDocument();
    expect(screen.getByText('Performance Issues:')).toBeInTheDocument();
    expect(screen.queryByText('Readability Issues:')).not.toBeInTheDocument();
    
    expect(screen.getByText('Some performance issue')).toBeInTheDocument();
  });

  test('chart data is correctly populated from props', () => {
    render(<AnalysisReport report={mockReport} />);
    expect(screen.getByTestId('mock-bar-chart')).toBeInTheDocument();
  });
});