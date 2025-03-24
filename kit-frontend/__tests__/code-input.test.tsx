// __tests__/code-input.test.tsx
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { act } from 'react'; // Använd act från 'react' istället för 'react-dom/test-utils'
import CodeInputForm from '../src/app/components/code-input/code-input';

// Mocka @heroicons/react/24/solid komponenten
jest.mock('@heroicons/react/24/solid', () => ({
  XMarkIcon: () => <div data-testid="x-mark-icon">X</div>
}));

describe('CodeInputForm Component', () => {
  const mockOnSubmit = jest.fn();
  
  beforeEach(() => {
    mockOnSubmit.mockClear();
  });

  test('renders form with all expected elements', () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    // Kontrollera att grundläggande element finns
    expect(screen.getByPlaceholderText('Paste your code here')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Analyze' })).toBeInTheDocument();
    expect(screen.getByRole('textbox')).toBeInTheDocument();
    
    // Kontrollera att fil-input finns - använd querySelector istället för getByRole
    const fileInput = document.querySelector('input[type="file"]');
    expect(fileInput).toBeInTheDocument();
  });

  test('submit button should be disabled when no code or file is provided', () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    const submitButton = screen.getByRole('button', { name: 'Analyze' });
    expect(submitButton).toBeDisabled();
  });

  test('submit button should be enabled when code is provided', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    const textArea = screen.getByPlaceholderText('Paste your code here');
    await act(async () => {
      fireEvent.change(textArea, { target: { value: 'const x = 10;' } });
    });
    
    const submitButton = screen.getByRole('button', { name: 'Analyze' });
    expect(submitButton).not.toBeDisabled();
  });

  test('submit button should show loading text when loading is true', () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={true} submitted={false} />);
    
    expect(screen.getByRole('button', { name: 'Analyzing...' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Analyzing...' })).toBeDisabled();
  });

  test('onSubmit is called with code when form is submitted', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    const textArea = screen.getByPlaceholderText('Paste your code here');
    const testCode = 'function test() { return true; }';
    
    await act(async () => {
      fireEvent.change(textArea, { target: { value: testCode } });
    });
    
    // Använd querySelector istället för getByRole för att hitta formuläret
    const form = document.querySelector('form');
    await act(async () => {
      fireEvent.submit(form);
    });
    
    expect(mockOnSubmit).toHaveBeenCalledWith(testCode, null);
  });

  test('textarea is cleared after form submission', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    const textArea = screen.getByPlaceholderText('Paste your code here');
    
    await act(async () => {
      fireEvent.change(textArea, { target: { value: 'const y = 20;' } });
    });
    
    expect(textArea).toHaveValue('const y = 20;');
    
    // Använd querySelector istället för getByRole för att hitta formuläret
    const form = document.querySelector('form');
    await act(async () => {
      fireEvent.submit(form);
    });
    
    expect(textArea).toHaveValue('');
  });

  test('form uses submitted class when submitted is true', () => {
    const { container } = render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={true} />);
    
    const form = container.querySelector('form');
    expect(form).toHaveClass('lg:max-w-md');
  });

  test('form uses non-submitted class when submitted is false', () => {
    const { container } = render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    const form = container.querySelector('form');
    expect(form).toHaveClass('md:max-w-2xl');
  });

  test('file input works correctly', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    // Använd querySelector istället för getByLabelText för att hitta fileinput
    const fileInput = document.querySelector('input[type="file"]');
    expect(fileInput).toBeInTheDocument();
    
    // Skapa en test-fil
    const testFile = new File(['test file content'], 'test.js', { type: 'text/javascript' });
    
    // Simulera en fil-uppladdning
    await act(async () => {
      fireEvent.change(fileInput, { target: { files: [testFile] } });
    });
    
    // Verifiera att submit-knappen är aktiverad
    const submitButton = screen.getByRole('button', { name: 'Analyze' });
    expect(submitButton).not.toBeDisabled();
    
    // Verifiera att ta-bort-knappen visas
    expect(screen.getByTestId('x-mark-icon')).toBeInTheDocument();
    
    // Skicka in formuläret
    const form = document.querySelector('form');
    await act(async () => {
      fireEvent.submit(form);
    });
    
    // Verifiera att onSubmit anropas med filen
    expect(mockOnSubmit).toHaveBeenCalledWith('', testFile);
  });

  test('remove file button works correctly', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    // Använd querySelector istället för getByLabelText för att hitta fileinput
    const fileInput = document.querySelector('input[type="file"]');
    
    // Ladda upp en fil
    const testFile = new File(['test file content'], 'test.js', { type: 'text/javascript' });
    await act(async () => {
      fireEvent.change(fileInput, { target: { files: [testFile] } });
    });
    
    // Verifiera att ta-bort-knappen visas
    const removeButton = screen.getByTestId('x-mark-icon').closest('button');
    expect(removeButton).toBeInTheDocument();
    
    // Klicka på ta-bort-knappen
    await act(async () => {
      fireEvent.click(removeButton);
    });
    
    // Verifiera att ta-bort-knappen inte längre visas
    expect(screen.queryByTestId('x-mark-icon')).not.toBeInTheDocument();
    
    // Verifiera att submit-knappen är inaktiverad igen (eftersom ingen fil eller kod finns)
    expect(screen.getByRole('button', { name: 'Analyze' })).toBeDisabled();
  });

  test('can submit both code and file', async () => {
    render(<CodeInputForm onSubmit={mockOnSubmit} loading={false} submitted={false} />);
    
    // Ange kod
    const textArea = screen.getByPlaceholderText('Paste your code here');
    const testCode = 'console.log("test");';
    await act(async () => {
      fireEvent.change(textArea, { target: { value: testCode } });
    });
    
    // Ladda upp fil
    const fileInput = document.querySelector('input[type="file"]');
    const testFile = new File(['test file content'], 'test.js', { type: 'text/javascript' });
    await act(async () => {
      fireEvent.change(fileInput, { target: { files: [testFile] } });
    });
    
    // Skicka in formuläret
    const form = document.querySelector('form');
    await act(async () => {
      fireEvent.submit(form);
    });
    
    // Verifiera att onSubmit anropas med både kod och fil
    expect(mockOnSubmit).toHaveBeenCalledWith(testCode, testFile);
  });
});