'use client';

import { useState } from 'react';
import emailjs from 'emailjs-com';
import { lato } from '@/app/styles/fonts';

export default function FeedbackButton() {
  const [isFeedbackModalOpen, setIsFeedbackModalOpen] = useState(false);
  const [feedback, setFeedback] = useState('');
  const [isFeedbackSent, setIsFeedbackSent] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const openFeedbackModal = () => setIsFeedbackModalOpen(true);
  const closeFeedbackModal = () => setIsFeedbackModalOpen(false);

  const sendFeedback = (e: React.FormEvent) => {
    e.preventDefault();
    setIsSending(true);

    const templateParams = {
      to_name: 'Recipient Name', // Replace with the actual recipient name if needed
      from_name: 'Your Name', // Replace with the actual sender name if needed
      message: feedback,
    };

    emailjs.send('service_jjkkdwj', 'template_beyff15', templateParams, 'NGlmTxVLYkLiN_Dx4')
      .then((result) => {
        console.log(result.text);
        setIsFeedbackSent(true);
        setFeedback(''); // Clear the input field
        setIsSending(false);
        closeFeedbackModal();
        setTimeout(() => setIsFeedbackSent(false), 3000); // Hide the message after 3 seconds
      }, (error) => {
        console.log(error.text);
        setIsSending(false);
      });
  };

  return (
    <>
      <button
        onClick={openFeedbackModal}
        className="fixed top-4 left-4 px-2 py-1 sm:px-4 sm:py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
      >
        Feedback
      </button>
      {isFeedbackModalOpen && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
          <div className="bg-white p-6 rounded-md shadow-md max-w-md w-full relative">
            <button onClick={closeFeedbackModal} className="absolute top-2 right-2 text-gray-600 hover:text-gray-800 focus:outline-none">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
            <h3 className="text-xl font-bold mb-4" style={{ color: '#142d55' }}>Feedback</h3>
            <form onSubmit={sendFeedback}>
              <textarea
                value={feedback}
                onChange={(e) => setFeedback(e.target.value)}
                rows={5}
                className={lato.className + " w-full p-2 border border-gray-300 rounded-md text-[#142d55]"}
                placeholder="Write your feedback here..."
              />
              <button
                type="submit"
                className={`mt-4 w-full py-2 px-4 rounded-md focus:outline-none ${isSending || feedback.trim() === '' ? 'bg-gray-400 text-gray-700 cursor-not-allowed' : 'bg-blue-600 text-white hover:bg-blue-700'}`}
                disabled={isSending || feedback.trim() === ''}
              >
                {isSending ? 'Sending...' : 'Send Feedback'}
              </button>
            </form>
          </div>
        </div>
      )}
      {isFeedbackSent && (
        <div className={lato.className + " fixed bottom-4 right-4 bg-green-500 text-white px-4 py-2 rounded-md shadow-md"}>
          Feedback sent successfully!
        </div>
      )}
    </>
  );
}