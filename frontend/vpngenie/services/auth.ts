import { cookies } from 'next/headers';

export const getToken = () => {
  const cookieStore = cookies();
  const token = cookieStore.get('auth_cookie');
  return token?.value;
};
