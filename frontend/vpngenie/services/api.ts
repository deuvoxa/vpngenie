import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5087/api',
  withCredentials: true,
});

// export const getUserProfile = async () => {
//   const response = await api.get('/User/profile');
//   return response.data;
// };

export default api