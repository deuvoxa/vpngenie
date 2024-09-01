"use client";

import { useState } from 'react';
import axios from 'axios';
import { useRouter } from 'next/navigation';

const LoginForm: React.FC = () => {
  const [inputCode, setInputCode] = useState<string>('');
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);

    try {
      const response = await axios.post(`http://localhost:5087/api/Auth/login?code=${inputCode}`, null, {
        withCredentials: true,
      });

      if (response.status === 200) {
        window.location.reload();
      }
    } catch (error) {
      console.error('Login failed:', error);
      setError('Неверный код или ошибка входа. Попробуйте снова.');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mb-8 flex flex-col items-center">
      <input
        type="text"
        value={inputCode}
        onChange={(e) => setInputCode(e.target.value)}
        className="mb-4 p-4 w-80 text-dark rounded-full border border-gray-300 focus:outline-none focus:ring-2 focus:ring-neon-light-blue"
        placeholder="Введите код"
        required
      />
      <button
        type="submit"
        className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-3 px-8 rounded-full shadow-lg transition-transform transform hover:scale-105"
      >
        Войти
      </button>
      {error && <p className="text-red-500 mt-4">{error}</p>}
    </form>
  );
};

export default LoginForm;
