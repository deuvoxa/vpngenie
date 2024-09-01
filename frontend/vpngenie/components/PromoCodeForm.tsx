"use client";

import React, { useState } from 'react';
import axios from 'axios';

const PromoCodeForm: React.FC = () => {
  const [promoCode, setPromoCode] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleApplyPromoCode = async () => {
    setIsLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await axios.post('http://localhost:5087/api/user/apply-promo-code', { code: promoCode }, {
        withCredentials: true,
      });

      if (response.status === 200) {
        setSuccess('Промокод успешно применен!');
      }
    } catch (error) {
      setError('Ошибка при применении промокода. Попробуйте еще раз.');
      console.error('Error applying promo code:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="text-center p-8 bg-white rounded-2xl shadow-xl w-full">
      <h2 className="text-2xl font-bold text-gray-900 mb-6">
        Система промокодов
      </h2>
      <input
        type="text"
        value={promoCode}
        onChange={(e) => setPromoCode(e.target.value)}
        placeholder="Введите промокод"
        className="mb-4 p-4 w-80 text-dark rounded-full border border-gray-300 focus:outline-none focus:ring-2 focus:ring-neon-light-blue"
      />
      <button
        onClick={handleApplyPromoCode}
        disabled={isLoading}
        className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-3 px-8 rounded-full shadow-lg transition-transform transform hover:scale-105"
      >
        {isLoading ? 'Применение...' : 'Применить'}
      </button>
      {error && <p className="text-red-500 mt-4">{error}</p>}
      {success && <p className="text-green-500 mt-4">{success}</p>}
    </div>
  );
};

export default PromoCodeForm;