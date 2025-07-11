import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';

function HomePage() {
    return (
    <div className="bg-gray-50 text-gray-900 min-h-screen flex items-center justify-center">
      <div className="max-w-[90vw] mx-auto p-8 md:p-12 text-center bg-white rounded-xl shadow-lg">
        <h1 className="text-4xl md:text-5xl font-extrabold text-blue-800 mb-6">
          Data specification helper
        </h1>

        <p className="text-lg md:text-xl text-gray-700 mb-8 leading-relaxed">
          Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin convallis finibus magna, in condimentum leo rutrum vitae.
          Ut ut ipsum in mauris porttitor laoreet in quis eros. Nulla sed ipsum pulvinar, rhoncus ipsum ac, tincidunt libero.
          Nulla venenatis vel turpis ac mattis. Morbi sit amet arcu ut leo vestibulum dapibus.
          Curabitur finibus mattis posuere. Nam ac arcu hendrerit, hendrerit risus et, vehicula lectus.
        </p>

        <p className="text-md md:text-lg text-gray-600 mb-10 leading-relaxed">
          Duis tempor id turpis sit amet scelerisque. Duis eu neque velit. Donec vitae dictum tortor, quis condimentum orci.
          Vivamus nec elementum ex. Lorem ipsum dolor sit amet, consectetur adipiscing elit.
          Curabitur egestas vel libero non sagittis. Aenean bibendum nibh lacus, vitae pretium ante euismod id. Fusce consectetur at ligula non molestie.
          Praesent in metus dolor. Integer nisi tellus, sollicitudin eget libero eget, tristique convallis augue. Donec finibus iaculis imperdiet.
          Mauris mollis, mauris in iaculis dignissim, nulla mi hendrerit libero, a ultrices sapien leo at sem.
          Etiam blandit dui velit. Cras scelerisque egestas odio, id sollicitudin sem congue in. Integer iaculis iaculis elit vitae ornare.
        </p>

        <div className="flex flex-col md:flex-row justify-center items-center space-y-4 md:space-y-0 md:space-x-6">
          <Button asChild className="px-6 py-3 rounded-lg font-semibold transition-colors duration-200 ease-in-out bg-blue-600 text-white hover:bg-blue-700 shadow-md w-full md:w-auto">
            <Link to="/conversation">
                Chat bot (this button is temporary)
            </Link>
          </Button>
          <Button asChild className="px-6 py-3 rounded-lg font-semibold transition-colors duration-200 ease-in-out bg-gray-200 text-gray-800 hover:bg-gray-300 shadow-md w-full md:w-auto">
            <Link to="/manage-conversations">
              Manage existing conversations
            </Link>
          </Button>
        </div>
      </div>
    </div>
  );
}

export default HomePage;