"use client"

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

const Login: React.FC = () => {
  const router = useRouter();

  useEffect(() => {

    console.log(123)

    const login = async () => {
      const url = new URL(window.location.href);
      const code = url.searchParams.get('code');

      console.log('URL:', window.location.href);
      console.log('Code:', code);

      if (code) {
        try {
          const response = await axios.post(`http://localhost:5087/api/Auth/login?code=${code}`, null, {
            withCredentials: true,
          }); 

          if (response.status === 200) {
            router.push('/');
          }
        } catch (error) {
          console.error('Login failed:', error);
        }
      }
    };

    login();
  }, [router]);

  return (
    <main className="min-h-screen flex items-center justify-center bg-dark-purple text-white">
      <div className="text-center">
        <h1 className="text-3xl font-bold mb-4">Вход в систему...</h1>
        <p>Пожалуйста, подождите.</p>
      </div>
    </main>
  );
};

export default Login;
