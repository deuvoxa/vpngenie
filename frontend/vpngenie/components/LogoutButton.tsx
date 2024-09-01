"use client"

import React from 'react';

const LogoutButton: React.FC = () => {
    const handleLogout = async () => {
        await fetch('http://localhost:5087/api/auth/logout', {
          method: 'POST',
          credentials: 'include',
        });
        window.location.reload();
      };

  return (
    <button
        onClick={handleLogout}
        className="absolute top-4 right-4 bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold py-3 px-8 rounded-full shadow-lg transition-transform transform hover:scale-105"
      >
        Выйти
      </button>
  );
};

export default LogoutButton;