"use client"

import React, { useEffect, useState } from 'react';
import axios from 'axios';
import LogoutButton from './LogoutButton';
import AdminMenu from './AdminMenu';

const Profile: React.FC = () => {
  const [profile, setProfile] = useState<any>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [isAdminModalOpen, setIsAdminModalOpen] = useState<boolean>(false);
  const [email, setEmail] = useState<string>('');
  const [isAdmin, setIsAdmin] = useState<boolean>(false);

  const adminTelegramId = 761956168;

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await axios.get('http://localhost:5087/api/User/profile', {
          withCredentials: true,
        });
        setProfile(response.data);

        if (response.data.telegramId === adminTelegramId) {
          setIsAdmin(true);
        }
      } catch (error) {
        console.error('Error fetching profile:', error);
      }
    };

    fetchProfile();
  }, []);

  if (!profile) {
    return <p className="text-center text-gray-500">Загрузка...</p>;
  }

  const subscriptionEndDate = new Date(profile.subscriptionEndDate);
  const today = new Date();
  const isSubscriptionActive = subscriptionEndDate > today;

  const handleRenewSubscription = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await axios.post('http://localhost:5087/api/user/renew-subscription', { email: email }, {
        withCredentials: true
      });

      const paymentLink = response.data;
      window.location.href = paymentLink

    } catch (error) {
      setError('Ошибка при продлении подписки. Попробуйте еще раз.');
      console.error('Error renewing subscription:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <LogoutButton />
      <div className="text-center p-8 bg-white rounded-2xl shadow-xl max-w-md mx-auto">
        <h1 className="text-4xl font-bold text-gray-900 mb-6"
            onClick={() => isAdmin && setIsAdminModalOpen(true)}>
          Привет, {profile.username}
        </h1>
        {isSubscriptionActive ? (
          <>
            <p className="text-gray-600 mb-8">
              Подписка активна до: {subscriptionEndDate.toLocaleDateString()}
            </p>
            <div className="flex flex-col md:flex-row justify-center gap-4">
              <button className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105">
                Получить конфиг
              </button>
              <button className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105">
                Сменить регион
              </button>
            </div>
            <button
              onClick={() => setIsModalOpen(true)}
              className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105 mt-4"
            >
              Продлить подписку
            </button>
          </>
        ) : (
          <>
            <p className="text-red-600 font-semibold mb-8">
              У вас нет активной подписки
            </p>
            <button
              onClick={() => setIsModalOpen(true)}
              className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105 mt-4"
            >
              Продлить подписку
            </button>
          </>
        )}
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
          <div className="bg-white p-6 rounded-lg shadow-xl max-w-md mx-auto">
            <h2 className="text-2xl font-bold mb-4">Продлить подписку</h2>
            <p className="text-gray-700 mb-4">Введите ваш email для продления подписки:</p>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Ваш email"
              className="w-full p-2 border border-gray-300 rounded mb-4"
            />
            <button
              onClick={handleRenewSubscription}
              disabled={isLoading}
              className="bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105"
            >
              {isLoading ? 'Продление...' : 'Отправить'}
            </button>
            <button
              onClick={() => setIsModalOpen(false)}
              className="ml-4 mt-4 text-gray-500 hover:text-gray-700"
            >
              Отмена
            </button>
            {error && <p className="text-red-600 font-semibold mt-4">{error}</p>}
          </div>
        </div>
      )}

      {isAdminModalOpen && isAdmin && (
        <AdminMenu onClose={() => setIsAdminModalOpen(false)} />
      )}
    </>
  );
};

export default Profile;
