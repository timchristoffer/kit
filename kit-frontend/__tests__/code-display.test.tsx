import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import CodeDisplay from '../src/app/components/code-display/code-display';

// Mocka react-syntax-highlighter för att undvika problem med ESM-import
jest.mock('react-syntax-highlighter', () => ({
  Light: ({ children, language, customStyle }: { children: React.ReactNode; language: string; customStyle: React.CSSProperties }) => (
    <pre data-testid="syntax-highlighter" data-language={language} style={customStyle}>
      {children}
    </pre>
  )
}));

jest.mock('react-syntax-highlighter/dist/esm/styles/hljs', () => ({
  atomOneDark: {}
}));

describe('CodeDisplay Component', () => {
  const sampleCode = `
    function helloWorld() {
      console.log('Hello, World!');
      return true;
    }
  `;

  test('renders the component with a title', () => {
    render(<CodeDisplay code={sampleCode} />);
    expect(screen.getByText('Code')).toBeInTheDocument();
  });

  test('renders the syntax highlighter', () => {
    render(<CodeDisplay code={sampleCode} />);
    const highlighter = screen.getByTestId('syntax-highlighter');
    expect(highlighter).toBeInTheDocument();
  });

  test('passes the correct language to syntax highlighter', () => {
    render(<CodeDisplay code={sampleCode} />);
    const highlighter = screen.getByTestId('syntax-highlighter');
    expect(highlighter).toHaveAttribute('data-language', 'javascript');
  });

  test('displays the provided code', () => {
    render(<CodeDisplay code={sampleCode} />);
    expect(screen.getByText(/function helloWorld/)).toBeInTheDocument();
    expect(screen.getByText(/console\.log\('Hello, World!'\);/)).toBeInTheDocument();
  });

  test('handles empty code string', () => {
    render(<CodeDisplay code="" />);
    const highlighter = screen.getByTestId('syntax-highlighter');
    expect(highlighter.textContent).toBe('');
  });

  test('applies custom styles for overflow', () => {
    render(<CodeDisplay code={sampleCode} />);
    const highlighter = screen.getByTestId('syntax-highlighter');
    
    // Kontrollera att vi har de rätta style-attributen
    expect(highlighter).toHaveStyle('white-space: pre');
    expect(highlighter).toHaveStyle('overflow: auto');
  });

  test('contains parent div with proper classes', () => {
    const { container } = render(<CodeDisplay code={sampleCode} />);
    const parentDiv = container.firstChild;
    
    // Kontrollera att parent div har rätt klasser
    expect(parentDiv).toHaveClass('p-2');
    expect(parentDiv).toHaveClass('md:p-4');
    expect(parentDiv).toHaveClass('bg-slate-900');
    expect(parentDiv).toHaveClass('rounded-md');
  });

  test('has a wrapper div with correct height limitation', () => {
    const { container } = render(<CodeDisplay code={sampleCode} />);
    const wrapperDiv = container.querySelector('.overflow-auto');
    
    expect(wrapperDiv).toHaveClass('max-h-96');
    expect(wrapperDiv).toHaveClass('w-full');
  });
});