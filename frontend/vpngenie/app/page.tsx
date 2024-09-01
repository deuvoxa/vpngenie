import Profile from '../components/Profile';
import { cookies } from 'next/headers';
import Link from 'next/link';
import LoginForm from '@/components/LoginForm';

export default async function HomePage() {
  const cookieStore = cookies();
  const token = cookieStore.get('auth_cookie')?.value;

  if (!token) {
    return (
      <main className="min-h-screen flex flex-col items-center justify-center bg-light-blue">
        <div className="text-center">
          <h1 className="text-4xl font-bold text-dark mb-6">
            Войдите в систему
          </h1>
          <p className="text-gray-500 mb-8">
            Напишите /login в нашем {' '}
            <Link
              href="https://your-bot-link.com"
              className="relative text-blue-500 hover:text-transparent   bg-clip-text hover:bg-gradient-to-b from-blue-300 to-blue-400 transition-colors duration-300"
            >
              боте
            </Link>, чтобы получить код и введите код ниже:
          </p>

          <LoginForm />
        </div>
      </main>
    );
  }

  return (
    <main className="relative min-h-screen flex flex-col items-center justify-center bg-light-blue text-dark">
      <Profile />
    </main>
  );
}
