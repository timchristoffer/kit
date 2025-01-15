import { Light as SyntaxHighlighter } from 'react-syntax-highlighter';
import { atomOneDark } from 'react-syntax-highlighter/dist/esm/styles/hljs';

interface CodeDisplayProps {
  code: string;
}

export default function CodeDisplay({ code }: CodeDisplayProps) {
  return (
    <div className="p-2 md:p-4 bg-slate-900 rounded-md max-w-full lg:max-w-2xl mx-auto">
      <h2 className="text-lg md:text-xl font-bold mb-2">Code</h2>
      <div className="overflow-auto max-h-96 w-full">
        <SyntaxHighlighter language="javascript" style={atomOneDark} customStyle={{ whiteSpace: 'pre', overflow: 'auto' }}>
          {code}
        </SyntaxHighlighter>
      </div>
    </div>
  );
}