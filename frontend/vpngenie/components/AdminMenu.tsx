"use client";

import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface User {
  id: string;
  telegramId: number;
  username: string;
  subscriptionIsActive: boolean;
  subscriptionEndDate: string;
}

interface AdminMenuProps {
  onClose: () => void;
}

const AdminMenu: React.FC<AdminMenuProps> = ({ onClose }) => {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get('http://localhost:5087/api/Admin/get-all-users', {
          withCredentials: true,
        });
        setUsers(response.data);
      } catch (error) {
        console.error('Error fetching users:', error);
        setError('Не удалось загрузить пользователей.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchUsers();
  }, []);

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white p-6 rounded-lg shadow-xl max-w-2xl w-full mx-auto">
        <h2 className="text-2xl font-bold mb-4">Список пользователей</h2>
        {isLoading ? (
          <p className="text-center text-gray-500">Загрузка...</p>
        ) : error ? (
          <p className="text-center text-red-600">{error}</p>
        ) : (
          <div className="overflow-y-auto max-h-96">
            <table className="w-full table-auto border-collapse">
              <thead>
                <tr>
                  <th className="border-b px-4 py-2 text-left">Username</th>
                  <th className="border-b px-4 py-2 text-left">Telegram ID</th>
                  <th className="border-b px-4 py-2 text-left">Подписка активна</th>
                  <th className="border-b px-4 py-2 text-left">Дата окончания подписки</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id}>
                    <td className="border-b px-4 py-2">{user.username || 'N/A'}</td>
                    <td className="border-b px-4 py-2">{user.telegramId}</td>
                    <td className="border-b px-4 py-2">
                      {user.subscriptionIsActive ? 'Да' : 'Нет'}
                    </td>
                    <td className="border-b px-4 py-2">
                      {user.subscriptionIsActive
                        ? new Date(user.subscriptionEndDate).toLocaleDateString()
                        : 'N/A'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        <button
          onClick={onClose}
          className="mt-4 bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg transition-transform transform hover:scale-105"
        >
          Закрыть
        </button>
      </div>
    </div>
  );
};

export default AdminMenu;
