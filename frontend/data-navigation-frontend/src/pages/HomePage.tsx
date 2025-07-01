import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';

function HomePage() {
    return (
    <div className="bg-gray-50 text-gray-900 min-h-screen flex items-center justify-center">
      <div className="max-w-[90vw] mx-auto p-8 md:p-12 text-center bg-white rounded-xl shadow-lg">
        <h1 className="text-4xl md:text-5xl font-extrabold text-blue-800 mb-6">
          Dataspecer specification helper
        </h1>

        <p className="text-lg md:text-xl text-gray-700 mb-8 leading-relaxed">
          Unlock the power of your organization's data specifications. Our application helps you effortlessly
          construct complex data queries in natural language, even if you're not an expert in domain ontology.
        </p>

        <p className="text-md md:text-lg text-gray-600 mb-10 leading-relaxed">
          Simply provide your Dataspecer package, ask your question, and let our intelligent assistant guide you
          to the exact data you need, offering relevant items for query expansion along the way.
        </p>

        <div className="flex flex-col md:flex-row justify-center items-center space-y-4 md:space-y-0 md:space-x-6">
          <Button asChild className="px-6 py-3 rounded-lg font-semibold transition-colors duration-200 ease-in-out bg-blue-600 text-white hover:bg-blue-700 shadow-md w-full md:w-auto">
            <Link to="/conversation">
                Chat bot
            </Link>
          </Button>
          <Button asChild className="px-6 py-3 rounded-lg font-semibold transition-colors duration-200 ease-in-out bg-gray-200 text-gray-800 hover:bg-gray-300 shadow-md w-full md:w-auto">
            <Link to="/manage-conversations">
              Manage existing conversations
            </Link>
          </Button>
        </div>

        <div className="mt-12 text-sm text-gray-400">
          <p>Some optional footer value. Todo: Remove later if not needed.</p>
        </div>
      </div>
    </div>
  );
}

export default HomePage;